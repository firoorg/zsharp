namespace Zsharp.Entity.Postgres
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;

    public sealed class DbContextOptionsExtension : IDbContextOptionsExtension
    {
        public DbContextOptionsExtension()
        {
            this.Info = new ExtensionInfo(this);
        }

        public DbContextOptionsExtensionInfo Info { get; }

        public void ApplyServices(IServiceCollection services)
        {
            services.AddEntityFrameworkZcoinPostgresPlugins();
        }

        public void Validate(IDbContextOptions options)
        {
        }

        sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            public override bool IsDatabaseProvider => false;

            public override string LogFragment => "using Zsharp ";

            public override long GetServiceProviderHashCode() => 0;

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                debugInfo["ZsharpPostgres:enabled"] = "1";
            }
        }
    }
}
