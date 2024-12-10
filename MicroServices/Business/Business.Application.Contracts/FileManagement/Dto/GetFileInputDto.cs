using Volo.Abp.Application.Dtos;

namespace Business.FileManagement.Dto;

public class GetFileInputDto: PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}