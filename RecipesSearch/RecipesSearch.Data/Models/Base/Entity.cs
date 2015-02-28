using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesSearch.Data.Models.Base
{
    public class Entity : IEntity
    {
        private DateTime _createdDate;
        private DateTime _modifiedDate;      
        private bool? _isActive;

        protected TimeZoneInfo CurrentTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time");

        [Key]
        public int Id { get; set; }

        public DateTime CreatedDate
        {
            get { return Id == 0 ? DateTime.Now.ToUniversalTime() : _createdDate; }
            set { _createdDate = value; }
        }

        public DateTime ModifiedDate
        {
            get { return _modifiedDate == DateTime.MinValue ? CreatedDate : _modifiedDate; }
            set { _modifiedDate = value; }
        }

        public bool IsActive
        {
            get { return _isActive ?? true; }
            set { _isActive = value; }
        }

        [NotMapped]
        public DateTime LocalCreatedDate
        {
            get
            {
                return CreatedDate.Add(CurrentTimeZone.GetUtcOffset(CreatedDate));
            }
        }   
    }
}
