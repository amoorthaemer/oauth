using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenPolicyAgent.Common.Client.Responses;
using Refit;

namespace OpenPolicyAgent.Common.Client;

public partial interface IOpenPolicyAgentClient {
	// Policies API

	[Get("/v1/policies")]
	Task<WithResult<GetPolicyResponse[]>> GetPoliciesAsync();

	[Get("/v1/policies/{policyId}")]
	Task<WithResult<GetPolicyResponse>> GetPolicyAsync(string policyId);

	[Put("/v1/policies/{policyId}")]
	Task CreateOrUUpdatePolicyAsync(string policyId, [Body] string policy);

	[Put("/v1/policies/{policyId}")]
	Task CreateOrUpdatePolicyAsync(string policyId, [Body] Stream policyData);

	[Delete("/v1/policies/{policyId}")]
	Task DeletePolicyAsync(string policyId);
}
