namespace Zsharp.Rpc.Client
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using NBitcoin.RPC;

    public sealed class RpcClientOptions
    {
        [Required]
        [NotNull]
        public RPCCredentialString? Credential { get; set; }

        [Required]
        [NotNull]
        public Uri? ServerUrl { get; set; }
    }
}
