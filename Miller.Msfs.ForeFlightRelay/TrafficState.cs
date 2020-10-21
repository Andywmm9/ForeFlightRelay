using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Miller.Msfs.ForeFlightRelay
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TrafficState
    {
        // this is how you declare a fixed size string
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Callsign;
        public double Latitude;
        public double Longitude;
        public double Altitude;
        public double Heading;
        public double Groundspeed;
        public bool IsOnGround;
        public int ICAOAddress;
    }
}
