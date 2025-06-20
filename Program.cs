using System.Net;
using System.Text;
using System.Threading.RateLimiting;
using Blog.Context;
using Blog.entities;
using Blog.SetRepositories.IRepositories;
using Blog.SetRepositories.Repositories;
using Blog.SetServices.IServices;
using Blog.SetServices.Services;
using Blog.SetUnitOfWork;
using Blog.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(connectionString).UseLazyLoadingProxies()
);

IConfigurationSection? jwtSettings = builder.Configuration.GetSection("jwt");
string? secretKey = jwtSettings.GetValue<string>("SecretKey");

if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(connectionString) ) 
    throw new InvalidOperationException("JWT SecretKey is missing in configuration.");

builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["ValidIssuer"],
        ValidAudience = jwtSettings["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var apiVersionDescriptionProvider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        c.SwaggerDoc(description.GroupName, new OpenApiInfo
        {
            Title = $"My Blog API {description.ApiVersion}", 
            Version = description.ApiVersion.ToString(),     
            Description = description.IsDeprecated ? "This API version has been deprecated." : "API for blog system with Swagger and PostgreSQL.",
            Contact = new OpenApiContact
            {
                Name = "Anderson",
                Email = "anderson.c.rms2005@gmail.com"
            }
        });
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddRateLimiter(RateLimiterOptions =>
{
    RateLimiterOptions.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
    };

    RateLimiterOptions.AddFixedWindowLimiter("fixedWindowLimiterPolicy", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromSeconds(8);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    });

    RateLimiterOptions.AddSlidingWindowLimiter("SlidingWindowLimiterPolicy", options => 
    {
        options.PermitLimit = 16;
        options.Window = TimeSpan.FromSeconds(10);
        options.SegmentsPerWindow = 2;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
    });

    RateLimiterOptions.AddConcurrencyLimiter("ConcurrencyLimiterPolicy", options => 
    {
        options.PermitLimit = 6;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    });

    RateLimiterOptions.AddFixedWindowLimiter("CreateItemPolicy", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromMinutes(10);
        options.QueueLimit = 0;
    });

    RateLimiterOptions.AddFixedWindowLimiter("DeleteItemPolicy", options =>
    {
        options.PermitLimit = 8;
        options.Window = TimeSpan.FromMinutes(12);
        options.QueueLimit = 0;
    });

    RateLimiterOptions.AddFixedWindowLimiter("UpdateItemPolicy", options =>
    {
        options.PermitLimit = 8;
        options.Window = TimeSpan.FromMinutes(12);
        options.QueueLimit = 0;
    });

    RateLimiterOptions.AddFixedWindowLimiter("authSystemPolicy", options => 
    {
        options.PermitLimit = 3;
        options.Window = TimeSpan.FromSeconds(16);
        options.QueueLimit = 0;
    });
});

builder.Services.AddApiVersioning(options => 
{
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1,0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddControllers();

builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserMetricRepository, UserMetricRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostMetricRepository, PostMetricRepository>();
builder.Services.AddScoped<IFavoritePostRepository, FavoritePostRepository>();
builder.Services.AddScoped<IFavoriteCommentRepository, FavoriteCommentRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentMetricRepository, CommentMetricRepository>();
builder.Services.AddScoped<IReactionPostRepository, ReactionPostRepository>();
builder.Services.AddScoped<IReactionCommentRepository, ReactionCommentRepository>();
builder.Services.AddScoped<IPlaylistItemRepository, PlaylistItemRepository>();
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<IRecoverAccountRepository, RecoverAccountRepository>();
builder.Services.AddScoped<IMediaPostRepository, MediaPostRepository>();

builder.Services.AddOpenApi();

WebApplication? app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
