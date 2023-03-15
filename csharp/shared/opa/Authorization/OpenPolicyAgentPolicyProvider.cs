using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using OpenPolicyAgent.Common.Client;
using OpenPolicyAgent.Common.Services;

namespace OpenPolicyAgent.Common.Authorization;

using static Constants;

internal sealed class OpenPolicyAgentPolicyProvider: IAuthorizationPolicyProvider, IDisposable {
	// private fields
	private readonly ConcurrentDictionary<string, AuthorizationPolicy> PolicyCache = new();
	private readonly FileSystemWatcher watcher = new();
	private readonly IOpenPolicyAgentClient opaClient;

	// public properties

	public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

	// constructor

	public OpenPolicyAgentPolicyProvider(
		IOpenPolicyAgentClient opaClient,
		IOptions<AuthorizationOptions> options,
		IOptions<OpenPolicyAgentOptions> opaOptions)
	{
		FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
		this.opaClient = opaClient;

		var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Policies");
		if (!Directory.Exists(path)) {
			return;
		}

		watcher.Path = path;
		watcher.Filter = "*.rego";

		UploadPolicies();

		watcher.NotifyFilter =
			NotifyFilters.LastAccess
			| NotifyFilters.LastWrite
			| NotifyFilters.FileName;

		watcher.IncludeSubdirectories = false;

		watcher.Created += UpdatePolicies;
		watcher.Changed += UpdatePolicies;
		watcher.Deleted += UpdatePolicies;
		watcher.Renamed += UpdatePolicies;

		watcher.EnableRaisingEvents = true;
	}

	// public methods

	public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
		FallbackPolicyProvider.GetDefaultPolicyAsync();

	public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
		await FallbackPolicyProvider.GetFallbackPolicyAsync();

	public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName) {
		if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase)) {
			policyName = policyName[POLICY_PREFIX.Length..];

			if (!string.IsNullOrEmpty(policyName)) {
				return Task.FromResult<AuthorizationPolicy?>(
					PolicyCache.GetOrAdd(policyName, oolicy =>
						new AuthorizationPolicyBuilder()
							.AddRequirements(new OpenPolicyAgentRequirement(oolicy))
							.Build()
					)
				);
			}
		}

		return FallbackPolicyProvider.GetPolicyAsync(policyName);
	}

	public void Dispose() {
		watcher.EnableRaisingEvents = false;
		watcher.Dispose();

		GC.SuppressFinalize(this);
	}

	// background service

	private async void UpdatePolicies(object sender, FileSystemEventArgs args) {
		try {
			switch (args.ChangeType) {
				case WatcherChangeTypes.Created:
				case WatcherChangeTypes.Changed: {
					await CreateOrUpdatePolicy(args.FullPath);
					break;
				}

				case WatcherChangeTypes.Deleted: {
					await DeletePolicy(args.FullPath);
					break;
				}

				case WatcherChangeTypes.Renamed when args is RenamedEventArgs renamed: {
					await RenamePolicy(renamed.OldFullPath, renamed.FullPath);
					break;
				}
			}
		} catch {
			// intentionally left empty
		}
	}

	private async void UploadPolicies() {
		var policies = Directory
			.GetFiles(watcher.Path, watcher.Filter)
			.Order()
			.ToList();

		foreach (var policy in policies) {
			try {
				await CreateOrUpdatePolicy(policy);
			} catch {
				// intentionally left empty
			}
		}
	}

	private async Task CreateOrUpdatePolicy(string path) {
		var policy = Path.GetFileNameWithoutExtension(path);
		using var policyData = File.OpenRead(path);

		await opaClient.CreateOrUpdatePolicyAsync(policy, policyData);
	}

	private async Task DeletePolicy(string path) {
		var policy = Path.GetFileNameWithoutExtension(path);
		await opaClient.DeletePolicyAsync(policy);
	}

	private async Task RenamePolicy(string oldPath, string newPath) {
		await DeletePolicy(oldPath);
		await CreateOrUpdatePolicy(newPath);
	}
}
