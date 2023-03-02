using Microsoft.Extensions.Options;

namespace OAuth.Common.Clients;

internal class TokenClientOptions: IOptions<TokenClientOptions> {
	//
	public string? Authority { get; set; }
	public string? ClientId { get; set; }
	public string? ClientSecret { get; set; }


	TokenClientOptions IOptions<TokenClientOptions>.Value => this;
}
