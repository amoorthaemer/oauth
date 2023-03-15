using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenPolicyAgent.Common.Client.Responses;

public class GetPolicyResponse {
	// public properties

	public string? Id { get; set; }
	public string? Raw { get; set; }

	[JsonExtensionData]
	public Dictionary<string, JToken> ExtraProperties { get; set; } = new();

}
