# qdrant_sample3

https://learn.microsoft.com/en-us/semantic-kernel/concepts/vector-store-connectors/vector-search

https://learn.microsoft.com/en-us/semantic-kernel/concepts/vector-store-connectors/out-of-the-box-connectors/qdrant-connector

## 1. Run qdrant in a Docker container

We first have to create a **Vector Database** with **qdrant** 

We first have to view the user folder name and set in in the command:

![image](https://github.com/user-attachments/assets/f1ec286f-9c8d-4277-85e0-fee34451ac8f)

Now we execute the command to run qdrant docker container

```
docker run -p 6333:6333 -p 6334:6334 ^
-v C:\Users\luisc\luiscocoenriquezvector\qdrant_storage:/qdrant/storage:z ^
qdrant/qdrant
```

## 2. How to run an Ollama Docker container

```
docker run -d -v ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama
```

![image](https://github.com/user-attachments/assets/9bc2d6f4-91e0-4a7c-b97a-04c5caf51eed)

We also verify in Docker Desktop

![image](https://github.com/user-attachments/assets/2d7a12f1-92ac-495c-b70e-8eeceb2bf443)

![image](https://github.com/user-attachments/assets/a695d24d-271e-4d22-aa06-6007a24474b8)

## 3. How to install the "mxbai-embed-large" model in the Ollama Docker container

![image](https://github.com/user-attachments/assets/0cfe41e2-f653-4936-a48e-19bea6844757)

Access the Running Container:  First, ensure your Ollama container is running. If it's named ollama, you can access its shell using:

```
docker exec -it ollama /bin/sh
```

Now we have to intall the **mxbai-embed-large** Model inside the container. We use the ollama pull command to download the model:

```
ollama pull mxbai-embed-large
```

we verify the installation

```
ollama list
```

We also verify the Ollama docker container is running in **Docker Desktop**

![image](https://github.com/user-attachments/assets/af7de325-f8d4-4a4d-b8a3-1c24b9538db4)

## 4. We create a new C# console application with Visual Studio Community Edition 2022 and .NET 9


## 5. We load the Nuget packages

![image](https://github.com/user-attachments/assets/9b8982b9-c052-42c3-b1d7-6d81ad004f6c)

## 6. We explain the sample source code

```csharp
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
```

## 7. We run the application and verify the results

![image](https://github.com/user-attachments/assets/9e5664d4-a756-4d75-a7de-f1f49595bb5b)

