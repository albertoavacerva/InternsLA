using System;
using System.Collections.Generic;
using System.Text;

namespace Sabio.Models.Requests.Event
{
    public class UpdateStatusRequest : IModelIdentifier
    {
        public int Id  { get; set; }
        public int EventStatusId { get; set; }
      
    }
}
