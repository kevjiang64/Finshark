using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Comments")]
    public class Comment
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int? StockId { get; set; } //FK to Stock
        public Stock? Stock{ get; set; } //Navigation property to Stock
        public string AppUserId { get; set; } = string.Empty; 
        public AppUser AppUser { get; set; }
    }
}