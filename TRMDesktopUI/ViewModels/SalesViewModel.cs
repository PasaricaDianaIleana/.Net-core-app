using AutoMapper;
using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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
        public  SalesViewModel(IProductsEndpoint productsEndpoint,
            IConfigHelper configHelper, ISaleEndpoint saleEndpoint, IMapper mapper)
        {
            _productsEndpoint = productsEndpoint;
            _saleEndpoint = saleEndpoint;
            _configHelper = configHelper;
            _mapper = mapper;
        }

        protected  override  async void OnViewLoaded(object view)
        {
            await LoadProducts();
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
           
                return output;
            }


        }
        public void RemoveFromCart()
        {
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
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
        }

    }
}
