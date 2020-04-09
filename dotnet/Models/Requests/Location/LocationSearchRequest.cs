using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sabio.Models.Requests.Location
{
    public class LocationSearchRequest
    {

        [Required(ErrorMessage = "Please enter valid latitude")]
        public double Latitude { get; set; }
        [Required(ErrorMessage = "Please enter valid longitude")]
        public double Longitude { get; set; }
        [Required(ErrorMessage = "Please enter valid radius")]
        public int Radius { get; set; }
        [Required(ErrorMessage = "Please enter valid page index")]
        public int PageIndex { get; set; }
        [Required(ErrorMessage = "Please enter valid page size")]
        public int PageSize { get; set; }

        [Required(ErrorMessage = "Please enter valid String")]
        public string Search { get; set; }
    }
}
