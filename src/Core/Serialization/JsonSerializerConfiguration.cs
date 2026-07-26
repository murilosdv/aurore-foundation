using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Aurore.Foundation.Core.Extensions;

namespace Aurore.Foundation.Core.Serialization;

/// <summary>
/// Provides the standard <see cref="JsonSerializerOptions"/> configuration used across Aurore applications,
/// along with convenience serialization/deserialization helpers built on top of it.
/// </summary>
public static class JsonSerializerConfiguration
{
    /// <summary>
    /// Creates a new <see cref="JsonSerializerOptions"/> based on <see cref="JsonSerializerDefaults.Web"/> and applies
    /// the standard Aurore configuration (see <see cref="Configure"/>).
    /// </summary>
    /// <returns>The configured <see cref="JsonSerializerOptions"/>.</returns>
    public static JsonSerializerOptions Default()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        Configure(options);

        return options;
    }

    /// <summary>
    /// Applies the standard Aurore JSON serialization conventions to <paramref name="options"/>: trailing commas,
    /// case-insensitive and camelCase property names, cycle-ignoring reference handling, relaxed escaping, and
    /// enums serialized as strings.
    /// </summary>
    /// <param name="options">The options instance to configure in place.</param>
    public static void Configure(JsonSerializerOptions options)
    {
        options.AllowTrailingCommas = true;
        options.PropertyNameCaseInsensitive = true;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.WriteIndented = false;
        options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.Converters.Clear();
        options.Converters.Add(new JsonStringEnumConverter());
    }

    /// <summary>
    /// Serializes an object to a JSON string using the standard Aurore configuration.
    /// </summary>
    /// <param name="source">The object to serialize.</param>
    /// <param name="optionsAction">An optional callback to further customize the serializer options.</param>
    /// <returns>The serialized JSON string.</returns>
    public static string Serialize(this object source, Action<JsonSerializerOptions>? optionsAction = null)
    {
        return JsonSerializer.Serialize(source, BuildOptions(optionsAction));
    }

    /// <summary>
    /// Serializes an object to JSON and writes it to a file, using indented formatting by default.
    /// </summary>
    /// <param name="source">The object to serialize.</param>
    /// <param name="filePath">The path of the file to write, normalized via <see cref="Extensions.PathExtensions.NormalizeRuntimePath(string[])"/>.</param>
    /// <param name="optionsAction">An optional callback to further customize the serializer options. If omitted, output is indented.</param>
    public static void SerializeToFile(this object source, string filePath, Action<JsonSerializerOptions>? optionsAction = null)
    {
        filePath = Path.NormalizeRuntimePath(filePath);

        File.WriteAllText(filePath, source.Serialize(optionsAction is null ? x => x.WriteIndented = true : optionsAction));
    }

    /// <summary>
    /// Asynchronously serializes <paramref name="target"/> as JSON and writes it to <paramref name="source"/>, using the standard Aurore configuration.
    /// </summary>
    /// <typeparam name="T">The type of the value being serialized.</typeparam>
    /// <param name="source">The stream to write the serialized JSON to.</param>
    /// <param name="target">The value to serialize.</param>
    /// <param name="optionsAction">An optional callback to further customize the serializer options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that completes when serialization has finished.</returns>
    public static async Task SerializeAsync<T>(this Stream source, T target, Action<JsonSerializerOptions>? optionsAction = null, CancellationToken cancellationToken = default)
    {
        await JsonSerializer.SerializeAsync(source, target, BuildOptions(optionsAction), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deserializes a JSON string into a <typeparamref name="T"/> instance using the standard Aurore configuration.
    /// </summary>
    /// <typeparam name="T">The type to deserialize into.</typeparam>
    /// <param name="source">The JSON string to deserialize.</param>
    /// <param name="optionsAction">An optional callback to further customize the serializer options.</param>
    /// <returns>The deserialized instance.</returns>
    public static T Deserialize<T>(this string source, Action<JsonSerializerOptions>? optionsAction = null)
    {
        return JsonSerializer.Deserialize<T>(source, BuildOptions(optionsAction))!;
    }

    /// <summary>
    /// Asynchronously deserializes JSON read from <paramref name="source"/> into a <typeparamref name="T"/> instance using the standard Aurore configuration.
    /// </summary>
    /// <typeparam name="T">The type to deserialize into.</typeparam>
    /// <param name="source">The stream to read JSON from.</param>
    /// <param name="optionsAction">An optional callback to further customize the serializer options.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The deserialized instance.</returns>
    public static async ValueTask<T> DeserializeAsync<T>(this Stream source, Action<JsonSerializerOptions>? optionsAction = null, CancellationToken cancellationToken = default)
    {
        return (await JsonSerializer.DeserializeAsync<T>(source, BuildOptions(optionsAction), cancellationToken).ConfigureAwait(false))!;
    }

    /// <summary>
    /// Reads and deserializes a JSON file into a <typeparamref name="T"/> instance using the standard Aurore configuration.
    /// </summary>
    /// <typeparam name="T">The type to deserialize into.</typeparam>
    /// <param name="filename">The path of the file to read.</param>
    /// <param name="throwOnError">When <see langword="true"/> (the default), throws if the file does not exist.</param>
    /// <returns>The deserialized instance.</returns>
    /// <exception cref="FileNotFoundException">Thrown when <paramref name="filename"/> does not exist and <paramref name="throwOnError"/> is <see langword="true"/>.</exception>
    public static T DeserializeFromFile<T>(this string filename, bool throwOnError = true)
    {
        if (File.Exists(filename) is false && throwOnError)
        {
            throw new FileNotFoundException($"The file '{filename}' was not found.");
        }

        return File.ReadAllText(filename).Deserialize<T>();
    }

    private static JsonSerializerOptions BuildOptions(Action<JsonSerializerOptions>? optionsAction)
    {
        var options = Default();

        optionsAction?.Invoke(options);

        return options;
    }
}
