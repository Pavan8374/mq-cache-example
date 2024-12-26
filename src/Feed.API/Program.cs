using Feed.API;
using Feed.Domain.Contents;
using Feed.Domain.UserInteractions;
using Feed.Domain.Users;
using Feed.EF;
using Feed.EF.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

var startup = new StartUp(configuration);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddTransient<IContentRepository,ContentRepository>();
builder.Services.AddTransient<IContentService, ContentService>();


builder.Services.AddTransient<IUserLikeRepository, UserLikeRepository>();
builder.Services.AddTransient<IUserLikeService, UserLikeService>();


startup.ConfigureServices(builder.Services);


//builder.Services.AddSingleton<ICacheService, RedisCacheService>();
//builder.Services.AddScoped<ILikeManager, LikeManager>();

//builder.Services.AddSingleton<IMessageQueueService, RabbitMQService>();
//builder.Services.AddSingleton<ICacheService, RedisCacheService>();
//builder.Services.AddHostedService<LikeProcessingService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
