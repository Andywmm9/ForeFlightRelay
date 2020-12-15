using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Diagnostics;

namespace Miller.Msfs.ForeFlightRelay
{
    public class MsfsSimulatorConnection : ISimulatorConnection
    {
        private const string _applicationName = "ForeFlight Relay";
        private const int WM_USER_SIMCONNECT = 0x0402;
        private const int _maximumTrafficRadiusMeters = 185200;
        private IntPtr m_hWnd = new IntPtr(0);
        private SimConnect _simConnect;

        public bool IsConnected { get; private set; }
        public event EventHandler<AircraftStateEventArgs> AircraftStateDataReceived;
        public event EventHandler<TrafficStateEventArgs> TrafficDataReceived;
        public event EventHandler<AHRSDataEventArgs> AHRSDataReceived;
        public event EventHandler<EventArgs> SimulatorConnectionLost;

        enum DEFINITIONS
        {
            AIRCRAFT_STATE_DATA = 0,
            TRAFFIC_LIST_DATA = 1,
            AHRS_DATA = 2,
            TRAFFIC_STATE_DATA = 3
        }

        enum DATA_REQUESTS
        {
            AIRCRAFT_STATE = 0,
            TRAFFIC_LIST = 1,
            AHRS = 2,
            TRAFFIC_STATE = 3
        };

        public void SetWindowHandle(IntPtr hWnd)
        {
            m_hWnd = hWnd;
        }

        public void Connect()
        {
            _simConnect = new SimConnect(_applicationName, m_hWnd, WM_USER_SIMCONNECT, null, 0);
            _simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(OnReceiveOpen);
            _simConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(OnReceiveQuit);
            _simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(OnReceiveException);
            _simConnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(OnReceiveData);
            _simConnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(OnReceiveData);

            IsConnected = true;
            Debug.WriteLine("Connected");
        }

        public void ReceiveMessage()
        {
            _simConnect?.ReceiveMessage();
        }

        public void Disconnect()
        {
            if (_simConnect == null)
            {
                return;
            }

            _simConnect.Dispose();
            _simConnect = null;
            IsConnected = false;
            OnSimulatorConnectionLost(this, new EventArgs());
            Debug.WriteLine("Disconnected");
        }

        private void OnReceiveData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            var dataRequestId = data.dwRequestID;

            if (dataRequestId > (int)DATA_REQUESTS.TRAFFIC_STATE)
                dataRequestId = (int)DATA_REQUESTS.TRAFFIC_STATE;

            switch ((DATA_REQUESTS)dataRequestId)
            {
                case DATA_REQUESTS.AIRCRAFT_STATE:
                    if (!(data.dwData[0] is AircraftState))
                    {
                        Debug.WriteLine("Expected AircraftState packet, but didn't get it.");
                        break;
                    }

                    AircraftState aircraftState = (AircraftState)data.dwData[0];
                    OnPositionReceived(this, new AircraftStateEventArgs(aircraftState));
                    _simConnect.RequestDataOnSimObjectType(DATA_REQUESTS.TRAFFIC_LIST, DEFINITIONS.TRAFFIC_LIST_DATA, _maximumTrafficRadiusMeters,
                        SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT); // This isn't a good place for this, but we need to get an updated list of traffic in the sim.

                    break;
                case DATA_REQUESTS.TRAFFIC_LIST:
                    // Subscribe to each aircraft for data.+
                    _simConnect?.RequestDataOnSimObject(DATA_REQUESTS.TRAFFIC_STATE + (int)data.dwObjectID, DEFINITIONS.TRAFFIC_STATE_DATA, data.dwObjectID,
                        SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);

                    break;
                case DATA_REQUESTS.AHRS:
                    if (!(data.dwData[0] is AHRSData))
                    {
                        Debug.WriteLine("Expected AHRSData packet, but didn't get it.");
                        break;
                    }

                    AHRSData ahrsData = (AHRSData)data.dwData[0];
                    OnAHRSDataReceived(this, new AHRSDataEventArgs(ahrsData));

                    break;
                case DATA_REQUESTS.TRAFFIC_STATE:
                    if (!(data.dwData[0] is TrafficState))
                    {
                        Debug.WriteLine("Expected TraffiState packet, but didn't get it.");
                        break;
                    }

                    TrafficState trafficState = (TrafficState)data.dwData[0];
                    trafficState.ICAOAddress = (int)data.dwRequestID;
                    OnTrafficReceived(this, new TrafficStateEventArgs(trafficState));
                    break;

                default:
                    Debug.WriteLine("Unknown request ID: " + data.dwRequestID);
                    break;
            }
        }

