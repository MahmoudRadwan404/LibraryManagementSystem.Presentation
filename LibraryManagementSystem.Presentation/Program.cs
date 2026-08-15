
using LibraryManagementSystem.Infrastructure.BackgroundServices;
using LibraryManagementSystem.Infrastructure.Extensions;
using LibraryManagementSystem.Presentation.Midddlewares;

namespace LibraryManagementSystem.Presentation
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
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddMemoryCache();
            builder.Services.AddHostedService<OverdueLoanBackgroundService>();
            var app = builder.Build();
            app.UseMiddleware<ExceptionHandlingMiddleware>(); 
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
