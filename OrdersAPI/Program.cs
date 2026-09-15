using FluentValidation;
using FluentValidation.AspNetCore;
using OrdersAPI.Filters;
using OrdersAPI.Middleware;
using OrdersAPI.Validators;
using OrdersAPI.Services;
using OrdersAPI.Services.Contracts;
using OrdersAPI.Data;
using Microsoft.EntityFrameworkCore;
using OrdersAPI.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using OrdersAPI.Adapters;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => 
    options.Filters.Add<ValidateModelFilter>() );

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderRequestValidator>();

var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtKey))
        };
        // Configure other JwtBearer options here if needed
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<ProductMappingProfile>();
    config.AddProfile<UserMappingProfile>();

});

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();


builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 5; // Maximum number of requests allowed
        opt.Window = TimeSpan.FromSeconds(10); // Time window for the limit
    });
   
    options.OnRejected=async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 503;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new 
        { Message = "Too many requests." 
        });
    };
}
);

builder.Services.AddHealthChecks().AddDbContextCheck<OrderDbContext>();
builder.Services.AddMemoryCache();

var app = builder.Build();

//using (var scope=app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
//    context.Database.EnsureCreated();

//    if (context.Products.Any() == false)
//    {
//        context.Products.AddRange(
//            new OrdersAPI.Models.Product() { Name = "Laptop", Price = 3000 },
//            new OrdersAPI.Models.Product() { Name = "Mouse", Price = 12 },
//            new OrdersAPI.Models.Product() { Name = "Keyboard", Price = 15000}
//        );
//        context.SaveChanges();
//    }
//}

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseRateLimiter();
app.UseHealthChecks("/health");
app.MapControllers().RequireRateLimiting("fixed");

app.Run();

