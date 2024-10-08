﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyGameStore.Core;
using MyGameStore.Extensions;
using MyGameStore.ViewModel;
using MyGameStoreModel.Data;
using MyGameStoreModel.Entities;
using MyGameStoreModel.Repositories.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace MyGameStore.Controllers;
public record NameSurname(string Name, string Surname);
public class HomeController(GameShopContext gameShopContext,
	IGameProductRepository gameProductRepository,
	IHttpContextAccessor httpContextAccessor) : Controller
{
	private const int pageSize = 6;
	private const int minValueMonth = -1;
	private const int countPopGames = 100;
	public async Task<IActionResult> Index(
		string selectedGenreGameProduct,
		string selectedTitleGamePoduct,
		SortGameProductState? sortGameProductState,
		int page = 1)
	{
		var gameProductGenre = from g in gameShopContext.Genres
							   select g.Title;

		var gameProducts = from g in gameShopContext.GameProducts
						   select g;

		if (!string.IsNullOrEmpty(selectedTitleGamePoduct))
		{
			gameProducts = gameProducts
				.Where(gameProduct => gameProduct.Title.ToUpper().Contains(selectedTitleGamePoduct.ToUpper()));
		}

		if (!string.IsNullOrEmpty(selectedGenreGameProduct))
		{
			gameProducts = gameProducts
				.Include(gameProduct => gameProduct.Genre)
				.Where(gameProduct =>
							gameProduct.Genre.Any(genre => genre.Title.Contains(selectedGenreGameProduct)));
		}

		gameProducts = sortGameProductState switch
		{
			SortGameProductState.TitleAsc => gameProducts.OrderBy(gameProduct => gameProduct.Title),
			SortGameProductState.TitleDesc => gameProducts.OrderByDescending(gameProduct => gameProduct.Title),
			_ => gameProducts
		};

		var gameProductsVM = new GameProductsVM
		{
			GameProducts = await PaginationList<GameProduct>.CreateAsync(gameProducts, page, pageSize),
			SortGPVM = new(sortGameProductState),
			FilteredGP_VM = new(new(gameProductGenre), selectedGenreGameProduct, selectedTitleGamePoduct)

		};
		return View(gameProductsVM);
	}
	public async Task<IActionResult> PopularGames()
	{

		var Carts = await gameShopContext.carts
			 .Include(cart => cart.gameProducts)
			 .Where(cart =>
			 cart.DatePurchase >= DateTime.UtcNow.Date.AddMonths(minValueMonth) &&
			 cart.DatePurchase <= DateTime.UtcNow.Date)
			 .ToListAsync();

		var dictionary = Carts.SelectMany(cart => cart.gameProducts)
			.GroupBy(gameProduct => gameProduct.Id)
			.Take(countPopGames)
			.ToDictionary(group => group.Key, group => group.First());

		return View(dictionary);
	}
	
	public async Task<IActionResult> Recommendations() // эксперты рекомендовали
	{
		var recommendedGameProducts = await gameShopContext.RecommendedGameProducts
			.Include(recom => recom.GameProduct).ToListAsync();

		var listGameProductsByExpert = new Dictionary<NameSurname, List<GameProduct>>();

		foreach (var product in recommendedGameProducts)
		{
			var nameSurname=new NameSurname(product.ExpertName, product.ExpertSurname);
			if (!listGameProductsByExpert.TryGetValue(nameSurname, out List<GameProduct>? value))
			{
				listGameProductsByExpert.Add(nameSurname, [product.GameProduct]);
			}
			else
			{
				listGameProductsByExpert[nameSurname].Add(product.GameProduct);
			}
		}

		return View(listGameProductsByExpert);
	}
	public async Task<IActionResult> WishList()
	{

		var idUser = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

		var wishList = await gameShopContext.WishList
			.Include(wishList => wishList.user)
			.Include(wishList => wishList.Gameproduct)
			.Where(wishlist => wishlist.user.Id == idUser)
			.ToListAsync();

		return View(wishList);
	}

	public async Task<IActionResult> DeleteWishList(int Id)
	{
		var idUser = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

		var wishlist = await gameShopContext.WishList
			.Include(wishlist => wishlist.user)
			.Include(wishlist => wishlist.Gameproduct)
			.Where(wishlist => wishlist.Gameproduct.Id == Id && wishlist.user.Id == idUser)
			.ExecuteDeleteAsync();

		return RedirectToAction("WishList", "Home");
	}
}
