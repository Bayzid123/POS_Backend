using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using POS.Core.Helpers;
using POS.Core.Models;
using POS.Core.ViewModels;
using POS.Services;
using POS.Services.HttpsClient;

namespace POS.ViewModels;

public class ChangePasswordViewModel : ObservableRecipient
{
    private readonly IGetService _iGetService;
    private readonly IConnectionCheck _connectionCheck;
    private readonly IMasterDataSQLRestService _masterDataSQLRestService;
    public ChangePasswordViewModel(IGetService iGetService, IConnectionCheck connectionCheck, IMasterDataSQLRestService masterDataSQLRestService)
    {
        _iGetService = iGetService;
        _connectionCheck = connectionCheck;
        _masterDataSQLRestService = masterDataSQLRestService;
    }
    public async Task<TblUser> GetUser()
    {
        try
        {
            DateTime currentDate = DateTime.Now.BD();

            var user = await _iGetService.GetUser(AppSettings.UserId);
            return user;
        }
        catch (Exception)
        {

            throw;
        }
    }
    public async Task<MessageHelper> ChangePassword(ChangePassword change)
    {
        try
        {
            MessageHelper msg = new MessageHelper();
            var connection = await _connectionCheck.IsServerConnectionAvailable();
            if (connection == false)
            {
                msg.Message = "Online Server Not Available. Can't change Password now !";
                msg.StatusCode = 500;
                return msg;
            }
            else
            {
                var user =  await _iGetService.GetUserAutorization(change.UserName,change.Password);
                if (user == null)
                {
                    msg.Message = "Old Password is Incorrect !";
                    msg.StatusCode = 500;
                    return msg;
                }
                var dt =await _masterDataSQLRestService.ChangePassword(change);
                msg.Message = dt.Message;
                msg.StatusCode = dt.StatusCode;
            }
            return msg;
        }
        catch (Exception)
        {

            throw;
        }
    }
}
