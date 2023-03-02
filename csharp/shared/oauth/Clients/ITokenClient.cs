using IdentityModel.Jwk;

namespace OAuth.Common.Clients;

public interface ITokenClient {
	//
	Task<string?> GetToken(params string[] scopes);
	Task<string?> GetToken(CancellationToken cancellationToken, params string[] scopes);
	
	Task<string?> GetRefreshedToken(string refreshToken);
	Task<string?> GetRefreshedToken(string refreshToken, CancellationToken cancellationToken);
	
	Task<string?> GetDelegatedToken(string subjectToken, params string[] scopes);
	Task<string?> GetDelegatedToken(string subjectToken, CancellationToken cancellationToken, params string[] scopes);
	
	Task<string?> GetImpersonatedToken(string subjectToken, params string[] scopes);
	Task<string?> GetImpersonatedToken(string subjectToken, CancellationToken cancellationToken, params string[] scopes);

	Task<ICollection<JsonWebKey>> GetSigningKeys();
}
