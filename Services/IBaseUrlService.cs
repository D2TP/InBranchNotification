using System.Threading.Tasks;

namespace InBranchDashboard.Services
{
    public interface IBaseUrlService
    {
        Task<string> BaseUrlLink();
        Task<string> BaseUrlLinkForActiveDirectory();
    }
}