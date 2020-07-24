namespace Zsharp.LightweightIndexer.Entity
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NBitcoin;
    using Zsharp.Bitcoin;
    using Zsharp.Elysium;
    using Zsharp.Entity;

    public sealed class BlockRepository : Zsharp.LightweightIndexer.IBlockRepository
    {
        readonly Network network;
        readonly IDbContextFactory<DbContext> db;
        readonly ITransactionSerializer elysiumSerializer;

        public BlockRepository(
            Network network,
            IDbContextFactory<DbContext> db,
            ITransactionSerializer elysiumSerializer)
        {
            this.network = network;
            this.db = db;
            this.elysiumSerializer = elysiumSerializer;
        }

        public async ValueTask AddBlockAsync(
            NBitcoin.Block block,
            int height,
            CancellationToken cancellationToken = default)
        {
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            await using (var db = await this.db.CreateAsync(cancellationToken))
            {
                await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

                var entity = this.ToEntity(block, height);

                // Do not insert transactions that already exists.
                var transactions = entity.Transactions
                    .Select(t => t.TransactionHash)
                    .ToList();

                var existed = await db.Transactions
                    .Where(t => transactions.Contains(t.Hash))
                    .ToDictionaryAsync(t => t.Hash, cancellationToken);

                foreach (var tx in entity.Transactions.Where(t => existed.ContainsKey(t.TransactionHash)))
                {
                    tx.Transaction = null;
                }

                // Commit.
                await db.Blocks.AddAsync(entity, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                db.Database.CommitTransaction();
            }
        }

        public async ValueTask<(NBitcoin.Block? Block, int Height)> GetBlockAsync(
            uint256 hash,
            CancellationToken cancellationToken = default)
        {
            await using (var db = await this.db.CreateAsync(cancellationToken))
            {
                await db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);

                var block = await db.Blocks.SingleOrDefaultAsync(b => b.Hash == hash, cancellationToken);

                if (block == null)
                {
                    return (null, -1);
                }

                return (await this.ToDomainAsync(db, block, cancellationToken), block.Height);
            }
        }

        public async ValueTask<NBitcoin.Block?> GetBlockAsync(int height, CancellationToken cancellationToken = default)
        {
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            await using (var db = await this.db.CreateAsync(cancellationToken))
            {
                await db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);

                var block = await db.Blocks.SingleOrDefaultAsync(b => b.Height == height, cancellationToken);

                if (block == null)
                {
                    return null;
                }

                return await this.ToDomainAsync(db, block, cancellationToken);
            }
        }

        public async ValueTask<(IEnumerable<NBitcoin.Block> Blocks, int Highest)> GetLatestsBlocksAsync(
            int count,
            CancellationToken cancellationToken = default)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            await using (var db = await this.db.CreateAsync(cancellationToken))
            {
                await db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);

                var blocks = await db.Blocks
                    .OrderByDescending(b => b.Height)
                    .Take(count)
                    .ToListAsync(cancellationToken);
                var result = new List<NBitcoin.Block>(blocks.Count);

                foreach (var block in blocks)
                {
                    result.Add(await this.ToDomainAsync(db, block, cancellationToken));
                }

                return (result, (blocks.Count > 0) ? blocks[0].Height : -1);
            }
        }

        public async ValueTask<NBitcoin.Transaction?> GetTransactionAsync(
            uint256 hash,
            CancellationToken cancellationToken = default)
        {
            await using (var db = await this.db.CreateAsync(cancellationToken))
            {
                await db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken);

                var tx = await db.Transactions.SingleOrDefaultAsync(t => t.Hash == hash, cancellationToken);

                if (tx == null)
                {
                    return null;
                }

                return await this.ToDomainAsync(db, tx, cancellationToken);
            }
        }

        public async ValueTask RemoveLastBlockAsync(CancellationToken cancellationToken = default)
        {
            await using (var db = await this.db.CreateAsync(cancellationToken))
            {
                await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

                // Remove block.
                var block = await db.Blocks
                    .Include(b => b.Transactions)
                    .ThenInclude(j => j.Transaction)
                    .ThenInclude(t => t!.Blocks)
                    .OrderByDescending(b => b.Height)
                    .FirstOrDefaultAsync(cancellationToken);

                if (block == null)
                {
                    return;
                }

                db.Blocks.Remove(block);

                // Remove referenced transactions if no other blocks referencing it.
                foreach (var tx in block.Transactions.Select(j => j.Transaction))
                {
                    Debug.Assert(tx != null, "The referencing transaction is null.");

                    if (tx.Blocks.Select(j => j.BlockHash).Distinct().Count() > 1)
                    {
                        continue;
                    }

                    db.Transactions.Remove(tx);
                }

                // Commit.
                await db.SaveChangesAsync(cancellationToken);

                db.Database.CommitTransaction();
            }
        }

        async Task<NBitcoin.Block> ToDomainAsync(
            DbContext db,
            Block block,
            CancellationToken cancellationToken = default)
        {
            // Block.
            var domain = NBitcoin.Block.CreateBlock(this.network);

            domain.Header.Version = block.Version;
            domain.Header.HashMerkleRoot = block.MerkleRoot;
            domain.Header.BlockTime = DateTime.SpecifyKind(block.Time, DateTimeKind.Utc);
            domain.Header.Bits = block.Target;
            domain.Header.Nonce = (uint)block.Nonce;

            if (block.Height != 0)
            {
                var previous = await db.Blocks.SingleAsync(b => b.Height == block.Height - 1, cancellationToken);
                domain.Header.HashPrevBlock = previous.Hash;
            }

            // MTP.
            await db
                .Entry(block)
                .Reference(b => b.MtpData)
                .LoadAsync(cancellationToken);

            var mtp = block.MtpData;

            if (mtp != null)
            {
                if (!domain.Header.IsMtp())
                {
                    throw new ArgumentException(
                        "The data is MTP-enabled but the consensus is not activated.",
                        nameof(block));
                }

                domain.Header.SetMtpHash(mtp.Hash);
                domain.Header.SetMtpVersion(mtp.Version);
                domain.Header.SetReserved1(mtp.Reserved1);
                domain.Header.SetReserved2(mtp.Reserved2);
            }

            // Transactions.
            await db
                .Entry(block)
                .Collection(b => b.Transactions)
                .LoadAsync(cancellationToken);

            foreach (var join in block.Transactions)
            {
                await db
                    .Entry(join)
                    .Reference(j => j.Transaction)
                    .LoadAsync(cancellationToken);

                domain.Transactions.Add(await this.ToDomainAsync(db, join.Transaction, cancellationToken));
            }

            return domain;
        }

        async Task<NBitcoin.Transaction> ToDomainAsync(
            DbContext db,
            Transaction? tx,
            CancellationToken cancellationToken = default)
        {
            if (tx == null)
            {
                throw new ArgumentNullException(nameof(tx));
            }

            // Transaction.
            var domain = NBitcoin.Transaction.Create(this.network);

            domain.SetTransactionType(tx.Type);
            domain.Version = (uint)tx.Version;
            domain.LockTime = tx.LockTime;

            // Outputs.
            await db
                .Entry(tx)
                .Collection(t => t.Outputs)
                .LoadAsync(cancellationToken);

            foreach (var output in tx.Outputs)
            {
                domain.Outputs.Add(output.Value, output.Script);
            }

            // Inputs.
            await db
                .Entry(tx)
                .Collection(t => t.Inputs)
                .LoadAsync(cancellationToken);

            foreach (var input in tx.Inputs)
            {
                domain.Inputs.Add(
                    new OutPoint(input.OutputHash, (uint)input.OutputIndex),
                    input.Script,
                    null,
                    (uint)input.Sequence);
            }

            // Extra payload.
            var extra = tx.ExtraPayload;

            if (extra != null)
            {
                domain.SetExtraPayload(extra);
            }

            // Elysium.
            await db
                .Entry(tx)
                .Reference(t => t.Elysium)
                .LoadAsync(cancellationToken);

            var elysium = tx.Elysium;

            if (elysium != null)
            {
                domain.SetElysiumTransaction(this.ToDomain(elysium));
            }

            return domain;
        }

        Elysium.Transaction ToDomain(ElysiumTransaction tx)
        {
            var sender = (tx.Sender != null)
                ? BitcoinAddress.Create(tx.Sender, this.network)
                : null;
            var receiver = (tx.Receiver != null)
                ? BitcoinAddress.Create(tx.Receiver, this.network)
                : null;
            var serialized = new SequenceReader<byte>(new ReadOnlySequence<byte>(tx.Serialized));

            return this.elysiumSerializer.Deserialize(sender, receiver, ref serialized);
        }

        Block ToEntity(NBitcoin.Block block, int height)
        {
            // Block.
            var header = block.Header;
            var entity = new Block(
                height,
                header.GetHash(),
                header.BlockTime.UtcDateTime,
                header.Version,
                header.Bits,
                (int)header.Nonce,
                header.HashMerkleRoot);

            if (header.IsMtp())
            {
                entity.MtpData = new MtpData(
                    entity.Hash,
                    header.GetMtpHash(),
                    header.GetMtpVersion(),
                    header.GetReserved1(),
                    header.GetReserved2());
            }

            // Transactions.
            var transactions = new HashSet<uint256>();

            for (int i = 0; i < block.Transactions.Count; i++)
            {
                var hash = block.Transactions[i].GetHash();
                var join = new BlockTransaction(entity.Hash, i, hash);

                if (transactions.Add(hash))
                {
                    join.Transaction = this.ToEntity(block.Transactions[i]);
                }

                entity.Transactions.Add(join);
            }

            return entity;
        }

        Transaction ToEntity(NBitcoin.Transaction tx)
        {
            var entity = new Transaction(
                tx.GetHash(),
                tx.GetTransactionType(),
                (short)Convert.ToUInt16(tx.Version),
                tx.LockTime);

            // Outputs.
            for (int i = 0; i < tx.Outputs.Count; i++)
            {
                var output = new Output(entity.Hash, i, tx.Outputs[i].ScriptPubKey, tx.Outputs[i].Value);
                entity.Outputs.Add(output);
            }

            // Inputs.
            for (int i = 0; i < tx.Inputs.Count; i++)
            {
                var input = new Input(
                    entity.Hash,
                    i,
                    tx.Inputs[i].PrevOut.Hash,
                    (int)tx.Inputs[i].PrevOut.N,
                    tx.Inputs[i].ScriptSig,
                    (int)tx.Inputs[i].Sequence.Value);

                entity.Inputs.Add(input);
            }

            // Extra payload.
            var extra = tx.GetExtraPayload();

            if (extra.Length != 0)
            {
                entity.ExtraPayload = extra;
            }

            // Elysium.
            var elysium = tx.GetElysiumTransaction();

            if (elysium != null)
            {
                var serialized = new ArrayBufferWriter<byte>();

                this.elysiumSerializer.Serialize(serialized, elysium);

                entity.Elysium = new ElysiumTransaction(
                    entity.Hash,
                    elysium.Sender?.ToString(),
                    elysium.Receiver?.ToString(),
                    serialized.WrittenSpan.ToArray());
            }

            return entity;
        }
    }
}
