﻿using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using AspNetCore.Authentication.ApiKey;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using OpenPolicyAgent.Common;
using WeatherForecast.Api.Authorization;
using WeatherForecast.Api.Controllers;
using WeatherForecast.Api.Services;

namespace WeatherForecast.Api;

internal class Startup {
	// consts
	private const string MultiAuthenticationScheme = "Multi";

	// settings

	public string Authority { get; set; } = "https://localhost";
	public string Audience { get; set; } = "weatherapps";

	public string Realm { get; set; } = "Weather API";
	public string ApiKeyName { get; set; } = "X-API-KEY";


	// properties

	internal IConfigurationRoot Configuration { get; }
	internal IWebHostEnvironment Environment { get; }


	public Startup(IConfigurationRoot configuration, IWebHostEnvironment environment) {
		Configuration = configuration;
		Environment = environment;

		Configuration.GetSection(nameof(Startup)).Bind(this);
	}

	public void ConfigureServices(IServiceCollection services) {
		var jwtBearerSection = $"{nameof(Authorization)}:{nameof(JwtBearerOptions)}";
		var apiKeySection = $"{nameof(Authorization)}:{nameof(ApiKeyOptions)}";

		services.Configure<JwtBearerOptions>(
			JwtBearerDefaults.AuthenticationScheme,
			Configuration.GetSection(jwtBearerSection)
		);

		services.Configure<ApiKeyOptionsWithKeys>(Configuration.GetSection(apiKeySection));

		services.Configure<ApiKeyOptions>(
			ApiKeyDefaults.AuthenticationScheme,
			Configuration.GetSection(apiKeySection)
		);

		services.Configure<KestrelServerOptions>(options =>
		{
			options.ConfigureHttpsDefaults(httpsOptions => {
				httpsOptions.AllowAnyClientCertificate();
				httpsOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
			});
		});

		services.Configure<OpenPolicyAgentOptions>(Configuration.GetSection(nameof(OpenPolicyAgentOptions)));

		//// NGINX
		//services.AddCertificateForwarding(options => {
		//	options.CertificateHeader = "ssl-client-cert";

		//	options.HeaderConverter = (headerValue) =>
		//	{
		//		X509Certificate2? certificate = null;

		//		if (!string.IsNullOrWhiteSpace(headerValue)) {
		//			certificate = X509Certificate2.CreateFromPem(
		//				WebUtility.UrlDecode(headerValue));
		//		}
		//		return certificate!;
		//	};
		//});

		services
			.AddTransient<IClaimsTransformation, ClaimsTransformer>()
			.AddHttpContextAccessor();

		services.AddAuthentication(options => {
			options.DefaultScheme = MultiAuthenticationScheme;
			options.DefaultChallengeScheme = MultiAuthenticationScheme;
		})
		// OAuth 2.0
		.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
			var signingKeys = GetSigningKeys(Authority);
			var hasSigningKeys = signingKeys.Any();

			options.Authority = Authority;
			options.Audience = Audience;

			options.TokenValidationParameters ??= new();

			options.TokenValidationParameters.ValidIssuer = Authority;
			options.TokenValidationParameters.ValidIssuers ??= new List<string>();
			options.TokenValidationParameters.ValidIssuers = new[] { Authority }
				.Concat(options.TokenValidationParameters.ValidIssuers)
				.Distinct()
				.ToArray();

			options.TokenValidationParameters.ValidAudience = Audience;
			options.TokenValidationParameters.ValidAudiences ??= new List<string>();
			options.TokenValidationParameters.ValidAudiences = new[] { Audience }
				.Concat(options.TokenValidationParameters.ValidAudiences)
				.Distinct()
				.ToArray();

			options.TokenValidationParameters.RequireSignedTokens = hasSigningKeys;
			options.TokenValidationParameters.IssuerSigningKey = hasSigningKeys
				? signingKeys.First()
				: null;
		})
		// API-KEY
		.AddApiKeyInHeader<ApiKeyProvider>(ApiKeyDefaults.AuthenticationScheme)
		// WS-Federation
		.AddCookie()
		.AddWsFederation(WsFederationDefaults.AuthenticationScheme, options => {
			options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

			options.Wtrealm = "api://221b41c5-fc29-431e-b3fe-4dbb838c5bba";
			options.MetadataAddress = "https://login.microsoftonline.com/a4a8edd7-6215-46f9-a1b0-e267e20c88c6/federationmetadata/2007-06/federationmetadata.xml";
		})
		// CERTIFICATE
		.AddCertificate(CertificateAuthenticationDefaults.AuthenticationScheme, options => {
			options.RevocationMode = X509RevocationMode.NoCheck;
			options.AllowedCertificateTypes = CertificateTypes.All;

			options.Events = new CertificateAuthenticationEvents() {
				OnCertificateValidated = context => {
					var claims = new[] {
						GetClaim(JwtClaimTypes.Subject, context.ClientCertificate.Subject),
						GetClaim(ClaimTypes.NameIdentifier, context.ClientCertificate.Subject),
						GetClaim(ClaimTypes.Name, context.ClientCertificate.Subject),
						new(JwtClaimTypes.Audience, Audience)
					};

					context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
					context.Success();

					return Task.CompletedTask;

					//
					Claim GetClaim(string type, string value, string? valueType = ClaimValueTypes.String) =>
						new Claim(type, value, valueType, context.Options.ClaimsIssuer);
				}
			};
		})
		// Swagger AuthN
		//.AddCookie("CookieScheme")
		//.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => {
		//	options.SignInScheme = "CookieScheme";
		//	options.Authority = Authority;
		//	options.ClientId = "DNhnanoGorHpOB7kDMedJtYPl2Ma";
		//	options.ClientSecret = "u3rrdgWMp2IL0qPR2_6DyUm9qrMa";
		//	options.ResponseType = "code";
		//	options.Prompt = "Login";
		//	options.GetClaimsFromUserInfoEndpoint = true;
		//	options.SaveTokens = true;
		//})
		// AuthN SELECTOR
		.AddPolicyScheme(MultiAuthenticationScheme, string.Empty, options => {
			options.ForwardDefaultSelector = context => {
				string? header = context.Request.Headers[HeaderNames.Authorization];

				if (!string.IsNullOrEmpty(header)) {
					if (header.StartsWith(JwtBearerDefaults.AuthenticationScheme)) {
						return JwtBearerDefaults.AuthenticationScheme;
					}

					if (header.StartsWith(ApiKeyDefaults.AuthenticationScheme)) {
						return ApiKeyDefaults.AuthenticationScheme;
					}
				}

				header = context.Request.Headers[ApiKeyName];
				if (!string.IsNullOrEmpty(header)) {
					return ApiKeyDefaults.AuthenticationScheme;
				}

				//return WsFederationDefaults.AuthenticationScheme;
				return CertificateAuthenticationDefaults.AuthenticationScheme;
			};
		});

		services
			.AddOpenPolicyAgent()
			.AddAuthorization();

		services.AddCors(options => {
			options.AddDefaultPolicy(policyBuilder => {
				policyBuilder
					.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowAnyOrigin();
			});
		});

		services
			.AddControllers()
			.AddNewtonsoftJson()
			.AddApplicationPart(typeof(WeatherForecastController).Assembly);

		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(options => {
			options.SwaggerDoc("v1", new OpenApiInfo() {
				Title = "Weather Forecast API",
				Version = "v1"
			});

			// OAuth 2.0
			options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme() {
				In = ParameterLocation.Header,
				Description = "Please enter a valid client-ID and secret",
				Name = HeaderNames.Authorization,
				Type = SecuritySchemeType.OAuth2,
				Flows = new OpenApiOAuthFlows() {
					ClientCredentials = new OpenApiOAuthFlow() {
						TokenUrl = new Uri(Authority),
						Scopes = new Dictionary<string, string>(),
					}
				},
				BearerFormat = "base64(JWT)",
				Scheme = JwtBearerDefaults.AuthenticationScheme
			});

			options.AddSecurityRequirement(new OpenApiSecurityRequirement() {
				{
					new OpenApiSecurityScheme() {
						Reference = new OpenApiReference() {
							Type = ReferenceType.SecurityScheme,
							Id = JwtBearerDefaults.AuthenticationScheme
						},
						In = ParameterLocation.Header,
					},
					new string[] {}
				}
			});

			// API Key
			options.AddSecurityDefinition(ApiKeyDefaults.AuthenticationScheme, new OpenApiSecurityScheme() {
				In = ParameterLocation.Header,
				Description = $"Please enter a valid API Key",
				Name = ApiKeyName,
				Type = SecuritySchemeType.ApiKey,
				Scheme = ApiKeyDefaults.AuthenticationScheme
			});

			options.AddSecurityRequirement(new OpenApiSecurityRequirement() {
				{
					new OpenApiSecurityScheme() {
						Reference = new OpenApiReference() {
							Type = ReferenceType.SecurityScheme,
							Id = ApiKeyDefaults.AuthenticationScheme
						},
						In = ParameterLocation.Header,
					},
					new string[] {}
				}
			});
		});
	}

	public void Configure(WebApplication app) {
		if (Environment.IsDevelopment()) {
			app.UseSwagger(options => {
				options.PreSerializeFilters.Add((swagger, httpReq) => {
					swagger.Servers = new List<OpenApiServer> {
						new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
					};
				});
			});
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();
		app.UseCors();

		app.UseRouting();
		app.UseCookiePolicy();

		app.UseCertificateForwarding();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();
	}

	private ICollection<SecurityKey> GetSigningKeys(string? authority) {
		if (!string.IsNullOrWhiteSpace(authority)) {
			var cfgManager = new ConfigurationManager<OpenIdConnectConfiguration>(
				$"{authority}/.well-known/openid-configuration",
				new OpenIdConnectConfigurationRetriever(),
				new HttpDocumentRetriever());

			try {
				var config = cfgManager.GetConfigurationAsync().Result;
				if (config?.SigningKeys != null) {
					return config.SigningKeys;
				}
			} catch {
				// do nothing
			}
		}

		return new List<SecurityKey>();
	}
}