using Caliburn.Micro;

using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TRMDesktopUI.Library.API;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel:Screen
    {
        private IProductsEndpoint _productsEndpoint;
        IConfigHelper _configHelper;
        public  SalesViewModel(IProductsEndpoint productsEndpoint, IConfigHelper configHelper)
        {
            _productsEndpoint = productsEndpoint;
            _configHelper = configHelper;
        }

        protected  override  async void OnViewLoaded(object view)
        {
            await LoadProducts();
        }
        private async Task LoadProducts()
        {
            var productList = await _productsEndpoint.GetAll();
            Products = new BindingList<ProductModel>(productList);
        }
        private BindingList<ProductModel> _products;

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set 
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private int _itemQuantity = 1;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set 
            { 
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddTocCart);
            }
        }

        private BindingList<CartItemModel> cart = new BindingList<CartItemModel>();

        public BindingList<CartItemModel> Cart
        {
            get { return cart; }
            set
            {
                cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }
        private ProductModel _selectedProduct;

        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            { 
                _selectedProduct = value;
                NotifyOfPropertyChange(() => _selectedProduct);
                NotifyOfPropertyChange(() => CanAddTocCart);
            }
        }

        public string SubTotal
        {
            get 
            {
                return CalculateSubTotal().ToString("C");
            }
     
        }

        private decimal CalculateSubTotal()
        {
            decimal subtTotal = 0;
            foreach (var item in Cart)
            {
                subtTotal += (item.Product.RetailPrice * item.QuantityInCart);
            }
            return subtTotal;
        }
        private decimal CalculateTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate()/100;
            foreach (var item in Cart)
            {
                if (item.Product.IsTaxable)
                {
                    taxAmount += (item.Product.RetailPrice * item.QuantityInCart * taxRate);
                }
            }

            return taxAmount;
        }
        public string Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalculateTax();
                return total.ToString("C");
            }

        }
        public string Tax
        {
            get
            {
                return CalculateTax().ToString();
            }

        }
        public bool CanAddTocCart
        {
            get 
            {
                bool output = false;
                //make sure that something is selected
                //make sure that is an item quantity
                if(ItemQuantity >0 && SelectedProduct?.QuantityInStock >= ItemQuantity )
                {
                    output = true;
                }
                return output;
            }
           
        }

        public void AddToCart()
        {
            CartItemModel extingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            if (extingItem != null)
            {
                extingItem.QuantityInCart += ItemQuantity;
                Cart.Remove(extingItem);
                Cart.Add(extingItem);
            }
            else
            {
                CartItemModel item = new CartItemModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }
            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => extingItem.DisplayText);
        }

        public bool CanRemoveFromCart
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
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
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
