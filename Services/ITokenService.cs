using InBranchDashboard.DTOs;
using System.Threading.Tasks;

namespace InBranchDashboard.Services
{
    public interface ITokenService
    {
        Task<string> GetToken(ADUserDTORespons query);
    }
}