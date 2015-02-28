using System;

namespace RecipesSearch.Data.Models.Base
{
    public interface IEntity
    {
        int Id { get; set; }
        
        DateTime CreatedDate { get; set; }
    }
}
