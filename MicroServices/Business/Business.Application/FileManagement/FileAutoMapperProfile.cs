using AutoMapper;
using Business.FileManagement.Dto;
using Business.Models;
using FileInfo = System.IO.FileInfo;

namespace Business.FileManagement;

public class FileAutoMapperProfile : Profile
{
    public FileAutoMapperProfile()
    {
        CreateMap<FileInfo, FileInfoDto>();
    }
}