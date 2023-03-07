using AutoMapper;
using ParanaBanco.Application.DTOs;
using ParanaBanco.Domain.Entities;

namespace ParanaBanco.UnitTests.Helper
{
    public class TestMapper : Profile
    {
        public static IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>().ReverseMap();
            });

            return config.CreateMapper();
        }
    }
}
