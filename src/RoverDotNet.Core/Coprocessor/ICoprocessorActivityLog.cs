namespace RoverDotNet.Core.Coprocessor;

/// <summary>
/// A single recorded coprocessor invocation: the payload the router sent, and the
/// (possibly modified) payload returned to it.
/// </summary>
/// <param name="Stage">The request-handling lifecycle stage.</param>
/// <param name="ServiceName">The subgraph name, for subgraph stages; otherwise <see langword="null"/>.</param>
/// <param name="Timestamp">When the coprocessor request was received.</param>
/// <param name="Request">The payload received from the router.</param>
/// <param name="Response">The payload returned to the router.</param>
public sealed record CoprocessorActivityEntry(
    CoprocessorStage Stage,
    string? ServiceName,
    DateTimeOffset Timestamp,
    CoprocessorPayload Request,
    CoprocessorPayload Response);

/// <summary>
/// Tracks the most recent coprocessor request/response for each stage (and, for subgraph
/// stages, each subgraph). Intended primarily to power diagnostic/demo UIs.
/// </summary>
public interface ICoprocessorActivityLog
{
    /// <summary>Raised whenever a new entry is recorded.</summary>
    event EventHandler<CoprocessorActivityEntry>? EntryRecorded;

    /// <summary>Records a completed coprocessor invocation.</summary>
    void Record(CoprocessorActivityEntry entry);

    /// <summary>
    /// Gets the most recent entry for each distinct stage/subgraph combination,
    /// ordered by <see cref="CoprocessorActivityEntry.Timestamp"/> descending.
    /// </summary>
    IReadOnlyList<CoprocessorActivityEntry> GetLatestEntries();
}
