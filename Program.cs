using Microsoft.EntityFrameworkCore;
using MyGameStoreModel.Data;
using MyGameStoreModel.Repositories;
using MyGameStoreModel.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IGameProductRepository, GameProductRepository>();
builder.Services.AddTransient<IGenreRepository, GenreRepository>();
builder.Services.AddTransient<IImageUrlRepository, ImageUrlRepository>();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GameShopContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("GameShopContext") ??
       throw new InvalidOperationException("Connection string 'GameShopContext' not found.")));

var app = builder.Build();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{Id?}");



app.Run();
