# qdrant_sample3

## 1. Run qdrant in a Docker container

We first have to create a **Vector Database** with **qdrant** 

We first have to view the user folder name and set in in the command:

![image](https://github.com/user-attachments/assets/f1ec286f-9c8d-4277-85e0-fee34451ac8f)

Now we execute the command to run qdrant docker container

```
docker run -p 6333:6333 - p 6334:6334 ^
-v C:\\Users\\luisc\\luiscocoenriquezvector\\qdrant_storage:/ qdrant / storage:z ^
qdrant / qdrant
```

## 2. Create Ollama phi3:latest 

We download and run the Ollama AI engine in our local laptop

We also download the latest phi3 image

## 3. We 
