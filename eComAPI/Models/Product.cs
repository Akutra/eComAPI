using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static eComAPI.Models.Annotations;

namespace eComAPI.Models
{
    public class Product
    {
        [Required]
        [Key]
        [AutoIncrement]
        public int ProductId { get; set; }
        public int CollectionId { get; set; }
        public string Name { get; set; }
        public string defImage { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public string Access { get; set; }
        public string Match { get; set; }
        public string ItemType { get; set; }
        public double d1 { get; set; }
        public double d2 { get; set; }
        public double d3 { get; set; }
        public double weight { get; set; }
        public double price { get; set; }
        public override bool Equals(object obj)
        {
            bool rt = false; //if this is not a Products object it cannot equal the same

            if (obj.GetType() == typeof(Product)) //simplify like to like comparison
            {
                rt = ((Product)obj).ProductId == this.ProductId; //compare only the ID's (key field)
            }

            return rt;
        }

        public override int GetHashCode()
        {
            return this.ProductId.GetHashCode();
        }
    }
}