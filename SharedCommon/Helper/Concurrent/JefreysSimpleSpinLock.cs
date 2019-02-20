namespace SharedCommon.Helper.Concurrent
{
    using System.Threading;

    public struct JefreysSimpleSpinLock
    {
        private int resourceInUse;

        public void Enter()
        {
            while (true)
            {
                if (Interlocked.Exchange(ref this.resourceInUse, 1) == 0)
                {
                    return;
                }
            }
        }

        public void Leave()
        {
            Volatile.Write(ref this.resourceInUse, 0);
        }
    }
}