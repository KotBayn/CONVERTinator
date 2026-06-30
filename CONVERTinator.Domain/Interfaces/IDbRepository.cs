using CONVERTinator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CONVERTinator.Domain.Interfaces
{
    public interface IDbRepository
    {
        Task<List<Currency>> GetCachedRatesAsync();
        Task<bool> IsCacheFreshAsync(TimeSpan maxAge);
        Task SaveRatesAsync(List<Currency> rates);
    }
}
