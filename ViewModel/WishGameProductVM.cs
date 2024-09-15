using MyGameStoreModel.Entities;

namespace MyGameStore.ViewModel;

public class WishGameProductVM
{
    public required GameProduct gameProduct { get; set; }

    public  bool  ContainsWishGameProducts { get; set; }
	public bool ContainsGameProducts { get;  set; }
}
