using G16_Catalog.Models;

namespace G16_Catalog;

public class CatalogFileReader
{
    private readonly string _filePath;

    public CatalogFileReader(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        if (!File.Exists(filePath))
            throw new FileNotFoundException("The specified file was not found.", filePath);

        _filePath = filePath;
    }

    public IEnumerable<Category> ReadData()
    {
        using var reader = new StreamReader(_filePath);
        var categories = new List<Category>();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var parts = line!.Split('\t');

            var category = new Category
            {
                Name = parts[0],
                IsActive = parts[1] == "1"
            };

            int index = categories.IndexOf(category);
            if (index == -1)
                categories.Add(category);
            else
                categories[index].IsActive = category.IsActive;
        }

        return categories;
    }
}