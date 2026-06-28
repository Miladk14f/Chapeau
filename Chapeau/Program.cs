using Chapeau.Repositories;
using Chapeau.Repositories.BillRepository;
using Chapeau.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IPaymentRepository, DBPaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<IBillRepository, DBBillRepository>();

builder.Services.AddScoped<IRestaurantTableRepository, DBRestaurantTableRepository>();
builder.Services.AddScoped<IRestaurantTableService, RestaurantTableService>();

builder.Services.AddScoped<IStaffRepository, DBStaffRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();

builder.Services.AddScoped<IMenuItemRepository, DBMenuItemRepository>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();

builder.Services.AddScoped<ICommentRepository, DBCommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddScoped<IOrderRepository, DBOrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IManagerService, ManagerService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Staff}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
