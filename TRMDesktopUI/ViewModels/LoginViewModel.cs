using Caliburn.Micro;
using System;
using System.Threading.Tasks;
using TRMDesktopUI.Library.API;

namespace TRMDesktopUI.ViewModels
{
    public class LoginViewModel:Screen
    {
       
        private string _userName;
        private string _password;
        private readonly IAPIHelper _apiHelper;
        public LoginViewModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper ;
        }
        public string UserName
        {
            get { return _userName; }
            set {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
               NotifyOfPropertyChange(() => CanLogIn);
            }
        }
       
        public string Password
        {
            get { return _password; }
            set
            { 
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }
        //private bool _isErrorVisible;

        public bool IsErrorVisible
        {
            get {

                bool output = false;
                if (ErrorMessage?.Length > 0)
                {
                    output = true;
                }
                return output;
            }
        
        }

        public bool CanLogIn
        {
            get
            {
                bool output = false;
                if (UserName?.Length > 0 && Password?.Length > 0)
                {
                    output = true;
                }
                return output;
            }
            set
            {

            }
        }
        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set {
           
                
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
                NotifyOfPropertyChange(() => IsErrorVisible);
            }
        }

        public async Task LogIn()
        {
            try
            {
                ErrorMessage = "";
                var result = await _apiHelper.Authenticate(UserName, Password);

                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);
                //capture more information about user
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                
            }
        }
    }
}
