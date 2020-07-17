namespace Zsharp.LightweightIndexer.Entity
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public abstract class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        public DbSet<Block> Blocks { get; set; } = null!;

        public DbSet<BlockTransaction> BlockTransactions { get; set; } = null!;

        public DbSet<ElysiumTransaction> ElysiumTransactions { get; set; } = null!;

        public DbSet<Input> Inputs { get; set; } = null!;

        public DbSet<MtpData> MtpData { get; set; } = null!;

        public DbSet<Output> Outputs { get; set; } = null!;

        public DbSet<Transaction> Transactions { get; set; } = null!;

        protected virtual void ConfigureBlock(EntityTypeBuilder<Block> builder)
        {
            builder.Property(b => b.Height).IsRequired().ValueGeneratedNever();
            builder.Property(b => b.Hash).IsRequired();
            builder.Property(b => b.Time).IsRequired();
            builder.Property(b => b.Version).IsRequired();
            builder.Property(b => b.Target).IsRequired().HasConversion(Converters.TargetToInt32);
            builder.Property(b => b.Nonce).IsRequired();
            builder.Property(b => b.MerkleRoot).IsRequired();

            builder.HasKey(b => b.Height);
            builder.HasAlternateKey(b => b.Hash);
        }

        protected virtual void ConfigureBlockTransaction(EntityTypeBuilder<BlockTransaction> builder)
        {
            builder.Property(t => t.BlockHash).IsRequired();
            builder.Property(t => t.Index).IsRequired();
            builder.Property(t => t.TransactionHash).IsRequired();

            builder.HasKey(t => new { t.BlockHash, t.Index });
            builder.HasIndex(t => t.TransactionHash);
            builder.HasOne(t => t.Block)
                   .WithMany(b => b!.Transactions)
                   .HasForeignKey(t => t.BlockHash)
                   .HasPrincipalKey(b => b!.Hash)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(t => t.Transaction)
                   .WithMany(t => t!.Blocks)
                   .HasForeignKey(t => t.TransactionHash)
                   .HasPrincipalKey(t => t!.Hash)
                   .OnDelete(DeleteBehavior.Restrict);
        }

        protected virtual void ConfigureElysiumTransaction(EntityTypeBuilder<ElysiumTransaction> builder)
        {
            builder.Property(t => t.TransactionHash).IsRequired();
            builder.Property(t => t.Sender);
            builder.Property(t => t.Receiver);
            builder.Property(t => t.Serialized).IsRequired();

            builder.HasKey(t => t.TransactionHash);
            builder.HasOne<Transaction>()
                   .WithOne(t => t.Elysium!)
                   .HasForeignKey<ElysiumTransaction>(t => t.TransactionHash)
                   .HasPrincipalKey<Transaction>(t => t.Hash)
                   .OnDelete(DeleteBehavior.Cascade);
        }

        protected virtual void ConfigureInput(EntityTypeBuilder<Input> builder)
        {
            builder.Property(i => i.TransactionHash).IsRequired();
            builder.Property(i => i.Index).IsRequired();
            builder.Property(i => i.OutputHash).IsRequired();
            builder.Property(i => i.OutputIndex).IsRequired();
            builder.Property(i => i.Script).IsRequired().HasConversion(Converters.ScriptToBytesConverter);
            builder.Property(i => i.Sequence).IsRequired();

            builder.HasKey(i => new { i.TransactionHash, i.Index });
            builder.HasOne<Transaction>()
                   .WithMany(t => t.Inputs)
                   .HasForeignKey(i => i.TransactionHash)
                   .HasPrincipalKey(t => t.Hash)
                   .OnDelete(DeleteBehavior.Cascade);
        }

        protected virtual void ConfigureMtpData(EntityTypeBuilder<MtpData> builder)
        {
            builder.Property(m => m.BlockHash).IsRequired();
            builder.Property(m => m.Hash).IsRequired();
            builder.Property(m => m.Version).IsRequired();
            builder.Property(m => m.Reserved1).IsRequired();
            builder.Property(m => m.Reserved2).IsRequired();

            builder.HasKey(m => m.BlockHash);
            builder.HasOne<Block>()
                   .WithOne(b => b.MtpData!)
                   .HasForeignKey<MtpData>(t => t.BlockHash)
                   .HasPrincipalKey<Block>(b => b.Hash)
                   .OnDelete(DeleteBehavior.Cascade);
        }

        protected virtual void ConfigureOutput(EntityTypeBuilder<Output> builder)
        {
            builder.Property(o => o.TransactionHash).IsRequired();
            builder.Property(o => o.Index).IsRequired();
            builder.Property(o => o.Script).IsRequired().HasConversion(Converters.ScriptToBytesConverter);
            builder.Property(o => o.Value).IsRequired();

            builder.HasKey(o => new { o.TransactionHash, o.Index });
            builder.HasOne<Transaction>()
                   .WithMany(t => t.Outputs)
                   .HasForeignKey(o => o.TransactionHash)
                   .HasPrincipalKey(t => t.Hash)
                   .OnDelete(DeleteBehavior.Cascade);
        }

        protected virtual void ConfigureTransaction(EntityTypeBuilder<Transaction> builder)
        {
            builder.Property(t => t.Hash).IsRequired();
            builder.Property(t => t.Version).IsRequired();
            builder.Property(t => t.LockTime).IsRequired();
            builder.Property(t => t.Extra);

            builder.HasKey(t => t.Hash);
        }

        protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Block>(this.ConfigureBlock);
            modelBuilder.Entity<BlockTransaction>(this.ConfigureBlockTransaction);
            modelBuilder.Entity<ElysiumTransaction>(this.ConfigureElysiumTransaction);
            modelBuilder.Entity<Input>(this.ConfigureInput);
            modelBuilder.Entity<MtpData>(this.ConfigureMtpData);
            modelBuilder.Entity<Output>(this.ConfigureOutput);
            modelBuilder.Entity<Transaction>(this.ConfigureTransaction);
        }
    }
}
