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


