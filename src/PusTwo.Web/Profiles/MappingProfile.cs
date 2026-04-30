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

        CreateMap<MachineLookupDto, MachineViewModel>();
        CreateMap<MachineViewModel, MachineLookupDto>();

        CreateMap<JobLookupDto, JobLookupViewModel>();
        CreateMap<JobLookupViewModel, JobLookupDto>();

        CreateMap<NonProdGroupDto, NonProdGroupViewModel>();
        CreateMap<NonProdGroupViewModel, NonProdGroupDto>();

        CreateMap<DowntimeRecordDto, DowntimeRecordViewModel>();
        CreateMap<DowntimeRecordViewModel, DowntimeRecordDto>();
    }
}