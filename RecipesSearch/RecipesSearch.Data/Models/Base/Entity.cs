using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecipesSearch.Data.Models.Base.Errors;

namespace RecipesSearch.Data.Models.Base
{
    public class Entity : IEntity
    {
        private DateTime _createdDate;
        private DateTime _modifiedDate;      
        private bool? _isActive;

        protected TimeZoneInfo CurrentTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time");

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedDate
        {
            get { return Id == 0 ? DateTime.Now.ToUniversalTime() : _createdDate; }
            set { _createdDate = value; }
        }

        public DateTime LocalCreatedDate
        {
            get
            {
                return CreatedDate.Add(CurrentTimeZone.GetUtcOffset(CreatedDate));
            }
        }

        [Required]
        public DateTime ModifiedDate
        {
            get { return _modifiedDate == DateTime.MinValue ? CreatedDate : _modifiedDate; }
            set { _modifiedDate = value; }
        }

        [Required]
        public bool IsActive
        {
            get { return _isActive ?? true; }
            set { _isActive = value; }
        }

        [NotMapped]
        public List<Error> Errors { get; set; }
    }
}
