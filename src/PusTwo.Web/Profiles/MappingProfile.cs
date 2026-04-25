using AutoMapper;
using PusTwo.Application.DTOs.Syspro;
using PusTwo.Web.ViewModels;

namespace PusTwo.Web.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
    
        CreateMap<BomOperationDto, BomViewModel>();
        CreateMap<BomViewModel, BomOperationDto>();

        
    }
}