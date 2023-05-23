using System;

namespace PUBG.Models.Telemetry;

public class Event
{
    public DateTime _D { get; set; }
    public EventType _T { get; set; }
    public Common Common { get; set; }
}
