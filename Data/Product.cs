using System.ComponentModel.DataAnnotations;

namespace CachingTechniques.Data
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
