using System.Security.Claims;
using OpenPolicyAgent.Common.Client.Generics;
using OpenPolicyAgent.Common.Models;

namespace OpenPolicyAgent.Common.Services;

internal interface IOpenPolicyAgentRequestFactory {
	// methods

	WithInput<Input> CreateRequest(ClaimsPrincipal user);
	WithInput<Input> CreateRequest(ClaimsPrincipal user, string? resource);
}
