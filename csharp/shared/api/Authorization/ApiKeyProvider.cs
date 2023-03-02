using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WeatherForecast.Api.Authorization;

public sealed class ApiKeyProvider: IApiKeyProvider {
	//
	private readonly ApiKey[] apiKeys;
	private readonly ILogger<IApiKeyProvider> logger;

	//
	public ApiKeyProvider(IOptions<ApiKeyOptionsWithKeys> options, ILogger<IApiKeyProvider> logger) {
		this.apiKeys = options.Value.Keys;
		this.logger = logger;
	}

	//
	public Task<IApiKey> ProvideAsync(string key) {
		try {
			return Task.FromResult<IApiKey>(apiKeys.Single(apiKey => apiKey.Key == key));
		} catch (Exception ex) {
			logger.LogError(ex, ex.Message);
			throw;
		}
	}
}
