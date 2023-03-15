using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenPolicyAgent.Common.Client.Generics;

namespace OpenPolicyAgent.Common.Client.Responses;

public class WithResult<TResult> {
	// public properties

	public TResult? Result { get; set; }
	public Metrics? Metrics { get; set; }

	[JsonExtensionData]
	public Dictionary<string, JToken> ExtraProperties { get; set; } = new();
}
