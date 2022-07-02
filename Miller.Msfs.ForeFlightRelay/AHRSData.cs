using System.Runtime.InteropServices;

namespace ForeFlightRelay.Wpf
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct AHRSData
    {
        public double TrueHeading { get; set; }
        public double Pitch { get; set; }
        public double Roll { get; set; }
    }
}
