using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace Miller.Msfs.ForeFlightRelay
{
    public class SimulatorConnection : ISimulatorConnection
    {
        private const string _applicationName = "ForeFlight Relay";
        private const int WM_USER_SIMCONNECT = 0x0402;
        private IntPtr m_hWnd = new IntPtr(0);
        private SimConnect _simConnect;
        private DispatcherTimer _dispatchTimer;

        public bool IsConnected { get; private set; }
        public event EventHandler<PositionUpdatedEventArgs> SimulatorDataReceived;

        enum DEFINITIONS
        {
            Struct1 = 0,
        }

        enum DATA_REQUESTS
        {
            REQUEST_1 = 0,
        };

        public SimulatorConnection()
        {
            _dispatchTimer = new DispatcherTimer();
            _dispatchTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            _dispatchTimer.Tick += OnDispatchTimerTick;
        }

        public void SetWindowHandle(IntPtr hWnd)
        {
            m_hWnd = hWnd;
        }

        public void Connect()
        {
            _simConnect = new SimConnect(_applicationName, m_hWnd, WM_USER_SIMCONNECT, null, 1);
            _simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(OnReceiveOpen);
            _simConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(OnReceiveQuit);
            _simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(OnReceiveException);
            _simConnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(OnReceiveData);

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

            _dispatchTimer.Stop();
            _simConnect.Dispose();
            _simConnect = null;
            IsConnected = false;
            Debug.WriteLine("Disconnected");
        }

        private void OnReceiveData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            switch ((DATA_REQUESTS)data.dwRequestID)
            {
                case DATA_REQUESTS.REQUEST_1:
                    AircraftState aircraftState = (AircraftState)data.dwData[0];
                    OnPositionReceived(this, new PositionUpdatedEventArgs(aircraftState));
                    break;

                default:
                    Debug.WriteLine("Unknown request ID: " + data.dwRequestID);
                    break;
            }
        }

        private void OnReceiveOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            _simConnect.AddToDataDefinition(DEFINITIONS.Struct1, "Title", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.00f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.00f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Altitude", "meters", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Heading Degrees True", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINITIONS.Struct1, "Ground Velocity", "meter/second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            _simConnect.RegisterDataDefineStruct<AircraftState>(DEFINITIONS.Struct1);

            _dispatchTimer.Start();
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

        private void OnDispatchTimerTick(object sender, EventArgs e)
        {
            _simConnect?.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            Debug.WriteLine("Request for data sent");
        }

        private void OnPositionReceived(object sender, PositionUpdatedEventArgs e)
        {
            SimulatorDataReceived?.Invoke(sender, e);
        }
    }
}
