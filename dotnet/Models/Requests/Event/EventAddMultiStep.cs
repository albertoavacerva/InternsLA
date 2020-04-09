using Sabio.Models.Requests.Files;
using Sabio.Models.Requests.Location;
using Sabio.Models.Requests.Venue;
using Sabio.Models.Requests.Venues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sabio.Models.Requests.Event
{
   public class EventAddMultiStep
    {

        [Required(ErrorMessage = "Event type is required")]
        [Range(1, 9999)]
        public int EventTypeId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(32, ErrorMessage = "Name is too long")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Summary is required")]
        [StringLength(250, ErrorMessage = "Summary is too long")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "Short description is required")]
        [StringLength(225, ErrorMessage = "Description is too long")]
        public string ShortDescription { get; set; }
        
        [Range(0,9999, ErrorMessage = "VenueId must be a positive number, or zero if one is not available")]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Event status is required")]
        [Range(1, 9999)]
        public int EventStatusId { get; set; }

        [Required(ErrorMessage = "Image url is required")]
        [Url]
        public string ImageUrl { get; set; }

        [Url]
        public string ExternalSiteUrl { get; set; }

        [Required(ErrorMessage = "IsFree boolean is required")]
        public bool IsFree { get; set; }

        [Required(ErrorMessage = "Date start is required")]
        public DateTime DateStart { get; set; }

        [Required(ErrorMessage = "Date end is required")]
        public DateTime DateEnd { get; set; }

        public List<FileAddRequest> EventFiles { get; set; }

        public VenueMultiStep Venue { get; set; }

        public LocationMultiStep Location { get; set; }

    }
}

