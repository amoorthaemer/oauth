using OpenPolicyAgent.Common;

namespace Microsoft.AspNetCore.Authorization;

using static Constants;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class EnforcePolicyAttribute: AuthorizeAttribute {

    // public properties

    public new string? Policy {
        get => base.Policy;
        set {
			if (string.IsNullOrWhiteSpace(value)) {
				throw new ArgumentNullException(nameof(Policy));
			}

			base.Policy = $"{POLICY_PREFIX}{value.Trim()}";
        }
    }

	// constructor

	public EnforcePolicyAttribute(string policyName): base() =>
        Policy = policyName;
}
