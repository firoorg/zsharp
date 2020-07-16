namespace Zsharp.LightweightIndexer
{
    using AsyncEvent;

    public interface IBlockSynchronizer
    {
        event AsyncEventHandler<BlockEventArgs>? BlockAdded;

        event AsyncEventHandler<BlockEventArgs>? BlockRemoving;
    }
}
