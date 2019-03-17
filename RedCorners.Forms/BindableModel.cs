using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using RedCorners.Models;

#if WINDOWS
using System.Windows;
#endif

namespace RedCorners
{
    [AttributeUsage(AttributeTargets.All)]
    public class ManualUpdate : Attribute { }

    public partial class BindableModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string m = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(m));
        public void RaisePropertyChanged([CallerMemberName] string m = null) =>
            OnPropertyChanged(m);

        public virtual bool IsBusy
        {
            get => Status == TaskStatuses.Busy;
            set
            {
                if (value) Status = TaskStatuses.Busy;
                else Status = TaskStatuses.Success;
            }
        }

        [ManualUpdate] public bool IsFailed => Status == TaskStatuses.Fail;
        [ManualUpdate] public bool IsFinished => Status == TaskStatuses.Success;
        [ManualUpdate] public bool IsFirstTime => _isFirstTime;
        [ManualUpdate] public bool IsIdle => !IsBusy;
        [ManualUpdate] public bool IsNotFailed => !IsFailed;
        [ManualUpdate] public bool IsNotFinished => !IsFinished;

        TaskStatuses _status = TaskStatuses.Busy;
        bool _isFirstTime = true;
        public TaskStatuses Status
        {
            get => _status;
            set
            {
                _status = value;
                if (_status == TaskStatuses.Success)
                    _isFirstTime = false;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsBusy));
                RaisePropertyChanged(nameof(IsFailed));
                RaisePropertyChanged(nameof(IsNotFailed));
                RaisePropertyChanged(nameof(IsFinished));
                RaisePropertyChanged(nameof(IsNotFinished));
                RaisePropertyChanged(nameof(IsFirstTime));
                RaisePropertyChanged(nameof(IsIdle));
            }
        }


        public void ResetFirstTime()
        {
            _isFirstTime = true;
            RaisePropertyChanged(nameof(IsBusy));
            RaisePropertyChanged(nameof(IsFailed));
            RaisePropertyChanged(nameof(IsNotFailed));
            RaisePropertyChanged(nameof(IsFinished));
            RaisePropertyChanged(nameof(IsFirstTime));
            RaisePropertyChanged(nameof(IsNotFinished));
            RaisePropertyChanged(nameof(IsIdle));
            RaisePropertyChanged(nameof(Status));
        }


#if WINDOWS
        public Visibility IsBusyVisibility =>
            IsBusy ? Visibility.Visible : Visibility.Collapsed;
#endif

        public virtual void Refresh()
        {

        }

        public BindableModel() { }

        protected virtual void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            storage = value;
            RaisePropertyChanged(propertyName);
        }

        public void UpdateProperties(bool forceAll = false)
        {
#if WINDOWS
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
#else
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
#endif
            {
                foreach (var item in GetType().GetProperties())
                {
                    if (item.GetCustomAttributes(typeof(ManualUpdate), true).Any() && !forceAll)
                        continue;

                    RaisePropertyChanged(item.Name);
                }
            });
        }

        public void UpdateProperties(IEnumerable<string> names)
        {
#if WINDOWS
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
#else
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
#endif
            {
                foreach (var item in names)
                    RaisePropertyChanged(item);
            });
        }
    }

#if WINDOWS
    public class Command : System.Windows.Input.ICommand
    {
        private Action _action;
        private bool _canExecute;
        public Command(Action action, bool canExecute = true)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
#endif
}
