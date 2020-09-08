using System.Runtime.InteropServices;

namespace Miller.Msfs.ForeFlightRelay
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct AircraftState
    {
        // this is how you declare a fixed size string
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Title;
        public double Latitude;
        public double Longitude;
        public double Altitude;
        public double Heading;
        public double Groundspeed;
    }
}