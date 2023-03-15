using OpenPolicyAgent.Common.Client.Generics;
using OpenPolicyAgent.Common.Client.Responses;

namespace OpenPolicyAgent.Common.Client;

public static class Extensions {
	// public methods

	public static Task<WithResult<bool?>> EvaluatePolicyAsync<TInput>(
		this IOpenPolicyAgentClient client,
		string path, WithInput<TInput> request,
		bool? includeMetrics = null) =>
		client.GetDocumentAsync<TInput, bool?>(path, request, includeMetrics);
}
