using AspNetCore.Authentication.ApiKey;
using System.Security.Claims;

namespace WeatherForecast.Api.Authorization;

public sealed class ApiKey: IApiKey {
	//
	public string? Key { get; set; }
	public string? OwnerName { get; set; }
	public IReadOnlyCollection<Claim> Claims { get; set; } = new List<Claim>();

	//
	public ApiKey() { }

	internal ApiKey(string key, string ownerName, IReadOnlyCollection<Claim>? claims = default) {
		Key = key;
		OwnerName = ownerName;
		Claims = claims ?? new List<Claim>();
	}
}
