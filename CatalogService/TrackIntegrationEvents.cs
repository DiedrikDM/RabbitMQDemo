using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService
{
  public class TrackRemovedIntegrationEvent
  {
    public TrackRemovedIntegrationEvent(string trackId, string reason)
    {
      TrackId = trackId;
      Reason = reason;
    }

    public string TrackId { get; }
    public string Reason { get; }
  }

  public class TrackAddedIntegrationEvent
  {
    public TrackAddedIntegrationEvent(string trackId, string name)
    {
      TrackId = trackId;
      Name = name;
    }

    public string TrackId { get; }
    public string Name { get; }
  }

}
