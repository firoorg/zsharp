namespace Zsharp.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class CancellationTokenExtenions
    {
        public static Task WaitAsync(this CancellationToken target, CancellationToken cancellationToken = default)
        {
            if (!target.CanBeCanceled)
            {
                throw new InvalidOperationException("The token cannot be canceled.");
            }

            var completionSource = new TaskCompletionSource<bool>();

            target.Register(() =>
            {
                try
                {
                    completionSource.SetResult(true);
                }
                catch (InvalidOperationException)
                {
                    // Ignore.
                }
            });

            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() =>
                {
                    try
                    {
                        completionSource.SetCanceled();
                    }
                    catch (InvalidOperationException)
                    {
                        // Ignore.
                    }
                });
            }

            return completionSource.Task;
        }
    }
}
