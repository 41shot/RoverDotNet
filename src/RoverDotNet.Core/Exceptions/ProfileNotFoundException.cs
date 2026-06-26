namespace RoverDotNet.Core.Exceptions;

/// <summary>
/// Thrown when a configuration profile with the specified name cannot be found.
/// Mirrors <c>HoustonProblem::ProfileNotFound</c>.
/// </summary>
public sealed class ProfileNotFoundException : RoverException
{
    /// <summary>The name of the profile that was not found.</summary>
    public string ProfileName { get; }

    /// <inheritdoc />
    public ProfileNotFoundException(string profileName)
        : base($"There is no profile named '{profileName}'.")
    {
        ProfileName = profileName;
    }
}
