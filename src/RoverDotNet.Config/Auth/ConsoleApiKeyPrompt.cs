using RoverDotNet.Core.Exceptions;

namespace RoverDotNet.Config.Auth;

/// <summary>
/// Console-based implementation of <see cref="IApiKeyPrompt"/> that displays
/// instructions and reads a secure API key from the user via standard input.
/// Mirrors <c>api_key_prompt</c> in <c>src/command/config/auth.rs</c>.
/// </summary>
public sealed class ConsoleApiKeyPrompt : IApiKeyPrompt
{
    private const string AuthUrl = "https://go.apollo.dev/r/auth";

    /// <inheritdoc />
    public Task<string> PromptAsync(CancellationToken cancellationToken = default)
    {
        // Display instructions
        Console.Error.WriteLine($"Go to {AuthUrl} and create a new Personal API Key.");
        Console.Error.WriteLine("Copy the key and paste it into the prompt below.");
        Console.Error.Write("> ");

        // Read the API key securely (hiding input)
        var apiKey = ReadSecureLine(cancellationToken);

        // Validate the input
        return Task.FromResult(Validate(apiKey));
    }

    /// <summary>
    /// Reads a line from standard input with hidden characters (for password/key entry).
    /// </summary>
    private static string ReadSecureLine(CancellationToken cancellationToken)
    {
        var input = new System.Text.StringBuilder();

        while (!cancellationToken.IsCancellationRequested)
        {
            if (!Console.KeyAvailable)
            {
                Thread.Sleep(50);
                continue;
            }

            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.Error.WriteLine(); // Move to next line
                break;
            }

            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input.Length--;
                Console.Error.Write("\b \b"); // Erase character visually
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input.Append(key.KeyChar);
                Console.Error.Write("*"); // Show asterisk for each character
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        return input.ToString();
    }

    /// <summary>
    /// Validates the API key input.
    /// Mirrors <c>validate</c> in <c>src/command/config/auth.rs</c>.
    /// </summary>
    private static string Validate(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
            throw new RoverException("Received an empty API Key. Please try again.");

        // Detect the Windows Ctrl+V paste issue (byte 0x16)
        if (apiKey.Length == 1 && apiKey[0] == '\x16')
        {
            throw new RoverException(
                "Your API key was not pasted successfully. " +
                "Re-run this command, and when you are prompted to enter your API key, " +
                "right click on the terminal and press paste instead of pressing Ctrl+V.");
        }

        return apiKey;
    }
}
