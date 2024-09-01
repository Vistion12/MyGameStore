using MyGameStoreModel.Data;
using MyGameStoreModel.Repositories.Interfaces;
using MyGameStoreModel.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MyGameStore;

public class StartUp
{

    public void ConfigureServices(IServiceCollection services)
    {
        //services.AddTransient<IGameProductRepository, GameProductRepository>();
        //services.AddControllersWithViews();
        //services.AddTransient<IGenreRepository, GenreRepository>();
        //services.AddTransient<IImageUrlRepository, ImageUrlRepository>();
        //services.AddDbContext<GameShopContext>(options =>
        //   options.UseSqlServer(builder.Configuration.GetConnectionString("GameShopContext") ??
        //       throw new InvalidOperationException("Connection string 'GameShopContext' not found.")));
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
