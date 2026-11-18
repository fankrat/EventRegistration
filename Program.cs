using Microsoft.EntityFrameworkCore;
using EventRegistration.Data;
using EventRegistration.Services;

var builder = WebApplication.CreateBuilder(args);

// Подключение к базе данных SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=events.db"));

// Регистрация нашего сервиса бизнес-логики
builder.Services.AddScoped<RegistrationService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// === НОВЫЙ БЛОК: Автоматическое применение миграций (создание БД) при запуске ===
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate(); // Если таблиц нет, они будут созданы
}
// ====================================================================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();