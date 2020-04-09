
using System;
using System.Collections.Generic;
using System.Text;

namespace Sabio.Models.Domain
{
   public class Event
    {
        public int Id { get; set; }
        public int EventTypeId { get; set; }
        public string EventType { get; set; }
        public string EventName { get; set; }
        public string Summary { get; set; }
        public string ShortDescription { get; set; }
        public int EventStatusId { get; set; }
        public string EventStatus { get; set; }
        public string EventImageUrl { get; set; }
        public string EventLink { get; set; }
        public bool IsFree { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int CreatedBy { get; set; }
        public UserProfile UserProfile { get; set; }
        public Venue Venue { get; set; }
        //public Location Location { get; set; }
        public State State { get; set; }        
        public List<File> Files { get; set; }
    }
}
