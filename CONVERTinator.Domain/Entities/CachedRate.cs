using System;

namespace CONVERTinator.Domain.Entities
{
    public class CachedRate
    {
        public int Id { get; set; } // Primary Key 
        public string CurrencyCode { get; set; } = null!;

        // Important!!: Use decimal for currency rates to avoid floating-point precision issues.
        public decimal UsdRate { get; set; }

        public DateTime FetchTime { get; set; } 
        public string Source { get; set; } = null!;
    }
}