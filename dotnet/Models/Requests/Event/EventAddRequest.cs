using Sabio.Models.Domain;
using Sabio.Models.Requests.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sabio.Models.Requests
{
    public class EventAddRequest
    {
        [Required(ErrorMessage = "Event Type is required")]
        [Range(1, 9999)]
        public int EventTypeId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(32, ErrorMessage = "Name is too long")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Summary is required")]
        [StringLength(250, ErrorMessage = "Name is too long")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "Short Description is required")]
        [StringLength(225, ErrorMessage = "Description is too long")]
        public string ShortDescription { get; set; }


        [Required(ErrorMessage = "Venue is required")]
        [Range(1, 9999)]
        public string VenueId { get; set; }

        [Required(ErrorMessage = "Event Status is required")]
        [Range(1, 9999)]
        public int EventStatusId { get; set; }

        [Required(ErrorMessage = "Image Url is required")]
        [Url]
        public string ImageUrl { get; set; }


        [Required(ErrorMessage = "Site Url is required")]
        [Url]
        public string ExternalSiteUrl { get; set; }


        [Required(ErrorMessage = "Free Boolean is required")]
        public bool IsFree { get; set; }
        [Required]
       public List<FileAddRequest> EventFiles { get; set; }

        [Required(ErrorMessage = "Date Start is required")]
        public DateTime DateStart { get; set; }
        [Required(ErrorMessage = "Date End is required")]
        public DateTime DateEnd { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }

    }

    
}
