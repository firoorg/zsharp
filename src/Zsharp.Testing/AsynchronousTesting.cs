namespace Zsharp.Testing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class AsynchronousTesting
    {
        public static void WithCancellationToken(Action<CancellationToken> test)
        {
            using (var source = new CancellationTokenSource())
            {
                test(source.Token);
            }
        }

        public static async Task WithCancellationTokenAsync(Func<CancellationToken, ValueTask> test)
        {
            using (var source = new CancellationTokenSource())
            {
                await test(source.Token);
            }
        }

        public static async Task WithCancellationTokenAsync(
            Func<CancellationToken, Action, ValueTask> test,
            Action<CancellationTokenSource> cancel)
        {
            using (var source = new CancellationTokenSource())
            {
                await test(source.Token, () => cancel(source));
            }
        }

        public static async Task WithCancellationTokenAsync(
            Func<CancellationToken, Func<ValueTask>, ValueTask> test,
            Func<CancellationTokenSource, ValueTask> cancel)
        {
            using (var source = new CancellationTokenSource())
            {
                await test(source.Token, () => cancel(source));
            }
        }
    }
}
