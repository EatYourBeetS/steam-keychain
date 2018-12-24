using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SteamKeychain
{
    public class Command : ICommand
    {
        private Action<object> _execute = null;
        private Func<object, bool> _canExecute = null;

        public Command(Action execute)
        {
            _execute = (_) => execute();
        }

        public Command(Action<object> execute)
        {
            _execute = execute;
        }

        public Command(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }

        public virtual void OnCanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged?.Invoke(sender, e);
        }

        public event EventHandler CanExecuteChanged;
    }
}
