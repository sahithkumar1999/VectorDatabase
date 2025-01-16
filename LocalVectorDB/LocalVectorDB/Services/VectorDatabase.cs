using LiteDB;
using LocalVectorDB.Models;
using MathNet.Numerics.LinearAlgebra;

namespace LocalVectorDB.Services
{
    /// <summary>
    /// Manages vector storage, retrieval, and similarity search operations.
    /// </summary>
    public class VectorDatabase
    {
        private readonly LiteDatabase _db;  // LiteDB instance for lightweight local storage
        private readonly ILiteCollection<VectorItem> _collection;  // Collection of stored vectors

        /// <summary>
        /// Initializes the vector database with a LiteDB instance.
        /// </summary>
        /// <param name="dbPath">Path to the LiteDB database file.</param>
        public VectorDatabase(string dbPath = "VectorDB.db")
        {
            _db = new LiteDatabase(dbPath);  // Open or create the database file
            _collection = _db.GetCollection<VectorItem>("vectors");  // Get or create the 'vectors' collection
        }

        /// <summary>
        /// Inserts a new vector into the database.
        /// </summary>
        /// <param name="item">VectorItem object containing the vector data.</param>
        public void InsertVector(VectorItem item) => _collection.Insert(item);  // Insert vector into the collection

        /// <summary>
        /// Searches for the top N vectors most similar to the query vector using cosine similarity.
        /// </summary>
        /// <param name="queryVector">The vector to compare against stored vectors.</param>
        /// <param name="topN">The number of top similar vectors to retrieve.</param>
        /// <returns>A list of the most similar VectorItems.</returns>
        public List<VectorItem> SearchSimilar(Vector<double> queryVector, int topN)
        {
            var results = new List<(VectorItem item, double similarity)>();  // List to store similarity results

            foreach (var item in _collection.FindAll())  // Iterate through all stored vectors
            {
                var storedVector = Vector<double>.Build.DenseOfEnumerable(item.Vector);  // Convert stored list to vector
                double similarity = queryVector.DotProduct(storedVector) / (queryVector.L2Norm() * storedVector.L2Norm());  // Compute cosine similarity
                results.Add((item, similarity));  // Add the vector and its similarity score to the results
            }

            results.Sort((a, b) => b.similarity.CompareTo(a.similarity));  // Sort results by descending similarity
            return results.Take(topN).Select(x => x.item).ToList();  // Return the top N similar vectors
        }

        /// <summary>
        /// Updates an existing vector in the database by ID.
        /// </summary>
        /// <param name="id">ID of the vector to update.</param>
        /// <param name="newVector">The new vector data.</param>
        public void UpdateVector(int id, List<double> newVector)
        {
            var item = _collection.FindById(id);  // Find the vector by ID
            if (item != null)
            {
                item.Vector = newVector;  // Update the vector data
                _collection.Update(item);  // Save changes back to the database
            }
        }

        /// <summary>
        /// Retrieves all stored vectors from the database.
        /// </summary>
        /// <returns>A list of all VectorItems.</returns>
        public List<VectorItem> GetAllVectors()
        {
            var vectors = _collection.FindAll().ToList();  // Fetch all vectors from the database
            Console.WriteLine($"Total vectors in DB: {vectors.Count}");  // Log the total count
            return vectors;  // Return the list of vectors
        }

        /// <summary>
        /// Deletes a vector from the database by its ID.
        /// </summary>
        /// <param name="id">ID of the vector to delete.</param>
        public void DeleteVector(int id) => _collection.Delete(id);  // Delete the vector by ID
    }
}
