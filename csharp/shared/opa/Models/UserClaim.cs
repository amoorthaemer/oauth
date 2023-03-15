using System.Security.Claims;

namespace OpenPolicyAgent.Common.Models;

public sealed class UserClaim {
	// public properties

	public string Type { get; set; } = null!;
	public string Value { get; set; } = null!;

	// internal methods

	internal static UserClaim[] ToUserClaims(params Claim[] claims) =>
		claims.Select(claim => new UserClaim() {
			Type = claim.Type,
			Value = claim.Value
		}).ToArray();
}