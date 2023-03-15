using System.Security.Claims;
using OpenPolicyAgent.Common.Client.Responses;

namespace OpenPolicyAgent.Common.Services;

public interface IOpenPolicyAgentAuthorizationService {
	// methods

	bool IsAuthorized(WithResult<bool?> response, ClaimsPrincipal user, string? resource);

}
