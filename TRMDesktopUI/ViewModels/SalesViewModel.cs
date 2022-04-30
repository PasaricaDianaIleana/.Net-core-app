using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel:Screen
    {
        private BindingList<string> _products;

        public BindingList<string> Products
        {
            get { return _products; }
            set 
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private string _itemQuantity;

        public string ItemQuantity
        {
            get { return _itemQuantity; }
            set 
            { 
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
            }
        }

        private BindingList<string> cart;

        public BindingList<string> Cart
        {
            get { return cart; }
            set
            {
                cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        public string SubTotal
        {
            get 
            {
                //TODO-replace with calculation
                return "$0.00";
            }
     
        }
        public string Total
        {
            get
            {
                //TODO-replace with calculation
                return "$0.00";
            }

        }
        public string Tax
        {
            get
            {
                //TODO-replace with calculation
                return "$0.00";
            }

        }
        public bool CanAddTocCart
        {
            get 
            {
                bool output = false;
                //make sure that something is selected
                //make sure that is an item quantity
                return output;
            }
           
        }

        public void AddToCart()
        {

        }

        public bool CanRemoveFroCart
        {
            get
            {
                bool output = false;
                //make sure that something is selected
           
                return output;
            }

        }
        public void RemoveFromCart()
        {

        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;
                //make sure there is something in the cart
                return output;
            }

        }

        public void CheckOut()
        {

        }

    }
}
