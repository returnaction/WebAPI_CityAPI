using Microsoft.AspNetCore.StaticFiles;
using NLog.Fluent;

namespace CityInfo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //added
            builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseHttpsRedirection();

            // added
            app.UseRouting();

            app.UseAuthorization();

            // changed
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });



            app.Run();
        }
    }
}

// stopped at chapter 5 video 3 did not install the same logger as in the video;
