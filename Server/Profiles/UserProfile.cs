using AutoMapper;
using Dtos;
using Models;

namespace Profiles {
    public class UserProfile : Profile{
        public UserProfile()
        {
            CreateMap<Users, UsersDto>();
            CreateMap<Team, TeamDto>();
            CreateMap<Sport, SportDto>();
            CreateMap<Players, PlayersDto>();
        }
    }
}