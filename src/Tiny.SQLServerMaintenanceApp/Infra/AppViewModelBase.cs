using GalaSoft.MvvmLight;

namespace Tiny.SQLServerMaintenanceApp
{
    public abstract class AppViewModelBase : ViewModelBase
    {
        private bool _isBusy;

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                Set(ref _isBusy, value);
            }
        }
    }
}
