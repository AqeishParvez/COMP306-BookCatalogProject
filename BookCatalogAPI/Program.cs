
using BookCatalogAPI.Models;
using BookCatalogAPI.RepositoryImpl;
using BookCatalogAPI.RepositoryInterface;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using BookCatalogAPI.DtoClasses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using MongoDB.Driver;

namespace BookCatalogAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            //Dependency Injection of DbContext Class now being replaced with MongoDB
            //builder.Services.AddDbContext<APIDbContext>(options => 
            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //Configure MongoDB Connection
            var mongoDbSettings = builder.Configuration.GetSection("MongoDBSettings");
            var connectionString = mongoDbSettings.GetValue<string>("ConnectionString");
            var databaseName = mongoDbSettings.GetValue<string>("DatabaseName");

            builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
            builder.Services.AddSingleton<IMongoDatabase>(provider =>
            {
                var client = provider.GetService<IMongoClient>();
                return client.GetDatabase(databaseName);
            });



            builder.Services.AddScoped<IBookInfoRepository, BookInfoRepository>();
            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
            //builder.Services.AddControllers()
            //.AddNewtonsoftJson(options =>
            //{options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //});

            //builder.Services.AddControllers().AddNewtonsoftJson(options => options.UseJsonPatch());


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }


    }
}