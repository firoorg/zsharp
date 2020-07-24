namespace Zsharp.LightweightIndexer.Postgres
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Zsharp.LightweightIndexer.Entity;

    public sealed class DbContext : Zsharp.LightweightIndexer.Entity.DbContext
    {
        internal DbContext(DbContextOptions<Zsharp.LightweightIndexer.Entity.DbContext> options)
            : base(options)
        {
        }

        protected override void ConfigureBlock(EntityTypeBuilder<Block> builder)
        {
            base.ConfigureBlock(builder);

            builder.Property(b => b.Target).HasConversion(Converters.TargetToInt32);
        }

        protected override void ConfigureInput(EntityTypeBuilder<Input> builder)
        {
            base.ConfigureInput(builder);

            builder.Property(i => i.Script).HasConversion(Converters.ScriptToBytesConverter);
        }

        protected override void ConfigureOutput(EntityTypeBuilder<Output> builder)
        {
            base.ConfigureOutput(builder);

            builder.Property(o => o.Script).HasConversion(Converters.ScriptToBytesConverter);
        }
    }
}
