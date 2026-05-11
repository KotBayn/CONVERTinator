namespace CONVERTinator.Domain
{
    public class Currency
    {
        public string Code { get; set; } = string.Empty; // "USD"
        public string Name { get; set; } = string.Empty; // "US Dollar"
        public decimal Value { get; set; }               // Price
        public string Source { get; set; } = string.Empty; // Source of the information
    }
}
