using System.Runtime.InteropServices;

namespace Miller.Msfs.ForeFlightRelay
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct AHRSData
    {
        public double TrueHeading { get; set; }
        public double Pitch { get; set; }
        public double Roll { get;  set; }
    }
}
