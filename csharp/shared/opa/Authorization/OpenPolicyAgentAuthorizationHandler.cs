using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenPolicyAgent.Common.Client;
using OpenPolicyAgent.Common.Services;
using Refit;

namespace OpenPolicyAgent.Common.Authorization;

internal sealed class OpenPolicyAgentAuthorizationHandler: AuthorizationHandler<OpenPolicyAgentRequirement> {
	// private fields
	private readonly IOpenPolicyAgentClient opaClient;
	private readonly IOpenPolicyAgentRequestFactory opaRequestFactory;
	private readonly IOpenPolicyAgentAuthorizationService opAuthorizationService;
	private readonly OpenPolicyAgentOptions options;
	private readonly ILogger<OpenPolicyAgentAuthorizationHandler> logger;


	public OpenPolicyAgentAuthorizationHandler(
		IOpenPolicyAgentClient opaClient,
		IOpenPolicyAgentRequestFactory opaRequestFactory,
		IOpenPolicyAgentAuthorizationService opAuthorizationService,
		IOptions<OpenPolicyAgentOptions> options,
		ILogger<OpenPolicyAgentAuthorizationHandler> logger
	)
    {
		this.opaClient = opaClient;
		this.opaRequestFactory = opaRequestFactory;
		this.opAuthorizationService = opAuthorizationService;
		this.options = options.Value;
		this.logger = logger;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OpenPolicyAgentRequirement requirement) {
		var user = context.User;
		var resource = context.Resource as string;

		var request = opaRequestFactory.CreateRequest(user, resource);
	
		try {
			var response = await opaClient
				.EvaluatePolicyAsync(requirement.Policy.Replace(options.Delimiter, "/"), request, options.IncludeMetrics)
				.ConfigureAwait(false);

			if (response.Result == null) {
				logger.LogWarning("Evaluation requested for missing policy: \"{Policy}\". Request {@Request}", requirement.Policy, request);

				if (options.IgnoreMissingPolicies) {
					context.Succeed(requirement);
				}

				return;
			}

			logger.LogDebug("Evaluation for policy \"{Policy}\" finished. Request: {@Request}, Response: {@Response}",
				requirement.Policy, request, response);

			if (response.Result == true && opAuthorizationService.IsAuthorized(response, user, resource)) {
				context.Succeed(requirement);
			}
		} catch (ApiException ex) {
			if (ex.StatusCode == HttpStatusCode.BadRequest) {
				logger.LogError(ex, "Malformed input document. Request: {@Request}", request);
			} else {
				logger.LogError(ex, "Request {@Request}", request);
			}
		}
	}
}
