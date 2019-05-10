using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace RedCorners.Forms
{
    public class PopCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        bool executed = false;

        bool ICommand.CanExecute(object parameter) => !executed;

        void ICommand.Execute(object parameter)
        {
            if (executed) return;
            executed = true;
            Signals.PopModal.Send();
        }
    }
}
