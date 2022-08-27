using InBranchNotification.DTOs;
using System.Threading.Tasks;

namespace InBranchNotification.Services
{
    public interface ITokenService
    {
        Task<string> GetToken(ADUserDTORespons query);
    }
}