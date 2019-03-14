namespace SharedCommon.Helper
{
    using System;
    using System.Linq;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Lamar;
    using SharedCommon.Helper.Concurrent;
    using SharedCommon.Logger;

    public class LoadBalancer : ILoadBalancer
    {
        private static readonly ConcurrentDictionary<string, HostInfo> hostsInfos= new ConcurrentDictionary<string, HostInfo>(); 
        
        private readonly TimeSpan delaySetting;

        public LoadBalancer(TimeSpan delaySetting)
        {
            this.delaySetting = delaySetting;
        }

        public async Task PauseIfNecessary(string host, CancellationToken token)
        {
            DateTime enterTime = DateTime.Now;
            string firstLevelHost = string.Join('.', host.Split('.').Reverse().Take(2).Reverse());

            HostInfo currentHost = hostsInfos.AddOrUpdate(
                firstLevelHost, 
                new HostInfo(this.delaySetting, this.Logger), 
                (s, info) => info.GetNext()
            );

            await currentHost.Delay(enterTime, token).ConfigureAwait(false);
        }
        
        [SetterProperty]
        public ILogger Logger { private get; set; }
        
        private class HostInfo
        {
            private readonly ILogger logger;
            
            private readonly TimeSpan delaySetting;
            private long count;
            private DateTime lastCall;
            private JefreysSimpleSpinLock @lock = new JefreysSimpleSpinLock();

            public HostInfo(TimeSpan delaySetting, ILogger logger)
            {
                this.logger = logger;
                this.delaySetting = delaySetting;
                this.count = 1;
                this.lastCall = default;
            }

            public long Count => Interlocked.Read(ref this.count);
            
            public async Task Delay(DateTime enterTime, CancellationToken token)
            {
                this.@lock.Enter();

                long localCount = this.Count;

                TimeSpan delay;

                if (localCount == 1)
                {
                    delay = enterTime - this.lastCall;
                    if (delay > this.delaySetting)
                    {
                        delay = TimeSpan.Zero;
                    }
                    else
                    {
                        delay = this.delaySetting - delay;
                    }
                }
                else
                {
                    delay = this.delaySetting * localCount;
                }
                
                this.lastCall = DateTime.Now + delay;
                
                this.@lock.Leave();
                //this.logger?.Log($"Delay: { delay.Milliseconds }");
                await Task.Delay(delay, token).ConfigureAwait(false);
                
                Interlocked.Decrement(ref this.count);
            }

            public HostInfo GetNext()
            {
                HostInfo result = new HostInfo(this.delaySetting, this.logger);
                this.@lock.Enter();

                result.count = this.Count + 1;
                result.lastCall = this.lastCall;
                
                this.@lock.Leave();

                return result;
            }
        }
    }
}