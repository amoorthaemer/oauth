using OpenPolicyAgent.Common.Client.Generics;
using OpenPolicyAgent.Common.Client.Responses;
using Refit;

namespace OpenPolicyAgent.Common.Client;

public partial interface IOpenPolicyAgentClient {
	// Query API

	[Post("/")]
	Task<WithResult<TResult>> SimpleQueryAsync<TInput, TResult>([Body] WithInput<TInput> request);

	[Post("/v1/query")]
	Task<WithResult<TResult>> AdhocQueryAsync<TInput, TResult>(
		[Body] WithQueryInput<TInput> request,
		[Query, AliasAs("metrics")] bool? includeMetrics = null);
}
