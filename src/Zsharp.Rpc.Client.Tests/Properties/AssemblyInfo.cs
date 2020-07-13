using Xunit;

// There is a race condition when spawning zcoind in parallel.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
