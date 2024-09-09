using MyGameStoreModel.Data;
using MyGameStoreModel.Repositories.Interfaces;
using MyGameStoreModel.Repositories;
using Microsoft.EntityFrameworkCore;
using MyGameStore.Repository.Interfaces;
using MyGameStore.Repository;

namespace MyGameStore;

public class StartUp(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddTransient<IGameProductRepository, GameProductRepository>();
        services.AddTransient<IGenreRepository, GenreRepository>();
        services.AddTransient<IImageUrlRepository, ImageUrlRepository>();
        services.AddSingleton<IRepositoryCart, RepositoryCart>();
        services.AddDbContext<GameShopContext>(options =>
           options.UseSqlServer(configuration.GetConnectionString("GameShopContext") ??
               throw new InvalidOperationException("Connection string 'GameShopContext' not found.")));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{Id?}");
        });


    }
}
