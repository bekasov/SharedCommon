namespace SharedCommon.Helper
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ILoadBalancer
    {
        Task PauseIfNecessary(string host, CancellationToken token);
    }
}