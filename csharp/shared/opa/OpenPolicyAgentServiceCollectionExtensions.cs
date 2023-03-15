using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenPolicyAgent.Common;
using OpenPolicyAgent.Common.Authorization;
using OpenPolicyAgent.Common.Client;
using OpenPolicyAgent.Common.Handlers;
using OpenPolicyAgent.Common.Services;
using Refit;

namespace Microsoft.Extensions.DependencyInjection;

public static class OpenPolicyAgentServiceCollectionExtensions {
	// private fields

	private static readonly JsonSerializerSettings serializerSettings = new() {
		Formatting = Formatting.None,
		DateTimeZoneHandling = DateTimeZoneHandling.Utc,
		DateFormatString = "yyyy-MM-ddTHH:mm:ss.FFFFFFFZ",
		DateFormatHandling = DateFormatHandling.IsoDateFormat,
		ContractResolver = new DefaultContractResolver() {
			NamingStrategy = new CamelCaseNamingStrategy(),
		},
		NullValueHandling = NullValueHandling.Ignore,
		MissingMemberHandling = MissingMemberHandling.Ignore,
	};

	// public methods

	public static IServiceCollection AddOpenPolicyAgent(this IServiceCollection services, Action<OpenPolicyAgentOptions> setupAction) {
		services
			.Configure(setupAction)
			.AddSingleton<IAuthorizationPolicyProvider, OpenPolicyAgentPolicyProvider>()
			.AddSingleton<IAuthorizationHandler, OpenPolicyAgentAuthorizationHandler>();

		services.TryAddSingleton<IOpenPolicyAgentAuthorizationService, DefaultOpenPolicyAgentAuthorizationService>();
		services.TryAddSingleton<IOpenPolicyAgentRequestFactory, DefaultOpenPolicyAgentRequestFactory>();
		services.TryAddScoped<AuthorizationHandler>();

		services
			.AddHttpContextAccessor()
			.AddRefitClient<IOpenPolicyAgentClient>(new RefitSettings() {
				ContentSerializer = new NewtonsoftJsonContentSerializer(serializerSettings)
			})
			.ConfigureHttpClient((serviceProvider, httpClient) => {
				var options = serviceProvider
					.GetRequiredService<IOptions<OpenPolicyAgentOptions>>()
					.Value;

				httpClient.BaseAddress = new Uri(options.BaseUrl);
				httpClient.DefaultRequestHeaders
					.Accept
					.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

				httpClient.DefaultRequestHeaders
					.AcceptCharset
					.Add(new StringWithQualityHeaderValue(Encoding.UTF8.WebName));
			})
			.AddHttpMessageHandler<AuthorizationHandler>()
			// TODO
			;

		return services;
	}

	public static IServiceCollection AddOpenPolicyAgent(this IServiceCollection services) =>
		services.AddOpenPolicyAgent(_ => { });
}
