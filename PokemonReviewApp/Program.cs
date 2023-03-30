using System.Configuration;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Repository;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));
// Add services to the container.
//builder.Services.AddDbContext<DataContext>(options =>
//options.UseSqlServer(
//builder.Configuration.GetConnectionString("DefaultConnection")
//));
builder.Services.AddDbContext<DataContext>(options => options.UseMySql(connectionString, serverVersion ));
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<Seed>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();

var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata") SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope()) { var service = scope.ServiceProvider.GetService<Seed>(); service.SeedDataContext(); }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

