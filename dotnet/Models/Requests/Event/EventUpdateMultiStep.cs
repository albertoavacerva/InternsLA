using System;
using System.Collections.Generic;
using System.Text;

namespace Sabio.Models.Requests.Event
{
    public class EventUpdateMultiStep : EventAddMultiStep, IModelIdentifier
    {
      public int Id { get; set; }
    }
}
