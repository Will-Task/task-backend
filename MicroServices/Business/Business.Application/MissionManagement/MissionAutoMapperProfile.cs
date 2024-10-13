using AutoMapper;
using Business.MissionManagement.Dto;
using Business.Models;
namespace Business.MissionManagement;

public class MissionAutoMapperProfile: Profile
{
    public  MissionAutoMapperProfile()
    {
        CreateMap<CreateOrUpdateMissionDto,MissionI18NDto>();
        CreateMap<CreateOrUpdateMissionDto,MissionI18N>();
        CreateMap<CreateOrUpdateMissionDto,Mission>();
        // use
        CreateMap<MissionI18N,MissionI18NDto>();
        // use
        CreateMap<Mission, MissionDto>();
        CreateMap<MissionView, MissionViewDto>();

        CreateMap<MissionImportDto, Mission>();
        CreateMap<MissionView, MissionExportDto>();
        CreateMap<MissionViewDto, MissionViewDto>();
        CreateMap<MissionView, MissionView>();
    }
}