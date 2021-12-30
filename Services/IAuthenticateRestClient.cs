using InBranchDashboard.Domain;
using InBranchDashboard.Queries.ADLogin.queries;
using System.Threading.Tasks;

namespace InBranchDashboard.Services
{
    public interface IAuthenticateRestClient
    {
        bool CheckXtradotAdUser(LoginWithAdQuery loginWithAdQuery);
       Task< XtradotAdUserdetails> GetXtradotAdUserDetails(string username, string domain);
        FBNDomain XtradotGetDomains();
    }
}