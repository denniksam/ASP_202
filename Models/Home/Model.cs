namespace ASP_202.Models.Home
{
    public class Model
    {
        public String Header { get; set; } = null!;
        public String Title { get; set; } = null!;
        public List<String> Departments { get; set; } = null!;
        public List<Product> Products { get; set; } = null!;
    }

    public class Product
    {
        public String Name { get; set; } = null!;
        public Double Price { get; set; }
    }
}
