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
        DateTimeOffset lastFire = DateTimeOffset.MinValue;

        public bool FireOnce { get; set; } = true;
        public double FireDelay { get; set; } = 0;

        bool ICommand.CanExecute(object parameter) => !executed;

        void ICommand.Execute(object parameter)
        {
            if (executed && FireOnce) return;
            if (FireDelay > 0 && (DateTimeOffset.Now - lastFire).TotalMilliseconds < FireDelay) return;
            executed = true;
            lastFire = DateTimeOffset.Now;
            Signals.PopModal.Send();
        }
    }
}
