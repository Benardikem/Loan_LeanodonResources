using Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;

namespace Web.Interfaces
{
    interface IAppManager
    {
        ActionResponse IsLogin(string strUser, string strPwd);
        Task<ActionResponse> ChangePasswordAsync(string username, string oldPassword, string newPassword);
        ActionResponse IsSignUp(string strDisplayName, string strEmailAddress, string StrPhoneNumber, string StrResidentialAddress, string StrZipCode, string StrPswd, string SecurityQ, string SecurityA);

        //Task<ActionResponse> VerifyAccount(string username);        
        //ActionResponse IsSignUp(string strDisplayName, string strEmailAddress, string StrPhoneNumber, string StrCountry,
        //    string StrProvince, string StrResidentialAddress, string StrZipCode, string StrPswd, string EmailClient);

        //ActionResponse ResetPass(string strUserID);
        //DataTable GetReturnFileRecords(string path);
        //IList<TDetail> PageResults<TDetail>(IEnumerable<TDetail> results, SearchRequest request) where TDetail : SearchDetail;
        //IList<TDetail> SearchagentsResults<TDetail>(IEnumerable<TDetail> results, SearchRequest request) where TDetail : SearchDetail;
        //bool TryValidate(object @object, out List<ValidationResult> results);
        ////ReturnItem GetReturnItem(long Id);
        //IEnumerable<InvestorItem> FilterInvestorItems(IEnumerable<InvestorItem> details, string searchText);
        IEnumerable<UserItem> FetchUsersWithRight(string _OperationKey);
    }
}
