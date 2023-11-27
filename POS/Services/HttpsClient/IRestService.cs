
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Models;

namespace POS.Services;

public interface IRestService
{

    Task DeleteTodoItemAsync(string id);
    Task<AllDDl> GetAllDDL(string App_Url);
    Task<GetPartnerDTO> GetAllPartner();
    Task<AllDDl> GetAllForPreDDL(string App_Url);
    Task<MessageHelper> UserLogIn(LoginModel login);
    Task<bool> CheckUserPermission(long accountId, long branchId, long officeId, long warehouseId, long counterId, long userId);
    Task<bool> AuthorizeUser(LoginModel login);
    Task<OfficeDetails> GetOfficeDetails(string officeId);
    Task<bool> AdminUserLogin(LoginModel login);
    Task<TblUser> GetUser(string username, string password);
    Task<MessageHelper> CreateSetting(TblSettings settings);
}
