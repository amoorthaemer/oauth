using System.Security.Claims;
using IdentityModel;
using Newtonsoft.Json;

namespace OpenPolicyAgent.Common.Models;

public class User {
	// public properties

	[JsonProperty(JwtClaimTypes.Subject)]
	public string? Subject { get; set; }

	public string? Name { get; set; }
	public UserClaim[] Claims { get; set; } = Array.Empty<UserClaim>();
	public string[] Roles { get; set; } = Array.Empty<string>();

	// public methods

	public static User FromPrincipal(ClaimsPrincipal user, OpenPolicyAgentOptions options) {
		var roles = user.Claims
			.Where(claim => claim.Type == options.RoleClaimType)
			.ToArray();

		var claims = user.Claims
			.Except(roles)
			.ToArray();

		var result = new User() {
			Subject = user.FindFirstValue(JwtClaimTypes.Subject)
				?? user.FindFirstValue(ClaimTypes.NameIdentifier),

			Name = user.FindFirstValue(options.NameClaimType)
				?? user.FindFirstValue(ClaimTypes.NameIdentifier)
				?? user.FindFirstValue(JwtClaimTypes.PreferredUserName)
				?? user.FindFirstValue(ClaimTypes.Name)
				?? user.Identity?.Name,

			Claims = UserClaim.ToUserClaims(claims),
			Roles = roles.Select(claim => claim.Value).ToArray(),
		};

		result.Subject ??= result.Name;

		return result;
	}
}
