using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static eComAPI.Models.Annotations;

namespace eComAPI.Models
{
    public class ProductCollection
    {
        [Required]
        [Key]
        [AutoIncrement]
        public int CollectionId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }
}