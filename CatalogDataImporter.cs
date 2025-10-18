using G16_Catalog.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace G16_Catalog
{
    public class CategoryDataImporter
    {
        private string _connectionString = @"server=JUNIOR; database=Categorys; integrated security = true";
        private readonly IDictionary<Category, ICollection<Product>> _catalogue;

        public CategoryDataImporter(IDictionary<Category, ICollection<Product>> catalogue)
        {
            _catalogue = catalogue;
        }

        public void InsertUpdateDelete()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand categoryCommand = connection.CreateCommand();
                    SqlCommand productCommand = connection.CreateCommand();

                    categoryCommand.Transaction = transaction;
                    productCommand.Transaction = transaction;

                    var code = categoryCommand.Parameters.Add("@CategoryCode", SqlDbType.VarChar);
                    var name = categoryCommand.Parameters.Add("@CategoryName", SqlDbType.NVarChar);
                    var isDeleted = categoryCommand.Parameters.Add("@CategoryIsDeleted", SqlDbType.Bit);

                    var prCategoryId = productCommand.Parameters.Add("@CategoryID", SqlDbType.Int);
                    var prCode = productCommand.Parameters.Add("@ProductCode", SqlDbType.VarChar);
                    var prName = productCommand.Parameters.Add("@ProductName", SqlDbType.NVarChar);
                    var prIsDeleted = productCommand.Parameters.Add("@ProductIsDeleted", SqlDbType.Bit);
                    var prPrice = productCommand.Parameters.Add("@ProductPrice", SqlDbType.Money);

                    foreach (var item in _catalogue)
                    {
                        categoryCommand.CommandText =
                            "INSERT INTO dbo.Categories (Code, Name, IsDeleted) " +
                            "VALUES (@CategoryCode, @CategoryName, @CategoryIsDeleted); " +
                            "SELECT SCOPE_IDENTITY();";

                        code.Value = item.Key.Code;
                        name.Value = item.Key.Name;
                        isDeleted.Value = item.Key.IsDeleted;

                        var lastInsertId = Convert.ToInt32(categoryCommand.ExecuteScalar());

                        productCommand.CommandText =
                            "INSERT INTO dbo.Products (CategoryID, Code, Name, Price, IsDeleted) " +
                            "VALUES (@CategoryID, @ProductCode, @ProductName, @ProductPrice, @ProductIsDeleted)";

                        foreach (var product in item.Value)
                        {
                            prCategoryId.Value = lastInsertId;
                            prCode.Value = product.Code;
                            prName.Value = product.Name;
                            prPrice.Value = product.Price;
                            prIsDeleted.Value = product.IsDeleted;

                            productCommand.ExecuteNonQuery();
                        }

                        categoryCommand.CommandText =
                            "UPDATE dbo.Categories SET Name = @CategoryName WHERE CategoryID = @CategoryID";

                        categoryCommand.Parameters.Clear();
                        categoryCommand.Parameters.Add("@CategoryID", SqlDbType.Int).Value = lastInsertId;
                        categoryCommand.Parameters.Add("@CategoryName", SqlDbType.NVarChar).Value = item.Key.Name + " (Updated)";

                        categoryCommand.ExecuteNonQuery();

                        productCommand.CommandText =
                            "DELETE FROM dbo.Products WHERE CategoryID = @CategoryID AND Price = 0";

                        productCommand.Parameters.Clear();
                        productCommand.Parameters.Add("@CategoryID", SqlDbType.Int).Value = lastInsertId;

                        productCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("შეცდომა: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}