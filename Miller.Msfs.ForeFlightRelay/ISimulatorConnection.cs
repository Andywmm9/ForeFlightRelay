using System;

namespace Miller.Msfs.ForeFlightRelay
{
    public interface ISimulatorConnection
    {
        /// <summary>
        /// Fires when data from the simulator is received.
        /// </summary>
        event EventHandler<PositionUpdatedEventArgs> SimulatorDataReceived;

        /// <summary>
        /// Determines if the simulator is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Initializes a connection to the simulator.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from the simulator.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Begins receiving messages from the simulator.
        /// </summary>
        void ReceiveMessage();

        /// <summary>
        /// Sets the window handler.
        /// </summary>
        void SetWindowHandle(IntPtr hWnd);
    }
}
