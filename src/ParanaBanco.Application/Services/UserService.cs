using AutoMapper;
using ParanaBanco.Application.DTOs;
using ParanaBanco.Application.Interfaces.Services;
using ParanaBanco.Domain.Entities;
using ParanaBanco.Domain.Exceptions;
using ParanaBanco.Domain.Interfaces.Repositories;
using ParanaBanco.Domain.ValueObjects;

namespace ParanaBanco.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<int> Add(UserDTO userDTO)
        {
            var user = await _userRepository.GetByEmail(userDTO.Email.ToLowerInvariant());

            if (user != null)
                throw new EmailAlreadyRegisteredException();

            return await _userRepository.Add(_mapper.Map<User>(userDTO));
        }

        public async Task Update(UserDTO userDTO)
        {
            var user = await _userRepository.GetById(userDTO.Id, false);
            if (user == null)
                throw new UserNotFoundException();

            var userByEmail = await _userRepository.GetByEmail(userDTO.Email.ToLowerInvariant());
            if (userByEmail != null && userDTO.FullName == userByEmail.FullName)
                throw new EmailAlreadyRegisteredException();

            if (userDTO.FullName == user.FullName && user.Email.Address == userDTO.Email.ToLowerInvariant())
                return;

            user.ModifyUser(userDTO.FullName, new Email(userDTO.Email));

            await _userRepository.Update(user);
        }

        public async Task Delete(UserDTO userDTO)
        {
            var user = await _userRepository.GetById(userDTO.Id, false);
            user.Delete();

            await _userRepository.Update(user);
        }

        public async Task Restore(UserDTO userDTO)
        {
            var user = await _userRepository.GetById(userDTO.Id, true);
            if (user  == null)
                throw new UserNotFoundException();

            user.Restore();

            await _userRepository.Update(user);
        }

        public async Task<IEnumerable<UserDTO>> GetAll(bool includeDeleted)
        {
            return _mapper.Map<IEnumerable<UserDTO>>(await _userRepository.GetAll(includeDeleted));
        }

        public async Task<UserDTO?> GetById(int id, bool includeDeleted)
        {
            return _mapper.Map<UserDTO>(await _userRepository.GetById(id, includeDeleted));
        }

        public async Task<UserDTO?> GetByEmail(string email)
        {
            return _mapper.Map<UserDTO>(await _userRepository.GetByEmail(email));
        }

        public async Task<IEnumerable<UserDTO>> GetOnlyDeleted()
        {
            return _mapper.Map<IEnumerable<UserDTO>>(await _userRepository.GetOnlyDeleted());
        }
    }
}
