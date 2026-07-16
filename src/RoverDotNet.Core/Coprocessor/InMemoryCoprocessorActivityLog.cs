using System.Collections.Concurrent;

namespace RoverDotNet.Core.Coprocessor;

/// <summary>
/// Default in-memory <see cref="ICoprocessorActivityLog"/> implementation, keyed by
/// stage and (for subgraph stages) subgraph name.
/// </summary>
public sealed class InMemoryCoprocessorActivityLog : ICoprocessorActivityLog
{
    private readonly ConcurrentDictionary<string, CoprocessorActivityEntry> _entriesByKey = new();

    /// <inheritdoc/>
    public event EventHandler<CoprocessorActivityEntry>? EntryRecorded;

    /// <inheritdoc/>
    public void Record(CoprocessorActivityEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var key = GetKey(entry.Stage, entry.ServiceName);
        _entriesByKey[key] = entry;

        EntryRecorded?.Invoke(this, entry);
    }

    /// <inheritdoc/>
    public IReadOnlyList<CoprocessorActivityEntry> GetLatestEntries() =>
        _entriesByKey.Values
            .OrderByDescending(entry => entry.Timestamp)
            .ToList();

    private static string GetKey(CoprocessorStage stage, string? serviceName) =>
        serviceName is null ? stage.ToString() : $"{stage}::{serviceName}";
}
