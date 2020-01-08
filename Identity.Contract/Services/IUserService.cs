using Identity.Contract.Model;
using System;
using System.Threading.Tasks;

namespace Identity.Contract.Services
{
    public interface IUserService
    {
        Task<UserDto> GetByIdentity(Guid id);
    }
}
