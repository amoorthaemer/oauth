using System.Security.Claims;
using OpenPolicyAgent.Common.Client.Responses;

namespace OpenPolicyAgent.Common.Services;

public interface IOpenPolicyAgentAuthorizationService {
	// methods
	public bool IsAuthorized(ClaimsPrincipal user, string? resource, WithResult<bool?> response);

}
