using AutoMapper;
using ParanaBanco.Application.DTOs;
using ParanaBanco.Domain.Entities;

namespace ParanaBanco.Application.Mapping
{
    public class EntityToDTOMapping : Profile
    {
        public EntityToDTOMapping()
        {
            CreateMap<User, UserDTO>();
        }
    }
}
