using Serilog;

namespace WeatherForecast.Api;

public class Program {

	public static async Task Main(string[] args) {
		Log.Logger = new LoggerConfiguration()
			.WriteTo.Console()
			.CreateLogger();

		try {
			//await Host
			//	.CreateDefaultBuilder(args)
			//	.UseSerilog()
			//	.ConfigureWebHostDefaults(builder => {
			//		builder.UseStartup<Startup>();
			//})
			//.Build()
			//.RunAsync();
			var builder = WebApplication.CreateBuilder(args);

			builder.Host.UseSerilog();

			var startUp = new Startup(
				builder.Configuration,
				builder.Environment
			);

			startUp.ConfigureServices(builder.Services);

			var app = builder.Build();

			startUp.Configure(app);
			// Add services to the container.

			await app.RunAsync();
		} catch (Exception ex) {
			Log.Fatal(ex, "Application termindated unexpectedly");
		} finally {
			Log.CloseAndFlush();
		}
	}
}
