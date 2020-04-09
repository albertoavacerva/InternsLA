using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sabio.Models.Requests.Location
{
    public class LocationMultiStep
    {
        [Required(ErrorMessage = "Please enter valid location type")]
        [Range(1, 9999)]
        public int LocationTypeId { get; set; }
        [Required(ErrorMessage = "Please enter valid location address")]
        [StringLength(3000, MinimumLength = 1)]
        public string LineOne { get; set; }
        public string LineTwo { get; set; }
        [Required(ErrorMessage = "Please enter valid location city")]
        [StringLength(3000, MinimumLength = 3)]
        public string City { get; set; }
        [Required(ErrorMessage = "Please enter valid location zipcode")]
        [StringLength(3000, MinimumLength = 5)]
        public string Zip { get; set; }
        [Required(ErrorMessage = "Please enter valid location state")]
        [Range(1, 181, ErrorMessage = "StateId must be between 1 and 181 ")]
        public int StateId { get; set; }
        [Required(ErrorMessage = "Please enter valid location latitude")]
        public double Latitude { get; set; }
        [Required(ErrorMessage = "Please enter valid location longitude")]
        public double Longitude { get; set; }
        [Required(ErrorMessage = "Please enter valid date")]
        public DateTime DateCreated { get; set; }
        [Required(ErrorMessage = "Please enter valid date")]
        public DateTime DateModified { get; set; }
    }
}
