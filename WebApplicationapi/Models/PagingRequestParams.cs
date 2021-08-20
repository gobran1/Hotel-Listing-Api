namespace WebApplicationapi.Models
{
  public class PagingRequestParams
  {
    private const int _MaxPageSize = 30;
    private int _pageSize;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
      get => _pageSize;
      set => _pageSize = (value > _MaxPageSize) ? _MaxPageSize : value;
    }
  }
}
