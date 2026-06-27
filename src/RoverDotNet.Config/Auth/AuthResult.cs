namespace RoverDotNet.Config.Auth;

/// <summary>
/// The result of a <c>config auth</c> execution.
/// Mirrors the success output from <c>src/command/config/auth.rs</c>.
/// </summary>
/// <param name="ProfileName">The name of the profile that was authenticated.</param>
/// <param name="Message">A user-facing message describing the result.</param>
public sealed record AuthResult(
    string ProfileName,
    string Message);
