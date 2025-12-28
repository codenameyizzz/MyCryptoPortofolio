using Microsoft.EntityFrameworkCore;
using MyCryptoPortfolio.Infrastructure.Data;
using MyCryptoPortfolio.Domain.Interfaces;
using MyCryptoPortfolio.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IPriceService, CoinGeckoPriceService>();

// --- TAMBAHAN PENTING 1: Database ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- TAMBAHAN PENTING 2: Daftarkan Memory Cache ---
builder.Services.AddMemoryCache(); // <--- JANGAN LEWATKAN INI

// --- TAMBAHAN PENTING 3: Daftarkan Service & HTTP Client ---
builder.Services.AddHttpClient<IPriceService, CoinGeckoPriceService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
