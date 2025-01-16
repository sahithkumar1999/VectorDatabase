
# 📦 Local Vector Database API

A **lightweight vector database** designed for efficient storage, retrieval, and similarity search of high-dimensional data, optimized for **LLM (Large Language Model)** use cases.

---

## 🚀 Features

- **Insert Vectors**: Store individual or batch vectors.
- **View All Vectors**: Retrieve all stored vectors.
- **Similarity Search**: Find the top `K` most similar vectors using **Cosine Similarity**.
- **Update & Delete Vectors**: Modify or remove stored vectors by ID.
- **Cosine Similarity Check**: Compare two vectors without saving them.
- **REST API**: Simple and intuitive API endpoints for easy integration.

---

## 📂 Project Structure

```
LocalVectorDB/
├── Models/
│   └── VectorItem.cs          # Data model for vector storage
├── Services/
│   └── VectorDatabase.cs      # Core logic for vector storage and search
├── Program.cs                 # API routing and endpoint definitions
├── VectorDB.db                # LiteDB storage file
└── README.md                  # Project documentation
```

---

## ⚙️ Installation & Setup

### 1. **Clone the Repository**
```bash
git clone https://github.com/sahithkumar1999/VectorDatabase.git
cd VectorDatabase/LocalVectorDB/LocalVectorDB/
```

### 2. **Install Dependencies**
```bash
dotnet add package LiteDB
dotnet add package MathNet.Numerics
```

### 3. **Run the Project**
```bash
dotnet run
```

- API runs by default on:
  - **HTTP:** `http://localhost:5227`
  - **HTTPS:** `https://localhost:7047`

---

## 🌐 API Endpoints

### 🔹 **1. Insert a Single Vector**
- **Endpoint:** `POST /insert`
- **Headers:** `Content-Type: application/json`
- **Body:**
```json
[0.5, 0.8, 0.3, 0.9]
```
- **Command Line (cURL):**
```bash
curl -X POST http://localhost:5227/insert -H "Content-Type: application/json" -d "[0.5, 0.8, 0.3, 0.9]"
```

### 🔹 **2. Batch Insert Vectors**
- **Endpoint:** `POST /insert-batch`
- **Headers:** `Content-Type: application/json`
- **Body:**
```json
[
  [0.5, 0.8, 0.3, 0.9],
  [0.1, 0.2, 0.3, 0.4]
]
```
- **Command Line (cURL):**
```bash
curl -X POST http://localhost:5227/insert-batch -H "Content-Type: application/json" -d "[[0.5, 0.8, 0.3, 0.9], [0.1, 0.2, 0.3, 0.4]]"
```

### 🔹 **3. View All Stored Vectors**
- **Endpoint:** `GET /vectors`
- **Command Line (cURL):**
```bash
curl http://localhost:5227/vectors
```
- **Postman Setup:**
  1. **Method:** `GET`
  2. **URL:** `http://localhost:5227/vectors`
  3. **Click:** **Send**

### 🔹 **4. Search for Similar Vectors (KNN Search)**
- **Endpoint:** `POST /search`
- **Headers:** `Content-Type: application/json`
- **Body:**
```json
{
  "QueryVector": [0.5, 0.8, 0.3, 0.9],
  "TopK": 2
}
```
- **Command Line (cURL):**
```bash
curl -X POST http://localhost:5227/search -H "Content-Type: application/json" -d "{"QueryVector": [0.5, 0.8, 0.3, 0.9], "TopK": 2}"
```

### 🔹 **5. Update a Vector**
- **Endpoint:** `PUT /update/{id}`
- **Headers:** `Content-Type: application/json`
- **Body:**
```json
[0.9, 0.8, 0.7, 0.6]
```
- **Command Line (cURL):**
```bash
curl -X PUT http://localhost:5227/update/1 -H "Content-Type: application/json" -d "[0.9, 0.8, 0.7, 0.6]"
```

### 🔹 **6. Delete a Vector**
- **Endpoint:** `DELETE /delete/{id}`
- **Command Line (cURL):**
```bash
curl -X DELETE http://localhost:5227/delete/1
```

### 🔹 **7. Cosine Similarity Check**
- **Endpoint:** `POST /cosine-similarity`
- **Headers:** `Content-Type: application/json`
- **Body:**
```json
[
  [1, 0, 0],
  [0, 1, 0]
]
```
- **Command Line (cURL):**
```bash
curl -X POST http://localhost:5227/cosine-similarity -H "Content-Type: application/json" -d "[[1, 0, 0], [0, 1, 0]]"
```

---

## 🤝 **Contributing**

1. Fork the repository.
2. Create a new branch (`feature/your-feature`).
3. Commit your changes.
4. Push to the branch.
5. Open a **Pull Request**.

---

**Happy Coding! 🚀**
