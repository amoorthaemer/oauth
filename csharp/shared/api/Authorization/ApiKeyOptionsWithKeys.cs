using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.Options;

namespace WeatherForecast.Api.Authorization;

public class ApiKeyOptionsWithKeys: ApiKeyOptions, IOptions<ApiKeyOptionsWithKeys> {
	//
	public ApiKey[] Keys { get; set; } = Array.Empty<ApiKey>();

	ApiKeyOptionsWithKeys IOptions<ApiKeyOptionsWithKeys>.Value => this;
}
