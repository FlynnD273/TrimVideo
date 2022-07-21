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
        private Func<T, bool>? _canExecute;
        private bool _canExecuteState;

        public DelegateCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public DelegateCommand(Action execute, Func<T, bool>? canExecute = null)
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
            bool prevCanExecuteState = _canExecuteState;

            _canExecuteState = _canExecute?.Invoke(param) ?? true;

            if (prevCanExecuteState != _canExecuteState) CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            return _canExecuteState;
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
