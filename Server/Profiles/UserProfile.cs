using AutoMapper;
using Models;

namespace Profiles {
    public class UserProfile : Profile{
        public UserProfile()
        {
            CreateMap<Users, UsersDto>();
        }
    }
}