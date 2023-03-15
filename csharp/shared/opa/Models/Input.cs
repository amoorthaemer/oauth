namespace OpenPolicyAgent.Common.Models;

public sealed class Input {
	// public properties

	public User? User { get; set; } = null;
	public string? Resource { get; set; } = null;

    // constructor

    public Input() { }

    public Input(User user, string? resouce = null) {
        User = user;
        Resource = resouce;
    }
}
