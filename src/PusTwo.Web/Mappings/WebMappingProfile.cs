using AutoMapper;
using PusTwo.Application.DownTime.DTOs;
using PusTwo.Domain.Entities;
using PusTwo.Web.ViewModels;

namespace PusTwo.Web.Mappings
{
    public class WebMappingProfile : Profile
    {
        public WebMappingProfile()
        {
            // ViewModel → Command
            CreateMap<DownTimeBatchLineViewModel, CreateDownTimeCommand>()
                .ForMember(dest => dest.GroupCode, opt => opt.MapFrom(src => src.GroupCode))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.DowntimeMinutes, opt => opt.MapFrom(src => src.DowntimeMinutes))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.WorkCentre, opt => opt.Ignore())
                .ForMember(dest => dest.Machine, opt => opt.Ignore())
                .ForMember(dest => dest.JobNumber, opt => opt.Ignore())
                .ForMember(dest => dest.StockCode, opt => opt.Ignore())
                .ForMember(dest => dest.EntryDate, opt => opt.Ignore())
                .ForMember(dest => dest.Shift, opt => opt.Ignore());

            CreateMap<DownTimeBatchFormViewModel, CreateDownTimeBatchCommand>()
                .ForMember(dest => dest.Entries, opt => opt.MapFrom(src => src.Entries));

            // Command → Entity
            CreateMap<CreateDownTimeCommand, DownTime>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.IsDeleted, opt => opt.Ignore())
                .ForMember(d => d.DeletedAt, opt => opt.Ignore());

            // Entity → Result
            CreateMap<DownTime, DownTimeResult>();
        }
    }
}