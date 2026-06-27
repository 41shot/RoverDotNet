namespace RoverDotNet.Dev.Exceptions;

/// <summary>
/// Thrown when supergraph composition fails due to schema errors or incompatibilities.
/// </summary>
public sealed class CompositionFailedException : DevException
{
    /// <summary>
    /// Gets the list of composition errors reported by the composition tool.
    /// </summary>
    public IReadOnlyList<string> CompositionErrors { get; }

    /// <param name="errors">The list of composition errors.</param>
    public CompositionFailedException(IReadOnlyList<string> errors)
        : base($"Supergraph composition failed with {errors.Count} error(s).")
    {
        CompositionErrors = errors;
    }

    /// <param name="message">Custom error message.</param>
    /// <param name="errors">The list of composition errors.</param>
    public CompositionFailedException(string message, IReadOnlyList<string> errors)
        : base(message)
    {
        CompositionErrors = errors;
    }
}
