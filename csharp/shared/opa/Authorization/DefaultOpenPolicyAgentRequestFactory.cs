using System.Security.Claims;
using Microsoft.Extensions.Options;
using OpenPolicyAgent.Common.Client.Generics;
using OpenPolicyAgent.Common.Models;
using OpenPolicyAgent.Common.Services;

namespace OpenPolicyAgent.Common.Authorization;

public class DefaultOpenPolicyAgentRequestFactory: IOpenPolicyAgentRequestFactory {
    // private fields

    private readonly OpenPolicyAgentOptions options;

    // constructor

    public DefaultOpenPolicyAgentRequestFactory(IOptions<OpenPolicyAgentOptions> options) =>
        this.options = options.Value;

    // public methods

    public virtual WithInput<Input> CreateRequest(ClaimsPrincipal user) =>
        new(new(User.FromPrincipal(user, options)));

    public WithInput<Input> CreateRequest(ClaimsPrincipal user, string? resource) =>
        new(new(User.FromPrincipal(user, options), resource));
}
