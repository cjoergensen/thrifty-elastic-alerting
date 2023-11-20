using System.Text.Json.Serialization;

namespace ThriftyElasticAlerting.Model;

[JsonSerializable(typeof(Document))]
public partial class DocumentContext : JsonSerializerContext { }