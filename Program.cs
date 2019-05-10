using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NorthwindConsole.Models;

namespace NorthwindConsole
{
    
    class MainClass
    {
        public const int archive = 22;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

         public static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                string choice;
                do
                {
                    Console.WriteLine();
                    Console.WriteLine(" 1) Display Product");
                    Console.WriteLine(" 2) Add Product");
                    Console.WriteLine(" 3) Edit Product");
                    Console.WriteLine(" 4) Delete Product");
                    Console.WriteLine(" 5) Display Category");
                    Console.WriteLine(" 6) Add Category");
                    Console.WriteLine(" 7) Edit Category");
                    Console.WriteLine(" 8) Delete Category");
                    Console.WriteLine(" \"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    
                    logger.Info($"Option {choice} selected");
                    Console.WriteLine();
                    if (choice == "1")  // Display Product
                    {
                        string displayChoice = "";
                        // display choices to user
                        Console.WriteLine(" 1) Display Active Products " +
                                          "\n 2) Display Discontinued Products " +
                                          "\n 3) Display All Products " +
                                          "\n 4) Display Product Detail");
                        Console.WriteLine(" Enter to quit");
                        // input selection
                        displayChoice = Console.ReadLine().ToUpper();
                        Console.Clear();
                        logger.Info($"Display choice: {displayChoice} selected");
                        Console.WriteLine();

                        var db = new NorthwindContext();
                        if (displayChoice == "1")  // Display Active Products
                        {
                            var query = db.Products.OrderBy(p => p.ProductName).Where(b => !b.Discontinued);
                            foreach (var item in query)
                            {
                                Console.WriteLine("  " + item.ProductName);
                            }
                            Console.WriteLine($"\n{query.Count()} active records returned");
                            logger.Info($"{query.Count()} active records returned");
                        }
                        else if (displayChoice == "2") // Display Discontinued Products
                        {
                            var query = db.Products.OrderBy(p => p.ProductName).Where(b => b.Discontinued);
                            foreach (var item in query)
                            {
                                Console.WriteLine("  " + item.ProductName);
                            }
                            Console.WriteLine($"\n{query.Count()} discontinued records returned");
                            logger.Info($"{query.Count()} discontinued records returned");
                        }
                        else if (displayChoice == "3") // Display All Products
                        {
                            var query = db.Products.OrderBy(p => p.ProductName);
                            foreach (var item in query)
                            {
                                Console.WriteLine("  " + item.ProductName);
                            }
                            Console.WriteLine($"\n{query.Count()} total records returned");
                            logger.Info($"{query.Count()} total records returned");
                        }
                        else if (displayChoice == "4")  // Display Product Detail
                        {
                            var product = GetProduct(db, "Select the Product ID to Display", false);

                            if (product != null)
                            {
                                Console.WriteLine("\n Product ID:         {0,-20}", product.ProductId);
                                Console.WriteLine(" Product Name:       {0,-20}", product.ProductName);
                                Console.WriteLine(" Category ID:        {0,-40}", product.CategoryId);
                                Console.WriteLine(" Supplier ID:        {0,-20}", product.SupplierId);
                                Console.WriteLine($" Quantity per Unit:  {product.QuantityPerUnit} ");
                                Console.WriteLine(" Unit Price:        {0,-20:C}", product.UnitPrice);
                                Console.WriteLine($" Units In Stock:     {product.UnitsInStock} ");
                                Console.WriteLine($" Units On Order:     {product.UnitsOnOrder} ");
                                Console.WriteLine($" Reorder Level:      {product.ReorderLevel} ");
                                Console.WriteLine($" Discontinued:       {product.Discontinued} \n");
                                logger.Info("Product (id: {productid}) displayed", product.ProductId);
                            }
                        }
                    }
                    else if (choice == "2") // Add Product
                    {
                        Product product = new Product();
                        NorthwindContext db = new NorthwindContext();

                        Console.WriteLine(" Enter Product Name:");
                        product.ProductName = Console.ReadLine();

                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(product, context, results, true);

                        if (product.ProductName  != "") 
                        {
                            // check for unique product name
                            if (db.Products.Any(p => p.ProductName == product.ProductName))
                            {
                                Console.WriteLine("Product is already in the database");
                            }
                            else
                            {
                                // allow user to select the category for the product 
                                Category category = new Category();
                                var query = db.Categories.OrderBy(p => p.CategoryName);
                                Console.WriteLine();

                                if (query.Count() == 0)
                                {
                                    Console.WriteLine($"  <No Categories>");
                                }
                                else foreach (var item in query)
                                {
                                    Console.WriteLine($"  {item.CategoryId}) {item.CategoryName} - {item.Description}");
                                }
                                Console.WriteLine("\n Enter the Category ID for the Product");
                                if (int.TryParse(Console.ReadLine(), out int intProdCat))
                                {
                                    var valCategory = db.Categories.FirstOrDefault(c => c.CategoryId == intProdCat);

                                    if (valCategory != null)
                                    {
                                        logger.Info($"CategoryId {intProdCat} {valCategory.CategoryName} selected");
                                        Console.WriteLine(" Enter the Quantity per Unit:");
                                        product.QuantityPerUnit = Console.ReadLine();
                                        Console.WriteLine(" Enter the Unit Price:");
                                        if (decimal.TryParse(Console.ReadLine(), out decimal UnitPrice))
                                        {
                                            product.UnitPrice = UnitPrice;

                                            Console.WriteLine(" Enter the Units in Stock:");
                                            if (short.TryParse(Console.ReadLine(), out short UnitsInStock))
                                            {
                                                product.UnitsInStock = UnitsInStock;

                                                Console.WriteLine(" Enter the Units on Order:");
                                                if (short.TryParse(Console.ReadLine(), out short UnitsOnOrder))
                                                {
                                                    product.UnitsOnOrder = UnitsOnOrder;

                                                    Console.WriteLine(" Enter the Reorder Level:");
                                                    if (short.TryParse(Console.ReadLine(), out short Reorder))
                                                    {
                                                        product.ReorderLevel = Reorder;

                                                        Console.WriteLine(" Has this product been discontinued?");
                                                        var YorN = Console.ReadLine();
                                                        if (YorN.ToUpper() == "N")
                                                            product.Discontinued = false;
                                                        else
                                                            product.Discontinued = true;

                                                        logger.Info("Validation passed");
                                                        var productDb = new Product
                                                        {
                                                            ProductName = product.ProductName,
                                                            CategoryId = intProdCat,
                                                            QuantityPerUnit = product.QuantityPerUnit,
                                                            UnitPrice = product.UnitPrice,
                                                            UnitsOnOrder = product.UnitsOnOrder,
                                                            UnitsInStock = product.UnitsInStock,
                                                            ReorderLevel = product.ReorderLevel,
                                                            Discontinued = product.Discontinued
                                                        };
                                                        db.AddProduct(productDb);
                                                        logger.Info("Product added - {name}", product.ProductName);
                                                    }
                                                    else logger.Info("Invalid Reorder Level");
                                                }
                                                else logger.Info("Invalid Units On Order");
                                            }
                                            else logger.Info("Invalid Units In Stock");
                                        }
                                        else logger.Info("Invalid Price");
                                    }
                                    else logger.Info("Category Not Found");                                    
                                }
                                else logger.Info("Invalid Category");
                            }
                        }
                        else logger.Info("Product Name is Required");
                    }                   
                    else if (choice == "3")  // Edit Product
                    {
                        var db = new NorthwindContext();
                        var product = GetProduct(db, "Select the Product ID to Update", true);

                        if (product != null)
                        {
                            Product saveProduct = product;
                            // input product
                            Product UpdatedProduct = InputProduct(db, saveProduct);

                            if (UpdatedProduct != null)
                            {
                                UpdatedProduct.ProductId = product.ProductId;
                                db.EditProduct(UpdatedProduct);
                                logger.Info("Product (id: {productid}) updated", UpdatedProduct.ProductId);
                            }
                        }
                    }
                    else if (choice == "4") // Delete Product
                    {
                        var db = new NorthwindContext();
                        var product = GetProduct(db, "Select the Product ID to Delete", true);
                        if (product != null)
                        {
                            Console.WriteLine("Are you sure you want to delete " + product.ProductName + "?(y/n)");
                            String answer = Console.ReadLine();
                            if (answer.ToUpper() == "Y")
                            {
                                db.DeleteProduct(product);
                                logger.Info("Product (id: {productid}) deleted", product.ProductId);
                            }
                            else Console.WriteLine(product.ProductName + " was not deleted.");
                        }
                    }
                    else if (choice == "5")
                    {
                        string displayChoice = "";
                        // display choices to user
                        Console.WriteLine(" 1) Display Categories " +
                                          "\n 2) Display Category and Related Products" +
                                          "\n 3) Display All Categories and Their Related Products");
                        // input selection
                        displayChoice = Console.ReadLine().ToUpper();
                        Console.Clear();
                        logger.Info($"Display choice: {displayChoice} selected");
                        Console.WriteLine();
                        var db = new NorthwindContext();
                        if (displayChoice == "1") // Display Categories
                        {
                            var query = db.Categories.OrderBy(p => p.CategoryName);

                            foreach (var item in query)
                            {
                                Console.WriteLine($"  {item.CategoryName} - {item.Description}");
                            }
                            Console.WriteLine($"\n{query.Count()} records returned");
                            logger.Info($"{query.Count()} records returned");
                        }
                        else if (displayChoice == "2") // Display Category and Related Products
                        {
                            var query = db.Categories.OrderBy(p => p.CategoryId);

                            foreach (var item in query)
                            {
                                Console.WriteLine($"  {item.CategoryId}) {item.CategoryName}");
                            }
                            Console.WriteLine("\nSelect the category whose products you want to display:");
                            int id = int.Parse(Console.ReadLine());
                            var valCategory = (db.Categories.FirstOrDefault(c => c.CategoryId == id));
                            while (valCategory == null)
                            {
                                logger.Error($"CategoryId {id} selected");
                                Console.WriteLine(" Choose a Category ID from above");
                                id= int.Parse(Console.ReadLine());
                                valCategory = (db.Categories.FirstOrDefault(c => c.CategoryId == id));
                            }
                           
                            Console.Clear();
                            logger.Info($"CategoryId {id} selected");
                            Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                            Console.WriteLine($"  {category.CategoryName} - {category.Description}");

                            if (category.Products.Where(c => !c.Discontinued).Count() == 0)
                            {
                                if (category.CategoryId == archive) // Archive category only contains discontinued items
                                {
                                    int count = 0;
                                    foreach (Product p in category.Products.Where(c => c.Discontinued).OrderBy(c => c.ProductName))
                                    { 
                                        Console.WriteLine("\t" + p.ProductName);
                                        count++;
                                    }
                                    logger.Info($"{count} products returned");
                                }
                                else
                                    Console.WriteLine($"  <No Active Products>");
                            }
                            else
                            {
                                int count = 0;
                                foreach (Product p in category.Products.Where(c => !c.Discontinued).OrderBy(c => c.ProductName))
                                {
                                    Console.WriteLine("\t" + p.ProductName);
                                    count++;
                                }
                                logger.Info($"{count} products returned");
                            }
                            
                        }
                        else if (displayChoice == "3") // Display All Categories and Their Related Products
                        {
                            int ttlCount = 0;
                            var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                            foreach (var item in query)
                            {
                                int count = 0;
                                Console.WriteLine($" {item.CategoryName}");
                                if (item.Products.Count() == 0)
                                {
                                    Console.WriteLine($"  <No Products>");
                                }
                                else
                                {
                                    if (item.CategoryId == archive) // Archive category only contains discontinued items
                                    {
                                        foreach (Product p in item.Products.Where(c => c.Discontinued).OrderBy(c => c.ProductName))
                                        {
                                            Console.WriteLine($"\t{p.ProductName}");
                                            count++;
                                            ttlCount++;
                                        }
                                    }
                                    else
                                    {
                                        foreach (Product p in item.Products.Where(c => !c.Discontinued).OrderBy(c => c.ProductName))
                                        {
                                            Console.WriteLine($"\t{ p.ProductName}");
                                            count++;
                                            ttlCount++;
                                        }
                                    }
                                }
                            }
                            logger.Info($"{ttlCount} total products returned");
                        }
                    }
                    else if (choice == "6") // Add Category
                    {
                        Category category = new Category();
                        Console.WriteLine(" Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine(" Enter the Category Description:");
                        category.Description = Console.ReadLine();

                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            var db = new NorthwindContext();
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                var categoryDb = new Category { CategoryName = category.CategoryName, Description = category.Description };

                                db.AddCategory(categoryDb);

                                logger.Info("Category added - {name}", category.CategoryName);
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "7")  // Edit Category
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        foreach (var item in query)
                        {
                            Console.WriteLine($"  {item.CategoryId}) {item.CategoryName}");
                        }
                        Console.WriteLine("\nSelect The Category You Want to Update:");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                          
                            if (category != null)
                            {
                                logger.Info($"CategoryId {id} selected");
                                Category saveCategory = category;
                                // input category
                                Category UpdatedCategory = InputCategory(db, saveCategory);
                                if (UpdatedCategory != null)
                                {
                                    UpdatedCategory.CategoryId = category.CategoryId;
                                    db.EditCategory(UpdatedCategory);
                                    logger.Info("Category (id: {categoryid}) updated", UpdatedCategory.CategoryId);
                                }
                                Console.WriteLine($"{category.CategoryName} - {category.Description}");
                            }
                            else logger.Info("Category not Found");                        
                        }
                        else logger.Info("Invalid Category ID");
                    }
                    else if (choice == "8") // Delete Category and inactivate and move Products to Archive Category
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        foreach (var item in query)
                        {
                            Console.WriteLine($"  {item.CategoryId}) {item.CategoryName} - {item.Description}");
                        }
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);

