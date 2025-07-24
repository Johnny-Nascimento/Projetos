using AutoMapper;
using Encurtador_De_Links.Data.Dto;
using Encurtador_De_Links.Models;

namespace Encurtador_De_Links.Profiles;

public class LinkProfile : Profile
{
    public LinkProfile()
    {
        CreateMap<CreateLinkDto, Link>();
    }
}
