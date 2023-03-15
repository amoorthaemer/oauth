using Microsoft.Extensions.Options;

namespace OpenPolicyAgent.Common;

using static Constants;

public class OpenPolicyAgentOptions: IOptions<OpenPolicyAgentOptions> {
	// public properties

	public string BaseUrl { get; set; } = BASE_URL;
	public string NameClaimType { get; set; } = NAME_CLAIM_TYPE;
	public string RoleClaimType { get; set; } = ROLE_CLAIM_TYPE;

	public bool RequireAuthentication { get; set; } = false;
	public bool IgnoreMissingPolicies { get; set; } = false;
	public bool? IncludeMetrics { get; set; } = null;
	public string Delimiter { get; set; } = DELIMITER;

	// interfaces
	// -- IOptions<OpenPolicyAgentOptions>

	OpenPolicyAgentOptions IOptions<OpenPolicyAgentOptions>.Value => this;
}
