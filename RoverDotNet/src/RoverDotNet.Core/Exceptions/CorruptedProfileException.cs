namespace RoverDotNet.Core.Exceptions;

/// <summary>
/// Thrown when a stored API key is corrupt (e.g. written by an older Rover version on Windows).
/// Mirrors <c>HoustonProblem::CorruptedProfile</c>.
/// </summary>
public sealed class CorruptedProfileException : RoverException
{
    /// <summary>The name of the profile whose key is corrupt.</summary>
    public string ProfileName { get; }

    /// <inheritdoc />
    public CorruptedProfileException(string profileName)
        : base($"The API key associated with profile '{profileName}' is corrupt.")
    {
        ProfileName = profileName;
    }
}
