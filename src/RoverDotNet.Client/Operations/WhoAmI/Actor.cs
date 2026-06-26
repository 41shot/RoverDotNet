namespace RoverDotNet.Client.Operations.WhoAmI;

/// <summary>
/// The type of Apollo API key actor.
/// Mirrors <c>rover-client::operations::config::who_am_i::Actor</c>.
/// </summary>
public enum Actor
{
    /// <summary>The key belongs to a graph (service key).</summary>
    Graph,

    /// <summary>The key belongs to a user (personal API key).</summary>
    User,

    /// <summary>The key actor type is not recognised by this client.</summary>
    Other
}
