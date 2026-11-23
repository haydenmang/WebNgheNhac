using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Data;
using WebNgheNhac.Middleware;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình DbContext kết nối SQL Server
builder.Services.AddDbContext<WebNgheNhacDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Cấu hình Supabase Client để lưu trữ audio và cover image
var supabaseUrl = builder.Configuration["Supabase:Url"] ?? "";
var supabaseKey = builder.Configuration["Supabase:Key"] ?? "";
builder.Services.AddSingleton(provider =>
    new Supabase.Client(supabaseUrl, supabaseKey, new SupabaseOptions { AutoConnectRealtime = false }));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Kích hoạt Swagger để sinh tài liệu API tự động
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Bật trang giao diện Swagger UI khi chạy ứng dụng
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web Nghe Nhac API v1");
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
