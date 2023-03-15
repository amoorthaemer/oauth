namespace OpenPolicyAgent.Common.Client.Generics;

public class WithInput<TInput> {
	// public properties

	public TInput? Input {  get; set; }


    public WithInput() { }

    public WithInput(TInput input) =>
        Input = input;
}
