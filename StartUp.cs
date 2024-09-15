using MyGameStoreModel.Data;
using MyGameStoreModel.Repositories.Interfaces;
using MyGameStoreModel.Repositories;
using Microsoft.EntityFrameworkCore;
using MyGameStore.Repository.Interfaces;
using MyGameStore.Repository;
using MyGameStoreModel.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MyGameStore;

public class StartUp(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        
        services.AddTransient<IGameProductRepository, GameProductRepository>();
        services.AddTransient<IGenreRepository, GenreRepository>();
        services.AddTransient<IImageUrlRepository, ImageUrlRepository>();
		services.AddTransient<IRepositoryWishList, RepositoryWishList>();
		services.AddSingleton<IRepositoryCart, RepositoryCart>();
		services.AddControllersWithViews();
		services.AddDbContext<GameShopContext>(options =>
           options.UseSqlServer(configuration.GetConnectionString("GameShopContext") ??
               throw new InvalidOperationException("Connection string 'GameShopContext' not found.")));
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<GameShopContext>();

        services.AddMemoryCache();
        services.AddSession();
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{Id?}");
        });

        //Seed.SeedUsersAndRolesAsync(app);
    }
}
