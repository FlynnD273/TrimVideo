using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TrimVideo.Commands
{
    internal class DelegateCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private Action _execute;
        private Func<bool>? _canExecute;
        private bool _canExecuteState;

        public DelegateCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            bool prevCanExecuteState = _canExecuteState;

            _canExecuteState = _canExecute?.Invoke() ?? true;

            if (prevCanExecuteState != _canExecuteState) CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            return _canExecuteState;
        }

        public void Execute(object? parameter) => _execute?.Invoke();
    }
}
