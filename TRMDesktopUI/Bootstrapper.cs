using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TRMDesktopUI.Helpers;
using TRMDesktopUI.Library.API;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;
using TRMDesktopUI.ViewModels;

namespace TRMDesktopUI
{
    public class Bootstrapper:BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();
        public Bootstrapper()
        {
            Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(
          PasswordBoxHelper.BoundPasswordProperty,
          "Password",
          "PasswordChanged");
        }
        protected override void Configure()
        {
            var mapper = ConfigureAutomapper();
            _container.Instance(mapper);
            _container.Instance(_container)
                .PerRequest<IProductsEndpoint,ProductsEndpoint>()
                .PerRequest<ISaleEndpoint,SaleEndpoint>()
                .PerRequest<IUserEndpoint,UserEndpoint>();
            
            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IAPIHelper,APIHelper>()
                .Singleton<ILoggedInUserModel,LoggedInUserModel>()
                .Singleton<IConfigHelper, ConfigHelper>();

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }
        private IMapper ConfigureAutomapper()
        {

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ProductModel, ProductDisplayModel>();
                cfg.CreateMap<CartItemModel, CartItemDisplayModel>();
            });
            var output = config.CreateMapper();

            return output;
        }
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //on start up launch SgellViewModel as base view
            DisplayRootViewFor<ShellViewModel>();
        }
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
