using System;
using System.ComponentModel.DataAnnotations;

namespace NorthwindConsole.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Product Name is Required")]
        [StringLength(40, ErrorMessage ="Maximum product name length of 40 characters")]
        public string ProductName { get; set; }
        [StringLength(20, ErrorMessage = "Maximum quantity per unit length of 20 characters")]
        public string QuantityPerUnit { get; set; }
     //   [Range(0.00, 9999.99, ErrorMessage = "Price must be between 0.00 and 9999.99")]
        [Range(typeof(decimal), "0.0", "9999.99", ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public decimal? UnitPrice { get; set; }
        [Range(0, Int16.MaxValue, ErrorMessage = "Units in Stock is out of range")]
        public Int16? UnitsInStock { get; set; }
        [Range(0, Int16.MaxValue, ErrorMessage = "Units on Order is out of range")]
        public Int16? UnitsOnOrder { get; set; }
        [Range(0, Int16.MaxValue, ErrorMessage = "Reorder Level is out of range")]
        public Int16? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}

