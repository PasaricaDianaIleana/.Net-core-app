using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.Library.API;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel:Screen
    {
        private IProductsEndpoint _productsEndpoint;
        private ISaleEndpoint _saleEndpoint;
        IConfigHelper _configHelper;
        IMapper _mapper;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        public  SalesViewModel(IProductsEndpoint productsEndpoint,
            IConfigHelper configHelper, ISaleEndpoint saleEndpoint, IMapper mapper,
            StatusInfoViewModel status,IWindowManager window)
        {
            _productsEndpoint = productsEndpoint;
            _saleEndpoint = saleEndpoint;
            _configHelper = configHelper;
            _mapper = mapper;
            _status = status;
            _window = window;
        }

        protected  override  async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadProducts();
            }
            catch(Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";
                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with th Sales Form.");
                    await _window.ShowDialogAsync(_status, null, settings);
                }
             else
                {
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    await _window.ShowDialogAsync(_status, null, settings);
                }
          
                await  TryCloseAsync();
            }
            
        }
        private async Task LoadProducts()
        {
            var productList = await _productsEndpoint.GetAll();
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }
        private BindingList<ProductDisplayModel> _products;

        public BindingList<ProductDisplayModel> Products
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

        private BindingList<CartItemDisplayModel> cart = new BindingList<CartItemDisplayModel>();

        private async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>();
            await LoadProducts();

            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
        }
        public BindingList<CartItemDisplayModel> Cart
        {
            get { return cart; }
            set
            {
                cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }
        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            { 
                _selectedProduct = value;
                NotifyOfPropertyChange(() => _selectedProduct);
                NotifyOfPropertyChange(() => CanAddTocCart);
            }
        }
        private CartItemDisplayModel _selectedCartItem;

        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => _selectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
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

               taxAmount = Cart
                .Where(x => x.Product.IsTaxable)
                .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

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
            CartItemDisplayModel extingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            if (extingItem != null)
            {
                extingItem.QuantityInCart += ItemQuantity;
            }
            else
            {
                CartItemDisplayModel item = new CartItemDisplayModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }
            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => CanCheckOut);
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
                if (SelectedCartItem != null && SelectedCartItem?.QuantityInCart>0)
                {
                    output = true;
                }
                return output;
            }


        }
        public void RemoveFromCart()
        {
            SelectedCartItem.Product.QuantityInStock += 1;
            if (SelectedCartItem.QuantityInCart > 1)
            {
                SelectedCartItem.QuantityInCart -= 1;
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => CanAddTocCart);
            NotifyOfPropertyChange(() => Total);
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;
                if (Cart.Count > 0)
                {
                    output = true;
                }
                return output;
            }

        }

        public async Task CheckOut()
        {
            //create a sale model and post to the API
            SaleModel sale = new SaleModel();
            foreach(var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }
           await _saleEndpoint.PostSale(sale);

            await ResetSalesViewModel();
        }

    }
}
