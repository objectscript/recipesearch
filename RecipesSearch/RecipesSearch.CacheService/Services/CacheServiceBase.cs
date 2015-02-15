using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesSearch.CacheService.Services
{
    public abstract class CacheServiceBase
    {
        protected readonly string ServiceBase;
        
        protected CacheServiceBase()
        {
            ServiceBase = ConfigurationManager.AppSettings["BaseURL"];           
        }
    }
}
