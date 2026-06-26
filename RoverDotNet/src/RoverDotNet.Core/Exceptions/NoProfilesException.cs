namespace RoverDotNet.Core.Exceptions;

/// <summary>
/// Thrown when no configuration profiles exist at all.
/// Mirrors <c>HoustonProblem::NoConfigProfiles</c>.
/// </summary>
public sealed class NoProfilesException : RoverException
{
    /// <inheritdoc />
    public NoProfilesException()
        : base("No configuration profiles were found, and this command requires one.") { }
}
