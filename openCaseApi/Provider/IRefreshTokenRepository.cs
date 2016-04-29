
using System.Threading.Tasks;

namespace openCaseApi.Provider
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> Get(string refreshToken);
        Task<bool> Save(RefreshToken refreshToken);
        Task<bool> Remove(string refreshToken);
    }
}