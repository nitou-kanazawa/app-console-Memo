using Microsoft.EntityFrameworkCore;
using MemoApp.Infrastructure.Data;
using MemoApp.Core.Interfaces;
using MemoApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MemoDbContext>(options =>
    options.UseSqlite("Data Source=memo.db"));

// Repository dependencies
builder.Services.AddScoped<IMemoRepository, MemoRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// Controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
