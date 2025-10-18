using G16_Catalog.Models;

namespace G16_Catalog
{
    internal class Program
    {
        public static IDictionary<Category, ICollection<Product>> Categories { get; private set; }

        static void Main(string[] args)
        {
            var CategoryDataImporter = new CategoryDataImporter(Categories);
            CategoryDataImporter.InsertUpdateDelete();
            foreach (var category in Categories)
            {
                Console.WriteLine(category);
                foreach (var item in Categories)
                {
                    Console.WriteLine("  " + item);
                }
                Console.WriteLine($"{category.Key}");
                foreach (var item in category.Value)
                {
                    Console.WriteLine($"   {item}");
                }
            }
            CatalogFileReader reader = new("Data\\Products.csv");

            var categories = reader.ReadData();
            foreach (var category in categories)
            {
                Console.WriteLine($"Category: {category.Name}, Active: {category.IsActive}");
            }
        }
    }
}