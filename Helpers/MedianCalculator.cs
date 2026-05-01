using System.Collections.Generic;
using System.Linq;

namespace CONVERTinator.Helpers
{
    public static class MedianCalculator
    {
        public static decimal Calculate(List<decimal> values)
        {
            if (values == null || values.Count == 0)
                return 0;

            // 1. sort the list of values
            // 2. find the middle index
            // 3. if the count is odd - return the middle value
            // 4. if the count is even - return the average of the two middle values
            var sortedValues = values.OrderBy(v => v).ToList();
                       
            int midIndex = sortedValues.Count / 2;
                        
            if (sortedValues.Count % 2 != 0)
            {
                return sortedValues[midIndex];
            }
                       
            return (sortedValues[midIndex] + sortedValues[midIndex - 1]) / 2m;
        }
    }
}