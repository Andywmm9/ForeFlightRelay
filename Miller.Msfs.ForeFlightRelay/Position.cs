using System;
using System.Runtime.InteropServices;

namespace Miller.Msfs.ForeFlightRelay
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct Position
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Title;
        public double Latitude;
        public double Longitude;
        public double Altitude;
    }
}
