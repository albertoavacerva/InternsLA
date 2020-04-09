using System;
using System.Collections.Generic;
using System.Text;

namespace Sabio.Models.Requests.NewFolder
{
    public class EventUpdateRequest : EventAddRequest , IModelIdentifier
    {
        public int Id { get; set; }
    }
}
