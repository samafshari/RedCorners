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

        bool ICommand.CanExecute(object parameter) => Page != null || PageType != null;

        void ICommand.Execute(object parameter)
        {
            if (PageType != null)
            {
                _page = Activator.CreateInstance(PageType) as Page;
                if (Page == null)
                    throw new Exception("PageType did not construct a Page!");
            }
            if (IsModal) Signals.ShowModalPage.Send(Page);
            else Signals.ShowPage.Send(Page);
        }

        Page _page;
        public Page Page
        {
            get => _page;
            set
            {
                if (_pageType != null)
                    throw new Exception("Cannot set Page when PageType is set.");

                _page = value;
                CanExecuteChanged?.Invoke(this, null);
            }
        }

        Type _pageType;
        public Type PageType
        {
            get => _pageType;
            set
            {
                if (value != null)
                {
                    _page = null;
                }

                _pageType = value;
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
