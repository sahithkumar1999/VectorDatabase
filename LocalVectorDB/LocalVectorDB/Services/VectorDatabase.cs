using LiteDB;
using LocalVectorDB.Models;
using MathNet.Numerics.LinearAlgebra;

namespace LocalVectorDB.Services
{
    public class VectorDatabase
    {
        private readonly LiteDatabase _db;
        private readonly ILiteCollection<VectorItem> _collection;

        public VectorDatabase(string dbPath = "VectorDB.db")
        {
            _db = new LiteDatabase(dbPath);
            _collection = _db.GetCollection<VectorItem>("vectors");
        }

        public void InsertVector(VectorItem item) => _collection.Insert(item);

        public List<VectorItem> SearchSimilar(Vector<double> queryVector, int topN)
        {
            var results = new List<(VectorItem item, double similarity)>();

            foreach (var item in _collection.FindAll())
            {
                var storedVector = Vector<double>.Build.DenseOfEnumerable(item.Vector);
                double similarity = queryVector.DotProduct(storedVector) / (queryVector.L2Norm() * storedVector.L2Norm());
                results.Add((item, similarity));
            }

            results.Sort((a, b) => b.similarity.CompareTo(a.similarity));
            return results.Take(topN).Select(x => x.item).ToList();
        }

        public void UpdateVector(int id, List<double> newVector)
        {
            var item = _collection.FindById(id);
            if (item != null)
            {
                item.Vector = newVector;
                _collection.Update(item);
            }
        }
        // Retrieve all stored vectors
        public List<VectorItem> GetAllVectors()
        {
            var vectors = _collection.FindAll().ToList();
            Console.WriteLine($"Total vectors in DB: {vectors.Count}");
            return vectors;
        }
        public void DeleteVector(int id) => _collection.Delete(id);
    }
}
