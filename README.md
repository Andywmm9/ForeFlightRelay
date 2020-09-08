<h1>Miller.Msfs.ForeFlightRelay</h1>
This stand-alone Windows application listens for data from Microsoft Flight Simulator and broadcast UDP packets for ForeFlight to receive and display information about your flight.

<h2>Installing the Application</h2>
1. Download the latest version in the Releases folder: https://github.com/Andywmm9/Miller.Msfs.ForeFlightRelay/tree/master/Miller.Msfs.ForeFlightRelay/Release

2. Unzip to a location on your machine.

3. Run Miller.Msfs.ForeFlightRelay.exe.  When a connection to the simulator is found, the red circle will change to green, and packets will then be broadcasted on your local network.  ForeFlight should automatically begin using this, if not check ForeFlight's Settings and Devices.


<h2>Compiling the Source Code</h2>
Download and compile the source code using Visual Studio.  The only prequisite, SimConnect.dll and Microsoft.FlightSimulator.SimConnect.dll are included in the project directory.
