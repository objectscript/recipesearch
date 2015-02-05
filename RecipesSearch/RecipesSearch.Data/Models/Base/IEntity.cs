using System;
using System.Collections.Generic;
using RecipesSearch.Data.Models.Base.Errors;

namespace RecipesSearch.Data.Models.Base
{
    public interface IEntity
    {
        int Id { get; set; }
        
        DateTime CreatedDate { get; set; }

        DateTime LocalCreatedDate{ get; }
        
        DateTime ModifiedDate { get; set; }
        
        bool IsActive { get; set; }
        
        List<Error> Errors { get; set; }
    }
}
