using InBranchNotification.Domain;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace InBranchNotification.Services
{
    public interface IBaseUrlService
    {
        Task<string> BaseUrlLink();
        Task<string> BaseUrlLinkForActiveDirectory();
        Task<ObjectResponse> AddAuditItem(Audit audit, StringValues userAgent)
    }
}