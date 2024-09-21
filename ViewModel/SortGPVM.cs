using MyGameStore.Core;

namespace MyGameStore.ViewModel;

public class SortGPVM(SortGameProductState? sortTitleGameProduct)
{
	public SortGameProductState? SortTitleGP => sortTitleGameProduct;
}
