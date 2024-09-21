using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyGameStore.ViewModel;

public class FilteredGP_VM(SelectList genres, string selectedGenreGameProduct,string selectedTitleGP)
{
	public SelectList Genres {  get; set; } = genres;
	public string SelectedGenreGameProduct { get; set; } = selectedGenreGameProduct;
	public string SelectedTitleGP { get; set; } = selectedTitleGP;
}
