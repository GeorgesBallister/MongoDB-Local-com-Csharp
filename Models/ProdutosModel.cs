using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace APIMongoDB.Models;

public class ProdutosModel // Construtor
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Name")]
    public string? Name { get; set; } = null;
}