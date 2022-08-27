using InBranchNotification.Domain;
using InBranchNotification.Queries.ADLogin.queries;
using System.Threading.Tasks;

namespace InBranchNotification.Services
{
    public interface IAuthenticateRestClient
    {
        bool CheckXtradotAdUser(LoginWithAdQuery loginWithAdQuery);
       Task< XtradotAdUserdetails> GetXtradotAdUserDetails(string username, string domain);
        FBNDomain XtradotGetDomains();
    }
}