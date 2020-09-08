using System;

namespace Miller.Msfs.ForeFlightRelay
{
    public class PositionUpdatedEventArgs : EventArgs
    {
        public PositionUpdatedEventArgs(AircraftState aircraftState)
        {
            AircraftState = aircraftState;
        }

        public AircraftState AircraftState { get; set;}
    }
}
