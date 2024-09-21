namespace MyGameStore.ViewModel;

public class PageViewModel(int count, int pageNumber, int pageSize)
{
	public int PageNumber => pageNumber;
	public int TotalPages => (int)Math.Ceiling(count / (double)pageSize);
	public bool HasPreviousPage => PageNumber > 1;
	public bool HasNextPage => PageNumber < TotalPages;
}