                            if (category != null)
                            {
                                var valCategory = (db.Categories.FirstOrDefault(c => c.CategoryId == id));

                                logger.Info($"CategoryId {id} {valCategory.CategoryName} selected");
                                if (id == archive)  // Archive Category
                                    Console.WriteLine("Archive Category Cannot Be Deleted");
                                else
                                {   // Confirm Delete
                                    Category delCategory = db.Categories.FirstOrDefault(p => p.CategoryId == id);
                                    Console.WriteLine("Are you sure you want to delete " + category.CategoryName + "? (y/n)");
                                    String answer = Console.ReadLine();
                                    if (answer.ToUpper() == "Y")
                                    {
                                        db.Database.ExecuteSqlCommand("UPDATE Products SET Discontinued = 'true', " +
                                            "CategoryId = 22 WHERE CategoryID = " + id);
                                        db.Database.ExecuteSqlCommand("DELETE FROM Categories WHERE CategoryId = " + id);
                                        logger.Info("Category (id: {categoryid}) deleted", id);
                                    }
                                    else Console.WriteLine("Category ID " + id + " was not deleted.");
                                }
                            }
                            else logger.Info("Category not Found");
                        }
                        else logger.Info("Invalid Category ID");
                    }
                    if (choice != "1" && choice !="2" && choice != "3" && choice != "4" && choice !="5" &&
                        choice != "6" && choice !="7" && choice != "8" && choice != "9" && choice.ToLower() != "q")
                    {
                        Console.WriteLine(" Choose a Valid Menu Option");
                        logger.Info($"Invalid choice made: {choice}");
                    }

                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }
  
        public static Product GetProduct(NorthwindContext db, String action, bool showActive)
        {
            // display all Categories & Products
            // force eager loading of Products
            var categories = db.Categories.Include("Products").OrderBy(b => b.CategoryName);
            foreach (Category b in categories)
            {
                Console.WriteLine(b.CategoryName);
                if (b.Products.Count() == 0)
                {
                    Console.WriteLine($"  <No Products>");
                }
                else
                {
                    foreach (Product p in b.Products)
                    {
                         string active;
                         if (showActive)
                           {
                            if (p.Discontinued)
                                 active = "*"; 
                            else active = ""; 
                               Console.WriteLine($"  {p.ProductId}) {p.ProductName} {active}");
                           }
                           else
                        Console.WriteLine($"  {p.ProductId}) {p.ProductName}");
                    }
                }
            }
            if (showActive) Console.WriteLine("* Indicates Discontinued Product");
            Console.Write($"\n{action} \n");

            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product product = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }

        public static Product InputProduct(NorthwindContext db, Product saveProduct)
        {
            String YorN = "N";
            Product product = new Product();

            Console.WriteLine(" Enter the Product Name or hit the Enter key to keep the same Name:");
            Console.WriteLine(" Current: " + saveProduct.ProductName);
            product.ProductName = Console.ReadLine();
            if (product.ProductName == "")
                product.ProductName = saveProduct.ProductName;

            Console.WriteLine(" Enter the Quantity per Unit or hit the Enter key to keep the same Quantity:");
            Console.WriteLine(" Current: " + saveProduct.QuantityPerUnit);
            product.QuantityPerUnit = Console.ReadLine();
            if (product.QuantityPerUnit == "")
                product.QuantityPerUnit = saveProduct.QuantityPerUnit;

            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(product, context, results, true);

            Console.WriteLine(" Enter the Unit Price: (Must be entered and must be numeric)");
            Console.WriteLine(" Current:        {0,-20:C} ", saveProduct.UnitPrice);
            if (decimal.TryParse(Console.ReadLine(), out decimal UnitPrice))
            {
                product.UnitPrice = UnitPrice;

                Console.WriteLine(" Enter the Units in Stock: (Must be entered and must be numeric)");
                Console.WriteLine(" Current: " + saveProduct.UnitsInStock);
                if (short.TryParse(Console.ReadLine(), out short UnitsInStock))
                {
                    product.UnitsInStock = UnitsInStock;

                    Console.WriteLine(" Enter the Units on Order: (Must be entered and must be numeric)");
                    Console.WriteLine(" Current: " + saveProduct.UnitsOnOrder);
                    if (short.TryParse(Console.ReadLine(), out short UnitsOnOrder))
                    {
                        product.UnitsOnOrder = UnitsOnOrder;

                        Console.WriteLine(" Enter the Reorder Level: (Must be entered and must be numeric)");
                        Console.WriteLine(" Current: " + saveProduct.ReorderLevel);
                        if (short.TryParse(Console.ReadLine(), out short Reorder))
                        {
                            product.ReorderLevel = Reorder;

                            Console.WriteLine(" Has this product been discontinued? (y/n)");
                            if (saveProduct.Discontinued)
                                Console.WriteLine(" Current: Y");
                            else Console.WriteLine(" Current: N");
                            YorN = Console.ReadLine();
                            while (YorN.ToUpper() != "Y" && YorN.ToUpper() != "N")
                            {
                                Console.WriteLine(" Has this product been discontinued? (y/n)");
                                YorN = Console.ReadLine();
                            }
                            if (YorN.ToUpper() == "N")
                            {
                                if (saveProduct.CategoryId == archive)  // Archive Category
                                {
                                    Console.WriteLine("\nChoose Another Category for Active Products");
                                    Console.WriteLine();
                                    Category category = new Category();
                                    var query = db.Categories.OrderBy(p => p.CategoryName);

                                    if (query.Count() == 0)
                                    {
                                        Console.WriteLine($"  <No Categories>");
                                    }
                                    else foreach (var item in query)
                                        {
                                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName} - {item.Description}");
                                        }
                                    Console.WriteLine("\nEnter the Category for the Product:");
                                    product.CategoryId = int.Parse(Console.ReadLine());
                                    if (product.CategoryId == archive) // Archive Category
                                    {
                                        Console.WriteLine("\nActive products cannot be in the Archive Category");
                                        Console.WriteLine("\nEnter the Category for the Product:");
                                        product.CategoryId = int.Parse(Console.ReadLine());
                                    }

                                } else  product.CategoryId = saveProduct.CategoryId;
                                product.Discontinued = false;
                            }
                            else
                            {
                                product.CategoryId = saveProduct.CategoryId;
                                product.Discontinued = true;
                            }
                            return product;
                        }
                        else logger.Info("Invalid Reorder Level");
                    }
                    else logger.Info("Invalid Units On Order");
                }
                else logger.Info("Invalid Units In Stock");
            }
            else logger.Info("Invalid Price");

            return null;
        }

        public static Category InputCategory(NorthwindContext db, Category saveCategory)
        {
            Category category = new Category();
            Console.WriteLine(" Enter the Category Name or hit the Enter key to keep the same Name");
            Console.WriteLine(" Current: " + saveCategory.CategoryName);
            category.CategoryName = Console.ReadLine();
            if (category.CategoryName == "")
                category.CategoryName = saveCategory.CategoryName;

            Console.WriteLine(" Enter the Category Description or hit the Enter key to keep the same Description:");
            Console.WriteLine(" Current: " + saveCategory.Description);
            category.Description = Console.ReadLine();
            if (category.Description == "")
                category.Description = saveCategory.Description;

            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                return category;
            }
            else
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            return null;
        }
    }
}
