using MyGameStoreModel.Entities;

namespace MyGameStore.ViewModel;

public class CartViewModel
{
    public required IEnumerable<GameProduct> gameProducts { get; init; }
    public required decimal  SumgameProducts { get; init; }
}
