namespace SharedCommon.Extensions
{
    using System.Threading.Tasks;
    
    public interface IAsyncEnumerator<out TItem>
    {
        Task<bool> MoveNext();

        TItem Current { get; }

        void Reset();
    }
}