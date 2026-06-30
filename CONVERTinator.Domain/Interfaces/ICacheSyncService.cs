using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CONVERTinator.Domain.Interfaces
{
    public interface ICacheSyncService
    {
        Task ForceUpdateAsync();
    }
}
