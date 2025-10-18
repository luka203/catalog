namespace G16_Catalog;

public class Product
{
    public string Code { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool IsDeleted { get; set; }
    public override string ToString()
    {
        return $"({Code}) {Name} - {Price:0.00}";
    }
}