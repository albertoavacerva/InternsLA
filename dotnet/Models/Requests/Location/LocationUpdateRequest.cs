using System;
using System.Collections.Generic;
using System.Text;

namespace Sabio.Models.Requests.Location
{
    public class LocationUpdateRequest : LocationAddRequest, IModelIdentifier
    {
        public int Id { get; set; }
    }
}
