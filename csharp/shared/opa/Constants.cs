using System.Security.Claims;

namespace OpenPolicyAgent.Common;

internal static class Constants {

	public const string BASE_URL = "http://localhost:8181";
	public const string VERSION = "v1";

	public const string POLICY_PREFIX = "opa://";

	public const string NAME_CLAIM_TYPE = ClaimTypes.Name;
	public const string ROLE_CLAIM_TYPE = ClaimTypes.Role;

	public const string DELIMITER = ".";
}
