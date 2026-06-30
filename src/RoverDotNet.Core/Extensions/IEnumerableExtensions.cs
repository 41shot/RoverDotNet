namespace RoverDotNet.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for IEnumerable&lt;T&gt; types.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Converts an enumerable of key-value pairs into a string representation suitable for command-line parameters.
        /// Source pairs with null values are omitted, and string values that are empty are represented by just the key.
        /// </summary>
        public static string ToParametersString(this IEnumerable<KeyValuePair<string, object?>> source)
        {
            return string.Join(" ", source
                .Where(kv => kv.Value != null)
                .Select(kv => kv.Value is string s && string.IsNullOrEmpty(s)
                    ? kv.Key
                    : $"{kv.Key} \"{kv.Value}\""));
        }
    }
}
