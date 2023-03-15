using Microsoft.AspNetCore.Authorization;

namespace OpenPolicyAgent.Common.Authorization;

internal sealed record OpenPolicyAgentRequirement(string Policy): IAuthorizationRequirement { }
