using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameShop.Repositoery.Repositories;
using GameShop.Repositoery.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyGameStoreModel.Data;
using MyGameStoreModel.Entities;

namespace MyGameStore.Controllers;

public class RecommendedGameProductsController(IRepositoryRecommendedGameProduct repositoryRecommendedGameProduct) : Controller
{
    

    // GET: RecommendedGameProducts
    public async Task<IActionResult> Index()=>       
        View(await repositoryRecommendedGameProduct.GetAllAsync());
    

    // GET: RecommendedGameProducts/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        
        var recommendedGameProducts=await repositoryRecommendedGameProduct.GetByIdAsync(id.Value);

        if (recommendedGameProducts == null)
        {
            return NotFound();
        }

        return View(recommendedGameProducts);
    }

    // GET: RecommendedGameProducts/Create
    public IActionResult Create()
    {
        return View();
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,ExpertName,ExpertSurname")] RecommendedGameProducts recommendedGameProducts)
    {
        if (ModelState.IsValid)
        {
            await repositoryRecommendedGameProduct.AddAsync(recommendedGameProducts);
            
            return RedirectToAction(nameof(Index));
        }
        return View(recommendedGameProducts);
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

    // POST: RecommendedGameProducts/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                if (! (await RecommendedGameProductsExists(recommendedGameProducts.Id)))
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

    // GET: RecommendedGameProducts/Delete/5
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

    // POST: RecommendedGameProducts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await repositoryRecommendedGameProduct.removeAsync(id);        
        return RedirectToAction(nameof(Index));
    }

    private async Task <bool> RecommendedGameProductsExists(int id)
    {
        var recommendedGameProduct = await repositoryRecommendedGameProduct .GetByIdAsync(id);

        return recommendedGameProduct!=null;
    }
}
