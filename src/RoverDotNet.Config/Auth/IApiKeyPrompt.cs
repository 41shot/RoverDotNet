namespace RoverDotNet.Config.Auth;

/// <summary>
/// Prompts the user to enter an API key.
/// Abstraction for testability; implementations may read from console, file, or test fixtures.
/// </summary>
public interface IApiKeyPrompt
{
    /// <summary>
    /// Displays instructions and prompts the user to enter an Apollo Studio API key.
    /// </summary>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    /// <returns>The API key entered by the user.</returns>
    /// <exception cref="OperationCanceledException">The operation was cancelled.</exception>
    Task<string> PromptAsync(CancellationToken cancellationToken = default);
}
