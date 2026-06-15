namespace CONVERTinator.Domain.Entities
{
    public class UserSettings
    {
        public int Id { get; set; } // In the local DB, there is only one user, so the ID will always be 1

        public string BaseCurrency { get; set; } = "USD"; // Main currency for conversion, default is USD
        public string SavedCurrencies { get; set; } = "EUR,GBP";
    }
}