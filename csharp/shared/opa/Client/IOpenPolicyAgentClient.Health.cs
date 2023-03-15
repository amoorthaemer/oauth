using Refit;

namespace OpenPolicyAgent.Common.Client;

public partial interface IOpenPolicyAgentClient {
	// Health API

	[Get("/v1/health")]
	Task<string?> GetHealthAsync(
		[Query, AliasAs("bundles")] bool? includeBundles = null,
		[Query, AliasAs("plugins")] bool? includePlugins = null,
		[Query(CollectionFormat.Multi), AliasAs("exclude-plugin")] params string[] pluginsToExclude); 
}
