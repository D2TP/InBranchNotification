using System.Threading.Tasks;

namespace InBranchNotification.Services
{
    public interface IBaseUrlService
    {
        Task<string> BaseUrlLink();
        Task<string> BaseUrlLinkForActiveDirectory();
    }
}