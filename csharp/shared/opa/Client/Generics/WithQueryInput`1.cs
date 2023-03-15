namespace OpenPolicyAgent.Common.Client.Generics;
public class WithQueryInput<TInput>: WithInput<TInput> {
	// public properties

	public string? Query { get; set; }
}
