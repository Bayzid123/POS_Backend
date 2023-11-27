using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.HttpsClient;
public interface IConnectionCheck
{
    Task<bool> IsInternetAvailable();
    bool IsInternetAvailableForPreAsync();
    Task<bool> IsServerConnectionAvailable();
    bool IsServerConnectionForLocal();
}
