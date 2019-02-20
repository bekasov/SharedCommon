namespace SharedCommon.UnitTests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using SharedCommon.Helper;
    using Xunit;

    public class LoadBalancerTests
    {
        private static readonly TimeSpan DELAY = TimeSpan.FromMilliseconds(500);
        
        [Fact]
        public async Task DelaysWithTheSameHosts_CorrectParams_ItMustDelayCorrectly()
        {
            const string HOST = "host.localhost";
            
            LoadBalancer balancer = new LoadBalancer(DELAY);
            
            Stopwatch watch1 = new Stopwatch();
            Stopwatch watch2 = new Stopwatch();
            Stopwatch watch3 = new Stopwatch();

            Task task1 = Task.Run(async () =>
            {
                watch1.Start();
                await balancer.PauseIfNecessary(HOST, CancellationToken.None);
                watch1.Stop();
            });
            
            Task task2 = Task.Run(async () =>
            {
                watch2.Start();
                await balancer.PauseIfNecessary(HOST, CancellationToken.None);
                watch2.Stop();
            });
            
            Task task3 = Task.Run(async () =>
            {
                watch3.Start();
                await balancer.PauseIfNecessary(HOST, CancellationToken.None);
                watch3.Stop();
            });

            await Task.WhenAll(task1, task2, task3);

            var w1 = watch1.ElapsedMilliseconds;
            var w2 = watch2.ElapsedMilliseconds;
            var w3 = watch3.ElapsedMilliseconds;
            
            var elapseds = new long[] { w1, w2, w3 };
            var min = elapseds.Min();
            var max = elapseds.Max();
            var middle = elapseds.First(v => v != min && v != max);
            
            Assert.True(middle - min >= 500 * 0.95);
            Assert.True(max - middle >= 500 * 0.95);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(200)]
        [InlineData(300)]
        [InlineData(400)]
        [InlineData(500)]
        public async Task DelaysWithTheSameHosts_TwoRequestsInShortTime_ItMustDelayCorrectly(int delayInMs)
        {
            const string HOST = "host.localhost";

            LoadBalancer balancer = new LoadBalancer(DELAY);

            Stopwatch watch1 = new Stopwatch();

            await balancer.PauseIfNecessary(HOST, CancellationToken.None);

            await Task.Delay(delayInMs);
            
            watch1.Start();

            await balancer.PauseIfNecessary(HOST, CancellationToken.None);

            var w1 = watch1.ElapsedMilliseconds;

            var expectedResult = 500 - delayInMs;
            var delta = expectedResult * 0.9; 
            
            Assert.True(w1 <= expectedResult);
            Assert.True(w1 >= delta);
        }
        
        [Fact]
        public async Task DelaysWithTheSameHosts_ParallelRequestsInShortTime_ItMustDelayCorrectly()
        {
            const string HOST = "host.localhost";

            LoadBalancer balancer = new LoadBalancer(DELAY);

            Stopwatch watch1 = new Stopwatch();
            Stopwatch watch2 = new Stopwatch();

            await balancer.PauseIfNecessary(HOST, CancellationToken.None);
            
            await Task.Delay(200);
            
            Task task1 = Task.Run(async () =>
            {
                watch1.Start();
                await balancer.PauseIfNecessary(HOST, CancellationToken.None);
                watch1.Stop();
            });

            await Task.Delay(200);
            
            Task task2 = Task.Run(async () =>
            {
                watch2.Start();
                await balancer.PauseIfNecessary(HOST, CancellationToken.None);
                watch2.Stop();
            });

            await Task.WhenAll(task1, task2);

            var w1 = watch1.ElapsedMilliseconds;
            var w2 = watch2.ElapsedMilliseconds;

            var expectedResult = 500 - 200;
            var delta = expectedResult * 0.9; 
        }
    }
}