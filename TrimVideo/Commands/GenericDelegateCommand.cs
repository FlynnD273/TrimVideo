using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TrimVideo.Commands
{
    internal class DelegateCommand<T> : ICommand<T>
    {
        public event EventHandler? CanExecuteChanged;

        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public DelegateCommand(Action execute, Func<T, bool> canExecute = null)
        {
            _execute = _ => execute.Invoke();
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter == null)
            {
                return _canExecute?.Invoke(default) ?? true;
            }

            T param = (T)parameter;
            return _canExecute?.Invoke(param) ?? true;
        }

        public void Execute(object? parameter)
        {
            if (parameter == null)
            {
                _execute?.Invoke(default);
                return;
            }

            T param = (T)parameter;
            _execute?.Invoke(param);
        }
    }
}
