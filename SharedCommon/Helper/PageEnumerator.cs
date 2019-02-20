namespace SharedCommon.Helper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    
    public class PageEnumerator<TNumber> : IEnumerator<TNumber>, IEnumerable<TNumber> 
            // where TNumber: IComparable
    {
        private readonly TNumber prependFirstPageValue;
        private readonly Func<TNumber, TNumber> nextNumberGetter;
        
        private TNumber currentPage;
        private TNumber lastPage;
        private bool customLastPage;
        
        public PageEnumerator(TNumber prependFirstPageValue, Func<TNumber, TNumber> nextNumberGetter)
        {
            this.prependFirstPageValue = prependFirstPageValue;
            this.nextNumberGetter = nextNumberGetter;
            this.Reset();
        }
        
        public bool MoveNext()
        {
            var comparer = Comparer<TNumber>.Default;
            int currentAndLastCompare = comparer.Compare(this.currentPage,this.lastPage);
            bool result = currentAndLastCompare < 0;
            if (result)
            {
                this.currentPage = this.nextNumberGetter(this.currentPage);
            }
            else
            {
                this.Reset();
            }

            return result;
        }

        public void SetLastPage(TNumber lastPage)
        {
            this.lastPage = lastPage;
            this.customLastPage = true;
        }
        
        public void Reset()
        {
            this.currentPage = this.prependFirstPageValue;
            if (!this.customLastPage)
            {
                this.lastPage =  this.nextNumberGetter(this.prependFirstPageValue);                
            }
        }

        public TNumber Current => this.currentPage;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public IEnumerator<TNumber> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}