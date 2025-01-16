namespace LocalVectorDB.Models
{
    // Supporting Models
    public record SearchRequest(List<double> QueryVector, int TopK);

    public class VectorItem
    {
        public int Id { get; set; }
        public List<double> Vector { get; set; }
    }
}
