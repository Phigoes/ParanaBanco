using ParanaBanco.Application.DTOs;

namespace ParanaBanco.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<int> Add(UserDTO userDTO);
        Task Delete(UserDTO userDTO);
        Task Update(UserDTO userDTO);
        Task Restore(UserDTO userDTO);
        Task<UserDTO?> GetById(int id, bool includeDeleted);
        Task<IEnumerable<UserDTO>> GetAll(bool includeDeleted);
        Task<UserDTO?> GetByEmail(string email);
        Task<IEnumerable<UserDTO>> GetOnlyDeleted();
    }
}
