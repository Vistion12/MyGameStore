using MyGameStore.Core;
using MyGameStoreModel.Entities;

namespace MyGameStore.ViewModel;

public class GameProductsVM
{
	public required PaginationList<GameProduct> GameProducts { get; set; }
	public required FilteredGP_VM FilteredGP_VM { get; set; }
	public required SortGPVM SortGPVM { get; set; }	
}
