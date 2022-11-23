namespace UserAuthTestA.Model
{
    public class Unit
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Plates { get; set; }
        public string Model { get; set; }
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
    }
}
