using System.Collections.Specialized;

namespace POS.Contracts.Services;

public interface IAppNotificationService
{
    void Initialize();

    bool Show(string payload);

    NameValueCollection ParseArguments(string arguments);

    void Unregister();
    void OnNotificationInvoked(string Log,string Title);
}
