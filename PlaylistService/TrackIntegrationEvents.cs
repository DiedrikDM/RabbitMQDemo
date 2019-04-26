using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistService
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

}
