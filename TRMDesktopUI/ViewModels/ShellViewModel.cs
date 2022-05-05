using Caliburn.Micro;
using System;
using System.Threading;
using System.Threading.Tasks;
using TRMDesktopUI.EventModels;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel:Conductor<object>,IHandle<LogOnEvent>
    {
     
        private IEventAggregator _events;
        private SimpleContainer _container;
        private SalesViewModel _salesVM;
        public ShellViewModel(SimpleContainer container,
            IEventAggregator events ,SalesViewModel salesVM)
        {
            _events = events;
        
            _salesVM = salesVM;
            _container = container;
            _events.SubscribeOnUIThread(this);
           
            ActivateItemAsync(_container.GetInstance<LoginViewModel>());
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
           await ActivateItemAsync(_salesVM);
        }
    }
}
