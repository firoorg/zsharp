namespace Zsharp.LightweightIndexer.Postgres
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    public sealed class DbContextOptions
    {
        [Required]
        [NotNull]
        public string? ConnectionString { get; set; }
    }
}
