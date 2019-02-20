namespace SharedCommon.Helper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    
    public class CollectionsEnumerator<TInnerItem, TOuterCollection> : IEnumerator<TInnerItem>, IEnumerable<TInnerItem>
    {
        private readonly IEnumerator<TInnerItem> collection;

        public CollectionsEnumerator(IEnumerable<TInnerItem> collection, IEnumerator<TOuterCollection> slowCollection = null)
        {
            if (collection == null)
            {
                throw new ArgumentException(nameof(collection));
            }

            this.SlowCollection = slowCollection;
            this.SlowCollection?.MoveNext();
            this.collection = collection.GetEnumerator();
        }

        public IEnumerator<TOuterCollection> SlowCollection { get; }
        
        public TOuterCollection SlowCollectionCurrentItem {
            get
            {
                if (this.SlowCollection != null)
                {
                    return this.SlowCollection.Current;
                }
                
                return default;
            }
        }

        public bool NoInnerCollection => this.SlowCollection == null;
        
        public bool MoveNext()
        {
            if (!this.collection.MoveNext())
            {
                if (this.SlowCollection?.MoveNext() == false || this.SlowCollection == null)
                {
                    this.collection.Reset();
                    this.SlowCollection?.Reset();
                    return false;
                }

                this.collection.Reset();
                return this.collection.MoveNext();
            }

            return true;
        }

        public void Reset()
        {
            this.collection.Reset();
            this.SlowCollection?.Reset();
        }

        public TInnerItem Current => this.collection.Current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            this.collection.Dispose();
            this.SlowCollection?.Dispose();
        }

        public IEnumerator<TInnerItem> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}