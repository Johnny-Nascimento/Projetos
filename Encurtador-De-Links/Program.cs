using Encurtador_De_Links.Data;
using Microsoft.EntityFrameworkCore;

namespace Encurtador_De_Links
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DB LinkContext configs
            string connectionString = builder.Configuration.GetConnectionString("LinkConnection");
            builder.Services.AddDbContext<LinkContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Add Automapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add services to the container.

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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
