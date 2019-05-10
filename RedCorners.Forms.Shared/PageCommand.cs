using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace RedCorners.Forms
{
    [ContentProperty("Page")]
    public class PageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public PageCommand()
        {
        }

        bool ICommand.CanExecute(object parameter) => Page != null;

        void ICommand.Execute(object parameter)
        {
            if (IsModal) Signals.ShowModalPage.Send(Page);
            else Signals.ShowPage.Send(Page);
        }

        Page _page;
        public Page Page
        {
            get => _page;
            set
            {
                _page = value;
                CanExecuteChanged?.Invoke(this, null);
            }
        }

        bool _isModal = true;
        public bool IsModal
        {
            get => _isModal;
            set
            {
                _isModal = value;
            }
        }
    }
}
