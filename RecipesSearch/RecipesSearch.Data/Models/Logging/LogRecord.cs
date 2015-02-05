using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesSearch.Data.Models.Logging
{
    public class LogRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public LogRecordType Type { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        public string Exception { get; set; }
        
        public string ExceptionStackTrace { get; set; }
        
        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
