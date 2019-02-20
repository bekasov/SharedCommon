namespace SharedCommon.UnitTests
{
    using System.Collections.Generic;
    using SharedCommon.Extensions;
    using SharedCommon.Helper;
    using Xunit;
    
    public class CollectionStorageTests
    {
        [Fact]
        public void Enumeration_CorrectParams_ItMustEnumerateAllCombinations()
        {
            IList<int> numbers = new List<int> { 33, 23, 42, 1 };
            
            string[] districts = new string[] { "KG", "FR" };
                       
            CollectionsEnumerator<string, int> districtsCollection = new CollectionsEnumerator<string, int>(districts, numbers.GetEnumerator());
            
            string[] pageSet = new string[] { "1", "2", "3" };
            
            CollectionsEnumerator<string, string> nestedCollection = new CollectionsEnumerator<string, string>(pageSet, districtsCollection);

            IList<string> result = new List<string>(); 
            
            foreach (string page in nestedCollection)
            {
                string district = nestedCollection.SlowCollectionCurrentItem;

                int num = (nestedCollection.SlowCollection as CollectionsEnumerator<string, int>).SlowCollectionCurrentItem;
                
                result.Add($"District: { district }, page: { page }, num: { num.ToString() }");
            }
            
            Assert.Equal(numbers.Count * pageSet.Length * districts.Length, result.Count);
        }
    }
}