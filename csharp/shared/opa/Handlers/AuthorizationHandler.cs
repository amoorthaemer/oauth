using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OpenPolicyAgent.Common.Handlers;

internal sealed class AuthorizationHandler: DelegatingHandler {
    // private fields

    private readonly IServiceProvider serviceProvider;
    private readonly OpenPolicyAgentOptions options;

    // constructor

    public AuthorizationHandler(IServiceProvider serviceProvider, IOptions<OpenPolicyAgentOptions> options) {
        this.serviceProvider = serviceProvider;
        this.options = options.Value;
    }

    // protected methods

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        if (options.RequireAuthentication && request.Headers.Authorization == null) {
            var httpContext = serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
            if (httpContext != null) {
				string? authorization = httpContext.Request.Headers.Authorization;
				request.Headers.Authorization = AuthenticationHeaderValue.TryParse(authorization, out var result) ? result : null;
			}
		}

		return base.SendAsync(request, cancellationToken);
	}
}
