using InBranchDashboard.Domain;
using InBranchDashboard.Queries.ADLogin.queries;

namespace InBranchDashboard.Services
{
    public interface IAuthenticateRestClient
    {
        bool CheckXtradotAdUser(LoginWithAdQuery loginWithAdQuery);
        XtradotAdUserdetails GetXtradotAdUserDetails(string username, string domain);
        FBNDomain XtradotGetDomains();
    }
}