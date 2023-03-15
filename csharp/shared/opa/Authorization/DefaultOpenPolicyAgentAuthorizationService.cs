using System.Security.Claims;
using OpenPolicyAgent.Common.Client.Responses;
using OpenPolicyAgent.Common.Services;

namespace OpenPolicyAgent.Common.Authorization;

public class DefaultOpenPolicyAgentAuthorizationService: IOpenPolicyAgentAuthorizationService {
	// public methods

	public virtual bool IsAuthorized(ClaimsPrincipal user, string? resource, WithResult<bool?> response) => true;
}
