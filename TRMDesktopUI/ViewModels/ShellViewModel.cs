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
        private ILoggedInUserModel _user;
        private IAPIHelper _apiHelper;
        public ShellViewModel(SimpleContainer container,
            IEventAggregator events,ILoggedInUserModel user , IAPIHelper apiHelper )
        {
            _events = events;
            _user = user;
            _apiHelper = apiHelper;
            _container = container;
            _events.SubscribeOnUIThread(this);
           
            ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());
        }

        public void ExitApplication()
        {
            TryCloseAsync();
        }
        public  async Task  UserManagement()
        {
            await ActivateItemAsync(IoC.Get<UserDisplayViewModel>(), new CancellationToken());
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
        public async Task LogOut()
        {
            _user.ResetUserModel();
            _apiHelper.LogOfUser();
           await ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
           await ActivateItemAsync(IoC.Get<SalesViewModel>(), cancellationToken);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        
    }
}
