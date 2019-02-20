namespace SharedCommon.UnitTests
{
    using System;
    using System.Linq;
    using SharedCommon.Helper;
    using Xunit;
    
    public class PageEnumeratorTests
    {
        [Fact]
        public void Enumeration_OnePageAmount_ItMustEnumerateCorrectly()
        {
            PageEnumerator<long> pageEnumerator = new PageEnumerator<long>(0, l => l + 1);

            foreach (long page in pageEnumerator)
            {
                Assert.Equal(1, page);                
            }
        }
        
        [Fact]
        public void Enumeration_MoreThanOnePageAmount_ItMustEnumerateCorrectly()
        {
            int expectedPages = 5;
            int nextNumberGetterCalls = 0;
            Func<int, int> nextNumberGetter = i =>
            {
                nextNumberGetterCalls++;
                return i + 1;
            };
            PageEnumerator<int> pageEnumerator = new PageEnumerator<int>(0, nextNumberGetter);
            pageEnumerator.SetLastPage(expectedPages);
            
            Assert.Equal(expectedPages, pageEnumerator.Count());
            int expectedCurrentPage = 1;
            foreach (long page in pageEnumerator)
            {
                Assert.Equal(expectedCurrentPage++, page);                
            }
            Assert.Equal(1 + expectedPages * 2, nextNumberGetterCalls);
        }
    }
}