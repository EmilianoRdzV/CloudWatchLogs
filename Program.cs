
using AWS.Logger.SeriLog;
using Serilog;
using Serilog.Formatting.Compact;
namespace CloudWatchLogs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, lc) =>
            {
                lc.ReadFrom.Configuration(ctx.Configuration)
                .WriteTo.AWSSeriLog(configuration: ctx.Configuration, textFormatter: new RenderedCompactJsonFormatter()); 
            }
                );
            // Add services to the container.
            //builder.Logging.AddAWSProvider();
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
