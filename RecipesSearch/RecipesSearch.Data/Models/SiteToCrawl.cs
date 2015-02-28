using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecipesSearch.Data.Framework;
using RecipesSearch.Data.Models.Base;

namespace RecipesSearch.Data.Models
{
    [CachePackage(Constants.DefaultCachePackage)]
    public class SiteToCrawl : Entity
    {
        public string Name { get; set; }

        public string URL { get; set; }

        public int ConfigId { get; set; }
    }
}
