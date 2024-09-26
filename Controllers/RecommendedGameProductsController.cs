using GameShop.Repositoery.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyGameStoreModel.Entities;
using MyGameStoreModel.Repositories;
using MyGameStoreModel.Repositories.Interfaces;

namespace MyGameStore.Controllers;

public class RecommendedGameProductVM
{
	public required string ExpertName { get; set; }
	public required string ExpertSurname { get; set; }
	public required string SelectedGameProduct {  get; set; }
	public required string SearchGameProduct { get; set; }
	public  SelectList? GameProducts { get; set; }
}


public class RecommendedGameProductsController(IGameProductRepository gameProductRepository, IRepositoryRecommendedGameProduct repositoryRecommendedGameProduct) : Controller
{

	public async Task<IActionResult> Index() =>
		View(await repositoryRecommendedGameProduct.GetAllAsync());

	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var recommendedGameProducts = await repositoryRecommendedGameProduct.GetByIdAsync(id.Value);

		if (recommendedGameProducts == null)
		{
			return NotFound();
		}

		return View(recommendedGameProducts);
	}

	public async Task<IActionResult> Create()
	{
		var recommendedGameProductVM = new RecommendedGameProductVM
		{
			ExpertName = string.Empty,
			ExpertSurname = string.Empty,
			SelectedGameProduct=string.Empty,
			SearchGameProduct=string.Empty,
			GameProducts = new SelectList((await gameProductRepository.GetAllAsync()).Select(x=>x.Title)),

		};

		return View(recommendedGameProductVM);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(RecommendedGameProductVM recommendedGameProductVM)
	{
		if (ModelState.IsValid)
		{
			var gameProduct = await gameProductRepository.GetByTitleAsync(recommendedGameProductVM.SelectedGameProduct);
			if (gameProduct == null)
			{
				// запись не найдена, можно вывести сообщение об ошибке
				ModelState.AddModelError("", "Игровой продукт не найден");
				return View(recommendedGameProductVM);
			}
			var recommendedGameProducts = new RecommendedGameProducts
			{
				ExpertName = recommendedGameProductVM.ExpertName,
				ExpertSurname = recommendedGameProductVM.ExpertSurname,
				GameProduct = gameProduct
			};
			await repositoryRecommendedGameProduct.AddAsync(recommendedGameProducts);
			return RedirectToAction(nameof(Index));
		}
		var gameProducts = (await gameProductRepository.GetAllAsync()).Select(x => x.Title);
		if (!string.IsNullOrEmpty(recommendedGameProductVM.SearchGameProduct))
		{
			gameProducts = gameProducts.Where(gameProduct => gameProduct.ToUpper().Contains(recommendedGameProductVM.SearchGameProduct.ToUpper()));
		}
		recommendedGameProductVM.GameProducts = new SelectList(gameProducts);
		return View(recommendedGameProductVM);
	}


	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}
		var recommendedGameProducts = await repositoryRecommendedGameProduct.GetByIdAsync(id.Value);
		if (recommendedGameProducts == null)
		{
			return NotFound();
		}
		return View(recommendedGameProducts);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, [Bind("Id,ExpertName,ExpertSurname")] RecommendedGameProducts recommendedGameProducts)
	{
		if (id != recommendedGameProducts.Id)
		{
			return NotFound();
		}

		if (ModelState.IsValid)
		{
			try
			{
				await repositoryRecommendedGameProduct.editAsync(id, recommendedGameProducts);


			}
			catch (DbUpdateConcurrencyException)
			{
				if (!(await RecommendedGameProductsExists(recommendedGameProducts.Id)))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			return RedirectToAction(nameof(Index));
		}
		return View(recommendedGameProducts);
	}

	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}
		var recommendedGameProducts = await repositoryRecommendedGameProduct.GetByIdAsync(id.Value);

		if (recommendedGameProducts == null)
		{
			return NotFound();
		}

		return View(recommendedGameProducts);
	}

	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		await repositoryRecommendedGameProduct.removeAsync(id);
		return RedirectToAction(nameof(Index));
	}

	private async Task<bool> RecommendedGameProductsExists(int id)
	{
		var recommendedGameProduct = await repositoryRecommendedGameProduct.GetByIdAsync(id);

		return recommendedGameProduct != null;
	}
}
