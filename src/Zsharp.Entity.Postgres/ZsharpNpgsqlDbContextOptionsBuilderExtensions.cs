namespace Microsoft.EntityFrameworkCore
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
    using Zsharp.Entity.Postgres;

    public static class ZsharpNpgsqlDbContextOptionsBuilderExtensions
    {
        public static NpgsqlDbContextOptionsBuilder UseZcoin(this NpgsqlDbContextOptionsBuilder builder)
        {
            RegisterEntityFrameworkInterceptors(builder);
            RegisterEntityFrameworkExtensions(builder);

            return builder;
        }

        static void RegisterEntityFrameworkInterceptors(IRelationalDbContextOptionsBuilderInfrastructure builder)
        {
            builder.OptionsBuilder.AddInterceptors(new DbConnectionInterceptor());
        }

        static void RegisterEntityFrameworkExtensions(IRelationalDbContextOptionsBuilderInfrastructure builder)
        {
            var extension = builder.OptionsBuilder.Options.FindExtension<DbContextOptionsExtension>();

            if (extension == null)
            {
                extension = new DbContextOptionsExtension();
            }

            ((IDbContextOptionsBuilderInfrastructure)builder.OptionsBuilder).AddOrUpdateExtension(extension);
        }
    }
}
