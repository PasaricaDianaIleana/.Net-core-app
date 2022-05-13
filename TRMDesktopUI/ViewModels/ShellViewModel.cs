using Caliburn.Micro;
using System;
using System.Threading;
using System.Threading.Tasks;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.API;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel:Conductor<object>,IHandle<LogOnEvent>
    {
     
        private IEventAggregator _events;
        private SimpleContainer _container;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;
        private IAPIHelper _apiHelper;
        public ShellViewModel(SimpleContainer container,
            IEventAggregator events ,SalesViewModel salesVM,
            ILoggedInUserModel user , IAPIHelper apiHelper )
        {
            _events = events;
            _user = user;
            _salesVM = salesVM;
            _apiHelper = apiHelper;
            _container = container;
            _events.SubscribeOnUIThread(this);
           
            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public void ExitApplication()
        {
            TryCloseAsync();
        }
        public bool IsLoggedIn
        {
            get
            {
                bool output = false;
                if (string.IsNullOrWhiteSpace(_user.Token)==false)
                {
                    output = true;
                }
                return output;
            }
        }
        public void LogOut()
        {
            _user.ResetUserModel();
            _apiHelper.LogOfUser();
            ActivateItemAsync(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
           await ActivateItemAsync(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
