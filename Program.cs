using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Embeddings;
using Qdrant.Client;
using System;
using System.Threading.Tasks;

#pragma warning disable
ITextEmbeddingGenerationService olamaTES = new OllamaTextEmbeddingGenerationService(
    "mxbai-embed-large",
    new Uri("http://localhost:11434"));

// Initialize Qdrant client and VectorStore
var client = new QdrantClient("localhost", 6334);
var vectorStore = new QdrantVectorStore(client);

try
{
    Console.WriteLine("Attempting to retrieve or create collection...");
    var collection = vectorStore.GetCollection<ulong, Hotel>("skhotels1");

    // Create collection if it does not exist
    await collection.CreateCollectionIfNotExistsAsync();
    Console.WriteLine("Collection created or retrieved successfully.");

    string descriptionText = "A happy place for everyone.";
    ulong hotelId = 1;

    Console.WriteLine("Generating embedding...");
    var embedding = await olamaTES.GenerateEmbeddingAsync(descriptionText);

    if (embedding.IsEmpty)
    {
        Console.WriteLine("Embedding generation failed or returned an empty result.");
        return;
    }
    Console.WriteLine($"Embedding generated successfully with length: {embedding.Length}");

    // Ensure the embedding dimensions match the attribute specification in Hotel class
    var hotelRecord = new Hotel
    {
        HotelId = hotelId,
        HotelName = "Happy Hotel",
        Description = descriptionText,
        DescriptionEmbedding = embedding.ToArray()
    };

    Console.WriteLine("Attempting to insert the record into the collection...");
    await collection.UpsertAsync(hotelRecord);
    Console.WriteLine("Record inserted successfully.");

    Console.WriteLine("Attempting to fetch the record...");
    var retrievedHotel = await collection.GetAsync(hotelId);

    if (retrievedHotel != null)
    {
        Console.WriteLine($"Hotel Retrieved: {retrievedHotel.HotelName}, Description: {retrievedHotel.Description}");
    }
    else
    {
        Console.WriteLine("Failed to retrieve the record after insertion.");
    }
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred: " + ex.Message);
    Console.WriteLine("Stack Trace: " + ex.StackTrace);
}

// Basic hotel class definition
public class Hotel
{
    [VectorStoreRecordKey]
    public ulong HotelId { get; set; }

    [VectorStoreRecordData(IsFilterable = true, StoragePropertyName = "hotel_name")]
    public string HotelName { get; set; }

    [VectorStoreRecordData(IsFullTextSearchable = true, StoragePropertyName = "hotel_description")]
    public string Description { get; set; }

    [VectorStoreRecordVector(1024, DistanceFunction.EuclideanDistance, IndexKind.Hnsw, StoragePropertyName = "hotel_description_embedding")]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}
