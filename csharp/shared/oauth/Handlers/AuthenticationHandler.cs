using IdentityModel.Client;
using Microsoft.Extensions.Logging;

namespace OAuth.Common.Handlers;

internal class AuthenticationHandler: DelegatingHandler {
	// 
	private readonly ILogger logger;
	private readonly DiscoveryCache cache;

	public AuthenticationHandler(ILogger<AuthenticationHandler> logger) {
		this.logger = logger;
		this.cache = new DiscoveryCache("");
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
		var disco = await cache.GetAsync();
		if (!disco.IsError) {
			using var client = new HttpClient();
		}
		return await base.SendAsync(request, cancellationToken);
	}
}
