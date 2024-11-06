# Leveraging Qdrant and Ollama for Efficient Text Embedding and Vector Search in .NET

This code demonstrates how to **generate embeddings** for text descriptions, **insert** those **embeddings** into a **Qdrant-based vector database**, and **retrieve** data based on an identifier. The **Hotel class** includes specific **annotations for Qdrant**, which specify how to store and index each field

For more information about this topic visit these links:

https://learn.microsoft.com/en-us/semantic-kernel/concepts/vector-store-connectors/vector-search

https://learn.microsoft.com/en-us/semantic-kernel/concepts/vector-store-connectors/out-of-the-box-connectors/qdrant-connector

## 1. Run Qdrant in a Docker container

We first have to create a **Vector Database** with **Qdrant** 

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

We run Visual Studio 2022 Community Edition and we Create a new Project

![image](https://github.com/user-attachments/assets/5270a5b0-5e23-4e7c-a254-5e30f4a3ce93)

We select the C# console project template and press the Next button

![image](https://github.com/user-attachments/assets/dd2df4c7-1557-4d23-a62f-e0cb8dbc2712)

We input the project name and location and press the Next button 

We select the .NET 9 framework and press the Create button

## 5. We load the Nuget packages

![image](https://github.com/user-attachments/assets/9b8982b9-c052-42c3-b1d7-6d81ad004f6c)

## 6. We explain the sample source code

The sample is this repo is focused on creating and interacting with a **vector database** using **Qdrant** and an **Ollama Embedding** service. It essentially follows these steps:

### 6.1. Initialization of Services and Clients

It starts by initializing an embedding generation service, **OllamaTextEmbeddingGenerationService**, with a specific model (**mxbai-embed-large**) and a local server endpoint

```csharp
ITextEmbeddingGenerationService olamaTES = new OllamaTextEmbeddingGenerationService(
    "mxbai-embed-large",
    new Uri("http://localhost:11434"));
```

A Qdrant client (**QdrantClient**) is set up to connect to a local instance of **Qdrant running on port 6334**, and a vector store (**QdrantVectorStore**) is created from this client

```csharp
var client = new QdrantClient("localhost", 6334);
var vectorStore = new QdrantVectorStore(client);
```

### 6.2. Collection Creation and Retrieval

The code tries to retrieve or create a collection called skhotels1 to store hotel data

This collection will store vector embeddings for fast similarity-based retrieval

If the collection doesn’t exist, it creates a new one using **CreateCollectionIfNotExistsAsync**

```csharp
Console.WriteLine("Attempting to retrieve or create collection...");
var collection = vectorStore.GetCollection<ulong, Hotel>("skhotels1");

// Create collection if it does not exist
await collection.CreateCollectionIfNotExistsAsync();
Console.WriteLine("Collection created or retrieved successfully.");
```

### 6.3. Embedding Generation

A description ("A happy place for everyone.") is prepared, and an embedding is generated for this text using the olamaTES.GenerateEmbeddingAsync method

The generated embedding, which is an array of floating-point numbers, represents a numerical vector encoding of the description text. If embedding generation fails, the program exits

```csharp
string descriptionText = "A happy place for everyone.";
var embedding = await olamaTES.GenerateEmbeddingAsync(descriptionText);
```

### 6.4. Record Creation and Insertion

A **Hotel record** is **created** with fields HotelId, HotelName, Description, and DescriptionEmbedding

This **record** is then **inserted** into the skhotels1 collection using UpsertAsync, which ensures that the record is added or updated if it already exists

```csharp
var hotelRecord = new Hotel
{
    HotelId = hotelId,
    HotelName = "Happy Hotel",
    Description = descriptionText,
    DescriptionEmbedding = embedding.ToArray()
};
await collection.UpsertAsync(hotelRecord);
```

### 6.5. Record Retrieval

After insertion, the record is retrieved from the collection using GetAsync based on the HotelId

If retrieval is successful, it prints out the retrieved hotel’s name and description

```csharp
var retrievedHotel = await collection.GetAsync(hotelId);
```

### 6.6. Exception Handling

If any errors occur during this process, they are caught and printed, including the stack trace for easier debugging

### 6.7. Hotel Class Definition

The Hotel class defines a basic structure for a hotel record with attributes like HotelId, HotelName, and Description

The attribute DescriptionEmbedding is specifically marked for storage as a 1024-dimensional vector with Euclidean distance for similarity matching

This embedding enables the database to perform vector searches (e.g., finding similar descriptions)

```csharp
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

### 6.8. Summary: This code demonstrates how to generate embeddings for text descriptions, insert those embeddings into a Qdrant-based vector database, and retrieve data based on an identifier

The Hotel class includes specific annotations for Qdrant, which specify how to store and index each field

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

