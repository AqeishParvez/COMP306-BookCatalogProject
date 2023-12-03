
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
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using BookCatalogAPI.Services;

namespace BookCatalogAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var credentialsService = new CredentialsService();
            var ssmClient = new AmazonSimpleSystemsManagementClient();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            //Dependency Injection of DbContext Class now being replaced with MongoDB
            //builder.Services.AddDbContext<APIDbContext>(options => 
            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Inject IConfiguration into the Program class
            var configuration = builder.Configuration;

            //Configure S3 Client
            builder.Services.AddAWSService<IAmazonS3>(new AWSOptions
            {
                Credentials = credentialsService.GetAWSCredentials(configuration),

                Region = Amazon.RegionEndpoint.CACentral1,
            });

            //Configure MongoDB Connection
            var connectionString = credentialsService.GetMongoDbConnectionString(builder.Configuration, ssmClient);
            var databaseName = credentialsService.GetMongoDbDatabaseName(builder.Configuration, ssmClient);

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