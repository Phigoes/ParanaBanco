using AutoMapper;
using ParanaBanco.Application.DTOs;
using ParanaBanco.Domain.Entities;

namespace ParanaBanco.Application.Mapping
{
    public class DTOToEntityMapping : Profile
    {
        public DTOToEntityMapping()
        {
            CreateMap<UserDTO, User>();
        }
    }
}
