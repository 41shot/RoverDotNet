using RoverDotNet.Core.Exceptions;

namespace RoverDotNet.Client.Http;

/// <summary>
/// Thrown when Apollo Studio returns one or more GraphQL errors in the response body.
/// </summary>
public sealed class StudioClientException : RoverException
{
    /// <summary>The errors returned by the Studio API.</summary>
    public IReadOnlyList<GraphQLError> Errors { get; }

    /// <inheritdoc />
    public StudioClientException(IReadOnlyList<GraphQLError> errors)
        : base(BuildMessage(errors))
    {
        Errors = errors;
    }

    private static string BuildMessage(IReadOnlyList<GraphQLError> errors)
        => string.Join("; ", errors.Select(e => e.Message));
}
