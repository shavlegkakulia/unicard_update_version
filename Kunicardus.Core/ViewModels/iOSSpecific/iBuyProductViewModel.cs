using System;
using Kunicardus.Core.Models;
using System.Collections.Generic;
using Kunicardus.Core.Services.Abstract;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Models.DB;
using System.Linq;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Kunicardus.Core.ViewModels.iOSSpecific
{
	public class iBuyProductViewModel : BaseViewModel
	{
		
		#region Properties

		private string _currentImageUrl;

		public string CurrentImageUrl {
			get{ return _currentImageUrl; }
			set {
				_currentImageUrl = value;
				RaisePropertyChanged (() => CurrentImageUrl);
			}
		}

		private int _productId;

		public int ProductID { 
			get{ return _productId; } 
			set {
				_productId = value;
				RaisePropertyChanged (() => ProductID);
			}
		}


		private int _productTypeId;

		public int ProductTypeID { 
			get{ return _productTypeId; } 
			set {
				_productTypeId = value;
				RaisePropertyChanged (() => ProductTypeID);
			}
		}

		private int _deliveryMethodId;

		public int DeliveryMethodId { 
			get{ return _deliveryMethodId; } 
			set {
				_deliveryMethodId = value;
				RaisePropertyChanged (() => DeliveryMethodId);
			}
		}

		private string _productName;

		public string ProductName { 
			get{ return _productName; } 
			set {
				_productName = value;
				RaisePropertyChanged (() => ProductName);
			}
		}

		private decimal _basePrice;
		private decimal _productPrice;

		public decimal ProductPrice { 
			get {
				_productPrice = _basePrice;
				if (SelectedDiscount != null && _productPrice != null) {
					_productPrice = _productPrice - _productPrice * SelectedDiscount.DiscountPercent / 100;
				}
				return _productPrice;
			} 
			set {
				_productPrice = value;
				RaisePropertyChanged (() => ProductPrice);
			}
		}

		private string _userAddress;

		public string UserAddress { 
			get{ return _userAddress; } 
			set {
				_userAddress = value;
				RaisePropertyChanged (() => UserAddress);
			}
		}

		private List<DiscountModel> _userDiscounts;

		public List<DiscountModel> UserDiscounts { 
			get {
				//				var l = new List<DiscountModel> ();
				//
				//				l.Add (new DiscountModel {
				//					DiscountID = 1,
				//					DiscountDescription = "test",
				//					DiscountPercent = 20
				//				});
				//
				//				l.Add (new DiscountModel {
				//					DiscountID = 2,
				//					DiscountDescription = "test 2",
				//					DiscountPercent = 10
				//				});
				return _userDiscounts;
			}
			set {
				_userDiscounts = value;
				RaisePropertyChanged (() => UserDiscounts);
				RaisePropertyChanged (() => HasDiscount);
			}
		}

		private bool _hasDiscount;

		public bool HasDiscount { 
			get {
				if (UserDiscounts == null) {
					return false;
				}
				return UserDiscounts.Count > 0;
			}
			set {
				_hasDiscount = value;
				RaisePropertyChanged (() => HasDiscount);
			}
		}

		private DiscountModel _selectedDiscount;

		public DiscountModel SelectedDiscount { 
			get{ return _selectedDiscount; }
			set {
				_selectedDiscount = value;
				//update value
				RaisePropertyChanged (() => SelectedDiscount);
				RaisePropertyChanged (() => ProductPrice);
			}
		}

		private int _selecteDPostion;

		public int SelecteDPostion {
			get { return _selecteDPostion; } 
			set {
				_selecteDPostion = value; 
				RaisePropertyChanged (() => ProductPrice);
			}
		}

		private List<ServiceCenterDTO> _serviceCenters;

		public List<ServiceCenterDTO> ServiceCenters { 
			get{ return _serviceCenters; }
			set {
				_serviceCenters = value;
				RaisePropertyChanged (() => UserDiscounts);
			}
		}

		private ServiceCenterDTO _selectedSCenter;

		public ServiceCenterDTO SelectedSCenter { 
			get{ return _selectedSCenter; }
			set {
				_selectedSCenter = value;
				RaisePropertyChanged (() => SelectedSCenter);
			}
		}

		private List<OrganizationModel> _organizations;

		public List<OrganizationModel> Organizations { 
			get{ return _organizations; }
			set {
				_organizations = value;
				RaisePropertyChanged (() => Organizations);
			}
		}

		private OrganizationModel _selectedOrg;

		public OrganizationModel SelectedOrg { 
			get{ return _selectedOrg; }
			set {
				_selectedOrg = value;
				RaisePropertyChanged (() => SelectedOrg);
			}
		}

		private string _firstName;

		public string FirstName { 
			get{ return _firstName; }
			set {
				_firstName = value;
				RaisePropertyChanged (() => FirstName);
			}
		}

		private string _lastName;

		public string LastName { 
			get{ return _lastName; }
			set {
				_lastName = value;
				RaisePropertyChanged (() => LastName);
			}
		}

		private String _userID;

		public String UserID { 
			get{ return _userID; } 
			set {
				_userID = value;
				RaisePropertyChanged (() => UserID);
			}
		}

		private string _personalNumber;

		public string PersonalNumber { 
			get{ return _personalNumber; }
			set {
				_personalNumber = value;
				RaisePropertyChanged (() => PersonalNumber);
			}
		}

		private string _onlinePaymentIdentifier;

		public string OnlinePaymentIdentifier { 
			get{ return _onlinePaymentIdentifier; }
			set {
				_onlinePaymentIdentifier = value;
				RaisePropertyChanged (() => OnlinePaymentIdentifier);
			}
		}

		private List<PaymentInfoDTO> _paymentInfos;

		public List<PaymentInfoDTO>  PaymentInfos { 
			get{ return _paymentInfos; }
			set {
				_paymentInfos = value;
				RaisePropertyChanged (() => PaymentInfos);
			}
		}

		private int _discountId;

		public int DiscountId {
			get{ return _discountId; }
			set {
				_discountId = value;
				RaisePropertyChanged (() => DiscountId);
			}
		}

		private decimal _score;

		public decimal Score {
			get{ return _score; }
			set {
				_score = value;
				RaisePropertyChanged (() => Score);
			}
		}

		//string phonePattern = "({0}) {1}-{2}";
		string phonePattern = "{0}{1}{2}";
		private string _phone;

		public string PhoneNumber {
			get { 
//				if (string.IsNullOrEmpty (_phone) || _phone.Length < 9) {
//					return _phone;
//				}
//				var result = string.Format (phonePattern, _phone.Substring (0, 3), _phone.Substring (3, 3), _phone.Substring (6, 3));
//				return result;
				return _phone;
			}
			set {
//				if (!string.IsNullOrEmpty (value)) {
//					//					_phone = Regex.Match (value.Trim (), @"\d+").Value;
//					var getNumbers = (from t in value.ToCharArray ()
//					                  where char.IsDigit (t)
//					                  select t).ToArray ();
//					_phone = new string (getNumbers);
//				} else {
//					_phone = value;
//				}
				_phone = value;
				RaisePropertyChanged (() => PhoneNumber);
			}
		}

		//		private string _fullName;

		public string FullName {
			get {
				return  string.Format ("{0} {1}", FirstName, LastName);
			}
		}

		private string _displayText;

		public string DisplayText {
			get{ return _displayText; }
			set {
				_displayText = value;
				RaisePropertyChanged (() => DisplayText);
			}
		}

		private string _note;

		public string Note {
			get{ return _note; }
			set {
				_note = value;
				RaisePropertyChanged (() => Note);
			}
		}

		private string _indentifierTitle;

		public string IndentifierTitle {
			get{ return _indentifierTitle; }
			set {
				_indentifierTitle = value;
				RaisePropertyChanged (() => DisplayText);
			}
		}

		private bool _dataChanged;

		public bool DataChanged {
			get{ return _dataChanged; }
			set {
				_dataChanged = value;
				RaisePropertyChanged (() => DataChanged);
			}
		}


		private bool _checkClicked;

		public bool CheckClicked {
			get{ return _checkClicked; }
			set {
				_checkClicked = value;
				RaisePropertyChanged (() => CheckClicked);
			}
		}

		#endregion

		#region Commands

		private ICommand _checkInfoClicked;

		public ICommand CheckInfoClickedCommand {
			get { 
				_checkInfoClicked = _checkInfoClicked ?? new MvxCommand (CheckPaymentInfo);
				return _checkInfoClicked;
			}
		}

		private ICommand _buyProductCommand;

		public ICommand BuyProductCommand {
			get { 
				_buyProductCommand = _buyProductCommand ?? new MvxCommand (BuyProduct);
				return _buyProductCommand;
			}
		}

		#endregion

		#region Vars

		private Guid _operationID;

		private IPaymentService _paymentService;
		private ILocalDbProvider _localDb;

		public bool IsOnlinePayment  { get; set; }

		#endregion

		#region Ctors

		public iBuyProductViewModel (
			IPaymentService paymentService,
			ILocalDbProvider localDb)
		{
			_paymentService = paymentService;
			_localDb = localDb;

			//init param
			_selecteDPostion = -1;
			_operationID = Guid.NewGuid ();
		}

		#endregion

		#region Init

		public void Init (string currentImageUrl, string productName, int productID, 
		                  int productPrice, int deliveryMethodId, int productTypeID, 
		                  string note, string discounts)
		{
			CurrentImageUrl = currentImageUrl;
			ProductName = productName;
			ProductPrice = _basePrice = productPrice;
			DeliveryMethodId = deliveryMethodId;
			ProductTypeID = productTypeID;
			ProductID = productID;
			Note = note;
			UserDiscounts = JsonConvert.DeserializeObject<List<DiscountModel>> (discounts);
			IsOnlinePayment = false;
			InitContentViewByType ();
		}

		#endregion

		#region Methods

		private void InitContentViewByType ()
		{
			var userInfo = _localDb.Get<UserInfo> ().FirstOrDefault ();
			FirstName = userInfo.FirstName;
			LastName = userInfo.LastName;
			UserID = userInfo.UserId;
			PhoneNumber = userInfo.Phone;
			PersonalNumber = userInfo.PersonalId;
			UserAddress = userInfo.FullAddress;


			// ადგილზე მიტანა - 3
			// მომენტალური მიღება 4
			// მომენტალური გადახდა 10
			switch (DeliveryMethodId) {

			// პარტნიორი ორგანიზაციიდან გატანა 2
			// სერვის ცენტრიდან 1
			// შოუ რუმიდან 5
			case 2:
			case 1:
			case 5:
				//				if (DeliveryMethodId == 2) {
				//					var org = _organizationService.GetOrganizations (1, false, int.MaxValue, null, null, false);
				//					Organizations = org.Result.Organizations;	
				//				} else {
				ServiceCenters = new List<ServiceCenterDTO> ();
				ServiceCenters.Add (new ServiceCenterDTO { ID = -1, Name = "აირჩიეთ" });
				var data = _paymentService.GetServiceCenters (this.ProductID, DeliveryMethodId);
				ServiceCenters.AddRange (data.Result.Result.ServiceCenters);
				break;
			case 4: 
				IsOnlinePayment = true;
				switch (ProductTypeID) {
				case 1:
					IndentifierTitle = "შეიყვანეთ ჯარიმის ნომერი";
					break;
				case 2:
					IndentifierTitle = "შეიყვანეთ მანქანის ნომერი";
					break;
				case 8:
					IndentifierTitle = "შეიყვანეთ აბონენტის ნომერი";
					break;
				default:
					break;
				}
				break;
			default:
				break;
			}
		}

		void CheckPaymentInfo ()
		{
			CheckClicked = true;
			var result = _paymentService.GetOnlinePaymentInfo (ProductID, UserID, OnlinePaymentIdentifier);

			if (result.Result != null) {
				this.Score = string.IsNullOrEmpty (result.Result.Result.DebitScore) ? 0 : int.Parse (result.Result.Result.DebitScore);
				//				this.ProductPrice = score;
				this.PaymentInfos = result.Result.Result.PaymentInfos;
				this.DataChanged = true;
				//extra check
				IsOnlinePayment = true;
			}

		}

		void BuyProduct ()
		{
			if ((IsOnlinePayment && Score == 0) && ProductPrice == 0) {
				this.DisplayText = "ოპერაცია არ განხორციელდა, პროდუქტზე ქულების რაოდენობა არის 0";
				InvokeOnMainThread (() => {
					_dialog.ShowToast (this.DisplayText);
				});
				return;
			}
			if (DeliveryMethodId == (int)DeliveryMethidIds.Fetch && string.IsNullOrEmpty (UserAddress)) {
				this.DisplayText = "შეავსეთ მისამართი";
				InvokeOnMainThread (() => {
					_dialog.ShowToast (this.DisplayText);
				});
				return;
			}
			//mobile
			if (DeliveryMethodId == (int)DeliveryMethidIds.OnlinePayment
			    && ProductTypeID == (int)ProductType.OnlinePayment
			    && string.IsNullOrEmpty (_phone)) {
				this.DisplayText = "შეავსეთ მობილურის ნომერი";
				InvokeOnMainThread (() => {
					_dialog.ShowToast (this.DisplayText);
				});
				return;
			} 
			if (DeliveryMethodId == (int)DeliveryMethidIds.OnlinePayment
			    && ProductTypeID != (int)ProductType.OnlinePayment
			    && string.IsNullOrEmpty (OnlinePaymentIdentifier)) {
				this.DisplayText = "შეავსეთ ნომერი";
				InvokeOnMainThread (() => {
					_dialog.ShowToast (this.DisplayText);
				});
				return;
			}
			if ((DeliveryMethodId == (int)DeliveryMethidIds.ShowRoom
			    || DeliveryMethodId == (int)DeliveryMethidIds.ServiceCenter
			    || DeliveryMethodId == (int)DeliveryMethidIds.PartnerOrganization)
			    && (string.IsNullOrEmpty (PersonalNumber)
			    || string.IsNullOrEmpty (FullName)
			    || SelectedSCenter.ID == -1)) {

				//პარტნიორი ორგანიზაციიდან გატანა 2
				//სერვის ცენტრიდან 1
				//შოუ რუმიდან 5

				this.DisplayText = "შეავსეთ ყველა ველი";
				InvokeOnMainThread (() => {
					_dialog.ShowToast (this.DisplayText);
				});
				return;
			}
			var serviceid = SelectedSCenter != null ? (int?)SelectedSCenter.ID : 0;

			var result = _paymentService.BuyProduct (ProductID, UserID,
				             ProductTypeID,
				             DeliveryMethodId,
				             SelectedDiscount != null ? SelectedDiscount.DiscountID : 0,
				             UserAddress ?? "",
				             0,
				             IsOnlinePayment ? Score : (SelectedDiscount != null ? _basePrice : ProductPrice),
				             "",
				             OnlinePaymentIdentifier ?? "",
				             _phone ?? OnlinePaymentIdentifier,
				             FullName,
				             PersonalNumber,
				             serviceid,
				             _operationID,
				             DateTime.Now,
				             DateTime.Now

			             );
			DisplayText = result.Result.DisplayMessage;
			if (result.Result.Success) {
				BuySuccess = true;
			}
			InvokeOnMainThread (() => {
				_dialog.ShowToast (result.Result.DisplayMessage);
			});
		}

		private bool _buySuccess;

		public bool BuySuccess {
			get {
				return _buySuccess;
			}
			set {
				_buySuccess = value;
				RaisePropertyChanged (() => BuySuccess);
			}
		}

		#endregion
	}
}

