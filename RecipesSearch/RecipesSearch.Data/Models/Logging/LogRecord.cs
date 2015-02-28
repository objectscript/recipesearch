using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.Data.Framework;
using RecipesSearch.Data.Models.Base;

namespace RecipesSearch.Data.Models.Logging
{
    [CachePackage(Constants.LoggingCachePackage)]
    public class LogRecord : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        public LogRecordType Type { get; set; }
        
        public string Description { get; set; }
        
        public string Exception { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }
}
