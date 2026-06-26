namespace RoverDotNet.Core.Credentials;

/// <summary>
/// Describes where a credential (API key) was sourced from.
/// Mirrors <c>houston::CredentialOrigin</c>.
/// </summary>
public abstract record CredentialOrigin
{
    private CredentialOrigin() { }

    /// <summary>The credential was read from the <c>APOLLO_KEY</c> environment variable.</summary>
    public sealed record EnvVar : CredentialOrigin;

    /// <summary>The credential was loaded from the named configuration profile on disk.</summary>
    public sealed record ConfigFile(string ProfileName) : CredentialOrigin;
}
