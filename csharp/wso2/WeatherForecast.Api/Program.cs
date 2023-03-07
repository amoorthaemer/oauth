using Serilog;

namespace WeatherForecast.Api;

public class Program {

	public static async Task Main(string[] args) {
		Log.Logger = new LoggerConfiguration()
			.WriteTo.Console()
			.CreateLogger();

		try {
			var builder = WebApplication.CreateBuilder(args);

			builder.Host.UseSerilog();

			var startUp = new Startup(
				builder.Configuration,
				builder.Environment
			);

			startUp.ConfigureServices(builder.Services);

			var app = builder.Build();

			startUp.Configure(app);

			await app.RunAsync();
		} catch (Exception ex) {
			Log.Fatal(ex, "Application termindated unexpectedly");
		} finally {
			Log.CloseAndFlush();
		}
	}
}
