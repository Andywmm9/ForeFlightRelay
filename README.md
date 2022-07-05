<h1>ForeFlightRelay</h1>
This stand-alone Windows application listens for data from Microsoft Flight Simulator and broadcast UDP packets for ForeFlight to receive and display information about your flight.

## Installing the Application
1. Download the ZIP file for the latest version here: https://github.com/Andywmm9/Miller.Msfs.ForeFlightRelay/releases.
2.  Unzip to a location on your machine.
3.  Run Miller.Msfs.ForeFlightRelay.exe.  When a connection to the simulator is found, the red circle will change to green, and packets will then be broadcasted on your local network.  ForeFlight should automatically begin using this, if not check ForeFlight's Settings and Devices.

## Configuring MSFS to Auto Launch ForeFlightRelay
A great feature about flight simulator is that it supports automatically launching apps when flight simulator starts so you will never have to remember to start this program.  Navigate to this path (note this may differ based on the MS Store version versus Steam):  C:\Users\[username]\AppData\Local\Packages\Microsoft.FlightSimulator_8wekyb3d8bbwe\LocalCache.

Check to see if a exe.xml file exists in this path.  If it does not, then create it.

If you created the file, paste this in the file, update the path, and save and close:

    <?xml version="1.0" encoding="Windows-1252"?>
    <SimBase.Document Type="Launch" version="1,0">
      <Descr>Launch</Descr>
      <Filename>EXE.xml</Filename>
      <Disabled>False</Disabled>
      <Launch.ManualLoad>True</Launch.ManualLoad>

      <Launch.Addon>
        <Name>ForeFlight Relay</Name>
        <Disabled>False</Disabled>
        <Path>[path to Miller.Msfs.ForeFlightRelay.exe]</Path>
        <CommandLine></CommandLine>
      </Launch.Addon>
    </SimBase.Document>

## Compiling the Source Code
Download and compile the source code using Visual Studio.  The only prequisite, SimConnect.dll and Microsoft.FlightSimulator.SimConnect.dll are included in the project directory.
