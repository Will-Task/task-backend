using AutoMapper;
using Business.FileManagement.Dto;
using Business.Models;

namespace Business.FileManagement;

public class FileAutoMapperProfile : Profile
{
    public FileAutoMapperProfile()
    {
        CreateMap<MyFileInfo, MyFileInfoDto>();
        CreateMap<ImportMyFileDto, MyFileInfo>();
    }
}