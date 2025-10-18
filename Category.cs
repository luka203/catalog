namespace G16_Catalog.Models;

public class Category
{
    public string? Name { get; set; }
    public string Code { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<Product>? Products { get; set; }

    public override bool Equals(object? obj)
    {
      return obj is Category category &&
             Code == category.Code;
    }
    public override int GetHashCode()
    {
      return HashCode.Combine(Code);
    }
    public override string? ToString()
    {
        return $"({Code}) {Name}";
    }
}