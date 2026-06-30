using CONVERTinator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CONVERTinator.Domain.Interfaces
{
    public interface IExchangeRateProvider
    {
       Task<List<Currency>> GetRatesAsync();
    }
}
