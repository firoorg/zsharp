namespace Zsharp.LightweightIndexer.Entity.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Zsharp.Entity;
    using DbContext = Zsharp.LightweightIndexer.Entity.DbContext;

    sealed class FakeDbContext : DbContext
    {
        public FakeDbContext(DbContextOptions<DbContext> options) : base(options)
        {
        }

        protected override void ConfigureBlock(EntityTypeBuilder<Block> builder)
        {
            base.ConfigureBlock(builder);

            builder.Property(b => b.Hash).HasConversion(Converters.UInt256ToBytes);
            builder.Property(b => b.MerkleRoot).HasConversion(Converters.UInt256ToBytes);
            builder.Property(b => b.Target).HasConversion(Converters.TargetToInt32);
        }

        protected override void ConfigureBlockTransaction(EntityTypeBuilder<BlockTransaction> builder)
        {
            base.ConfigureBlockTransaction(builder);

            builder.Property(j => j.BlockHash).HasConversion(Converters.UInt256ToBytes);
            builder.Property(j => j.TransactionHash).HasConversion(Converters.UInt256ToBytes);
        }

        protected override void ConfigureElysiumTransaction(EntityTypeBuilder<ElysiumTransaction> builder)
        {
            base.ConfigureElysiumTransaction(builder);

            builder.Property(t => t.TransactionHash).HasConversion(Converters.UInt256ToBytes);
        }

        protected override void ConfigureInput(EntityTypeBuilder<Input> builder)
        {
            base.ConfigureInput(builder);

            builder.Property(i => i.OutputHash).HasConversion(Converters.UInt256ToBytes);
            builder.Property(i => i.Script).HasConversion(Converters.ScriptToBytes);
            builder.Property(i => i.TransactionHash).HasConversion(Converters.UInt256ToBytes);
        }

        protected override void ConfigureMtpData(EntityTypeBuilder<MtpData> builder)
        {
            base.ConfigureMtpData(builder);

            builder.Property(m => m.BlockHash).HasConversion(Converters.UInt256ToBytes);
            builder.Property(m => m.Hash).HasConversion(Converters.UInt256ToBytes);
            builder.Property(m => m.Reserved1).HasConversion(Converters.UInt256ToBytes);
            builder.Property(m => m.Reserved2).HasConversion(Converters.UInt256ToBytes);
        }

        protected override void ConfigureOutput(EntityTypeBuilder<Output> builder)
        {
            base.ConfigureOutput(builder);

            builder.Property(o => o.TransactionHash).HasConversion(Converters.UInt256ToBytes);
            builder.Property(o => o.Script).HasConversion(Converters.ScriptToBytes);
        }

        protected override void ConfigureTransaction(EntityTypeBuilder<Transaction> builder)
        {
            base.ConfigureTransaction(builder);

            builder.Property(t => t.Hash).HasConversion(Converters.UInt256ToBytes);
        }
    }
}
