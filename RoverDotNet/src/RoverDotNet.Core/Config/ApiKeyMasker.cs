namespace RoverDotNet.Core.Config;

/// <summary>
/// Masks an Apollo API key for safe display in logs and terminal output.
/// Mirrors <c>houston::mask_key()</c>.
/// </summary>
/// <remarks>
/// The first four and last four characters are kept visible; all characters
/// in between are replaced with <c>*</c>. Keys of eight characters or fewer
/// are returned unchanged (every character would already be in the visible range).
/// </remarks>
public static class ApiKeyMasker
{
    /// <summary>Returns a masked representation of <paramref name="apiKey"/>.</summary>
    public static string Mask(string apiKey)
    {
        var chars = new char[apiKey.Length];
        for (var i = 0; i < apiKey.Length; i++)
        {
            chars[i] = (i <= 3 || i >= apiKey.Length - 4) ? apiKey[i] : '*';
        }
        return new string(chars);
    }
}
