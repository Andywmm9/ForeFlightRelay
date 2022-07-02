using System;
using System.Windows.Input;

namespace ForeFlightRelay.Wpf
{
    public class Command : ICommand
    {
        public Action<object> ExecuteDelegate { get; set; }
        public event EventHandler CanExecuteChanged = null;

        public Command()
        {
        }

        public Command(Action<object> executeDelegate)
        {
            ExecuteDelegate = executeDelegate;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ExecuteDelegate?.Invoke(parameter);
        }
    }
}