        private void OnReceiveOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            _simConnect.AddToDataDefinition(DEFINITIONS.AIRCRAFT_STATE_DATA, "Title", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.AIRCRAFT_STATE_DATA, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.00f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.AIRCRAFT_STATE_DATA, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.00f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.AIRCRAFT_STATE_DATA, "Plane Altitude", "meters", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.AIRCRAFT_STATE_DATA, "Plane Heading Degrees True", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.AIRCRAFT_STATE_DATA, "Ground Velocity", "meter/second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.RegisterDataDefineStruct<AircraftState>(DEFINITIONS.AIRCRAFT_STATE_DATA);
            _simConnect.RequestDataOnSimObject(DATA_REQUESTS.AIRCRAFT_STATE, DEFINITIONS.AIRCRAFT_STATE_DATA, 0,
                SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);

            _simConnect.AddToDataDefinition(DEFINITIONS.TRAFFIC_LIST_DATA, "Title", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.RegisterDataDefineStruct<TrafficList>(DEFINITIONS.TRAFFIC_LIST_DATA);

            _simConnect.AddToDataDefinition(DEFINITIONS.AHRS_DATA, "Plane Heading Degrees True", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.00f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.AHRS_DATA, "Plane Pitch Degrees", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.00f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.AHRS_DATA, "Plane Bank Degrees", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.00f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.RegisterDataDefineStruct<AHRSData>(DEFINITIONS.AHRS_DATA);
            _simConnect.RequestDataOnSimObject(DATA_REQUESTS.AHRS, DEFINITIONS.AHRS_DATA, 0,
                SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);

            _simConnect.AddToDataDefinition(DEFINITIONS.TRAFFIC_STATE_DATA, "ATC ID", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.TRAFFIC_STATE_DATA, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.00f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.TRAFFIC_STATE_DATA, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.00f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.TRAFFIC_STATE_DATA, "Plane Altitude", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.TRAFFIC_STATE_DATA, "Plane Heading Degrees True", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.TRAFFIC_STATE_DATA, "Ground Velocity", "knot", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.TRAFFIC_STATE_DATA, "Sim on Ground", null, SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.RegisterDataDefineStruct<TrafficState>(DEFINITIONS.TRAFFIC_STATE_DATA);
            _simConnect.RequestDataOnSimObjectType(DATA_REQUESTS.TRAFFIC_LIST, DEFINITIONS.TRAFFIC_LIST_DATA, _maximumTrafficRadiusMeters, SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT);

            Debug.WriteLine("Receive Open");
        }

        private void OnReceiveQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Disconnect();
        }

        private void OnReceiveException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Debug.WriteLine((SIMCONNECT_EXCEPTION)data.dwException);
        }

        private void OnPositionReceived(object sender, AircraftStateEventArgs e)
        {
            AircraftStateDataReceived?.Invoke(sender, e);
        }

        private void OnTrafficReceived(object sender, TrafficStateEventArgs e)
        {
            TrafficDataReceived?.Invoke(sender, e);
        }

        private void OnAHRSDataReceived(object sender, AHRSDataEventArgs e)
        {
            AHRSDataReceived?.Invoke(sender, e);
        }

        private void OnSimulatorConnectionLost(object sender, EventArgs e)
        {
            SimulatorConnectionLost?.Invoke(sender, e);
        }
    }
}
