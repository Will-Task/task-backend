namespace Business.Models;

public class PageModel
{
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public bool AllData { get; set; }

    public int Count
    {
        get
        {
            return (Page - 1) * PageSize;
        }
    }
}