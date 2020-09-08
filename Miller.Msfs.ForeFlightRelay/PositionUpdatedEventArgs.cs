using System;

namespace Miller.Msfs.ForeFlightRelay
{
    public class PositionUpdatedEventArgs : EventArgs
    {
        public Position Position { get; set;}
    }
}
