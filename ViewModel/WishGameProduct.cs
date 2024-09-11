using MyGameStoreModel.Entities;

namespace MyGameStore.ViewModel;

public class WishGameProduct
{
    public required GameProduct gameProduct { get; set; }

    public  bool  ContainsWishGameProducts { get; set; }
	public bool ContainsGameProducts { get;  set; }
}
