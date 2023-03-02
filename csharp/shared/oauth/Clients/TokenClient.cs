using IdentityModel;
using IdentityModel.Client;
using IdentityModel.Jwk;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OAuth.Common.Clients;

internal class TokenClient: ITokenClient {
	//
	public enum ExchangeTokenStyle {
		Delegation,
		Impersonation
	}

	// 
	private readonly DiscoveryCache cache;
	private readonly HttpClient client;
	private readonly ILogger logger;
	private readonly TokenClientOptions options;

	//
	public TokenClient(IOptions<TokenClientOptions> options, IHttpClientFactory factory, ILogger<TokenClient> logger) {
		this.client = factory.CreateClient();
		this.options = options.Value;
		this.logger = logger;
		this.cache = new DiscoveryCache(this.options.Authority);
	}

	public Task<string?> GetToken(params string[] scopes) =>
		GetToken(CancellationToken.None, scopes);

	public async Task<string?> GetToken(CancellationToken cancellationToken, params string[] scopes) {
		return await ExecuteRequest(async disco => {
			var token = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest() {
				Address = disco.TokenEndpoint,
				ClientId = options.ClientId,
				ClientSecret = options.ClientSecret,
				Scope = scopes.Length != 0 ? string.Join(" ", scopes) : default
			}, cancellationToken);

			return ValidateResponse(token)?.AccessToken;
		});
	}

	public Task<string?> GetRefreshedToken(string refreshToken) =>
		GetRefreshedToken(refreshToken, CancellationToken.None);

	public async Task<string?> GetRefreshedToken(string refreshToken, CancellationToken cancellationToken) {
		return await ExecuteRequest(async disco => {
			var token = await client.RequestRefreshTokenAsync(new RefreshTokenRequest() {
				Address = disco.TokenEndpoint,
				RefreshToken = refreshToken
			});

			return ValidateResponse(token)?.AccessToken;
		});
	}

	public async Task<ICollection<JsonWebKey>> GetSigningKeys() {
		return (await ExecuteRequest(disco => Task.FromResult(disco.KeySet.Keys))) ?? new List<JsonWebKey>();
	}

	public Task<string?> GetDelegatedToken(string subjectToken, params string[] scopes) =>
		GetDelegatedToken(subjectToken, CancellationToken.None, scopes);

	public Task<string?> GetDelegatedToken(string subjectToken, CancellationToken cancellationToken, params string[] scopes) =>
		DelegateToken(subjectToken, ExchangeTokenStyle.Delegation, cancellationToken, scopes);

	public Task<string?> GetImpersonatedToken(string subjectToken, params string[] scopes) =>
		GetImpersonatedToken(subjectToken, CancellationToken.None, scopes);

	public Task<string?> GetImpersonatedToken(string subjectToken, CancellationToken cancellationToken, params string[] scopes) =>
		DelegateToken(subjectToken, ExchangeTokenStyle.Impersonation, cancellationToken, scopes);

	private async Task<string?> DelegateToken(string subjectToken, ExchangeTokenStyle style, CancellationToken cancellationToken, params string[] scopes) {
		return await ExecuteRequest(async disco => {
			var token = await client.RequestTokenExchangeTokenAsync(new TokenExchangeTokenRequest() {
				Address = disco.TokenEndpoint,
				ClientId = options.ClientId,
				ClientSecret = options.ClientSecret,

				SubjectToken = subjectToken,
				SubjectTokenType = OidcConstants.TokenTypeIdentifiers.AccessToken,

				Scope = scopes.Length != 0 ? string.Join(" ", scopes) : default,

				Parameters = {
					{ "exchange_style", style.ToString().ToLower() }
				}
			}, cancellationToken);

			return ValidateResponse(token)?.AccessToken;
		});
	}

	private async Task<T?> ExecuteRequest<T>(Func<DiscoveryDocumentResponse, Task<T>> action) {
		var disco = ValidateResponse(await cache.GetAsync());
		if (disco == null) {
			return default;
		}

		return await action(disco);
	}

	private T? ValidateResponse<T>(T response) where T: ProtocolResponse {
		if (response.IsError) {
			logger.LogError(response.Error);
			return null;
		}

		return response;
	}
}
