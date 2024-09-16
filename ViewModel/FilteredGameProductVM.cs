using Microsoft.AspNetCore.Mvc.Rendering;
using MyGameStoreModel.Entities;

namespace MyGameStore.ViewModel;

public class FilteredGameProductVM
{
	public string? NameSearchString { get; set; }
	public string? GameGenre { get; set; }
	public required List <GameProduct> GameProducts { get; set; }
	public required SelectList GameGenres { get; set; }	
}
