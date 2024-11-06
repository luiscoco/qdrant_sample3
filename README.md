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

## 3. How to install the "phi3:latest" model in the Ollama Docker container

Access the Running Container:  First, ensure your Ollama container is running. If it's named ollama, you can access its shell using:

```
docker exec -it ollama /bin/sh
```

Now we have to intall the **phi3:latest** Model inside the container. We use the ollama pull command to download the model:

```
ollama pull phi3:latest
```

we verify the installation

```
ollama list
```

We also verify the Ollama docker container is running in **Docker Desktop**

## 4. 

