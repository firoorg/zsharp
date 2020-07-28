namespace Zsharp.LightweightIndexer
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    public sealed class LightweightIndexerOptions
    {
        [Required]
        [NotNull]
        public string? BlockPublisherAddress { get; set; }
    }
}
