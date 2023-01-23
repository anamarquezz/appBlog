using BlogLab.Models.Settings;
using BlogLab.Repository;
using BlogLab.Services;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BlogLab.Models.Account;
using BlogLab.Identity;
using BlogLab.web.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

/*
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true);
builder.Configuration.AddEnvironmentVariables();
*/

builder.Services.Configure<CloudinaryOptions>(builder.Configuration.GetSection("CloudinaryOptions"));


builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IBlogCommentRepository, BlogCommentRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();

builder.Services.AddIdentityCore<ApplicationUserIdentity>(opt =>
    {
        opt.Password.RequireNonAlphanumeric = false;

    })
    .AddUserStore<UserStore>()
    .AddDefaultTokenProviders()
    .AddSignInManager<SignInManager<ApplicationUserIdentity>>();

builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(
        options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"])),
                ClockSkew = TimeSpan.Zero
            };
        }
    );
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorizationCore();
// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.ConfigureExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
}
else
{
    app.UseCors(options => options.WithOrigins("http://website.com"));
}

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.MapRazorPages();

app.Run();
