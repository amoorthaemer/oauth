using OpenPolicyAgent.Common.Client.Generics;
using OpenPolicyAgent.Common.Client.Responses;
using Refit;

namespace OpenPolicyAgent.Common.Client;

public partial interface IOpenPolicyAgentClient {
	// Data API

	[Get("/v1/data/{**path}")]
	Task<WithResult<TResult>> GetDocumentAsync<TResult>(
		string path,
		[Query, AliasAs("metrics")] bool? includeMetrics = null);

	[Post("/v1/data/{**path}")]
	Task<WithResult<TResult>> GetDocumentAsync<TInput, TResult>(
		string path,
		[Body] WithInput<TInput> request,
		[Query, AliasAs("metrics")] bool? includeMetrics = null);

	[Put("/v1/data/{**path}")]
	Task CreateOrOverwriteDocumentAsync(
		string path,
		[Body] string data,
		[Query, AliasAs("metrics")] bool? includeMetrics = null);

	[Put("/v1/data/{**path}")]
	Task CreateOrOverwriteDocumentAsync(
		string path,
		[Body] Stream data,
		[Query, AliasAs("metrics")] bool? includeMetrics = null);

	// NOTE: PATCH isn't syupported yet

	[Delete("/v1/data/{**path}")]
	Task DeleteDocumentAsync(
		string path,
		[Query, AliasAs("metrics")] bool? includeMetrics = null);
}
