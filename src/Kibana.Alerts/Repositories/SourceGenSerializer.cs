using Kibana.Alerts.Model;
using Elasticsearch.Net;
using System.Text.Json;

namespace Kibana.Alerts.Repositories;

public class SourceGenSerializer : IElasticsearchSerializer
{
    private readonly JsonSerializerOptions options = new()
    {
            TypeInfoResolver = DocumentContext.Default
    };

    public object Deserialize(Type type, Stream stream) => JsonSerializer.Deserialize(stream, type, options) ?? throw new Exception("Unable to deserialize stream");

    public T Deserialize<T>(Stream stream) => JsonSerializer.Deserialize<T>(stream, options) ?? throw new Exception("Unable to deserialize stream");

    public async Task<object> DeserializeAsync(Type type, Stream stream, CancellationToken cancellationToken = default)
    {
        var result = await JsonSerializer.DeserializeAsync(stream, type, options, cancellationToken);
        return result ?? throw new Exception("Unable to deserialize stream");
    }

    public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        var result = await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);
        return result ?? throw new Exception("Unable to deserialize stream");
    }

    public void Serialize<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.None) => JsonSerializer.Serialize<T>(stream, data, options);

    public Task SerializeAsync<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.None, CancellationToken cancellationToken = default) => JsonSerializer.SerializeAsync<T>(stream, data, options, cancellationToken);
}
