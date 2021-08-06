using System;
using Kunicardus.Core.Services.Abstract;
using System.Threading.Tasks;
using Kunicardus.Core.Models;
using System.Collections.Generic;
using Kunicardus.Core.ViewModels.iOSSpecific;

namespace Kunicardus.Core.ViewModels
{
    public class OrganizationDetailsViewModel : BaseViewModel
    {
        #region Variables

        private int _organisationId;
        private IOrganizationService _organisationService;

        #endregion

        #region Constructor Implementation

        public OrganizationDetailsViewModel(IOrganizationService organisationService)
        {
            _organisationService = organisationService;
        }

        #endregion

        #region Properties

        private bool _dataPopulated;

        public bool DataPopulated
        {
            get { return _dataPopulated; }
            set
            {
                _dataPopulated = value;
                RaisePropertyChanged(() => DataPopulated);
            }
        }

        private string _imageUrl;

        public string ImageUrl
        {
            get
            {
                return (_imageUrl ?? "").Replace(@"\", "/");
            }
            set
            {
                _imageUrl = (value ?? "").Replace(@"\", "/");

                RaisePropertyChanged(() => ImageUrl);
            }
        }

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        private string _subHeading;

        public string SubHeading
        {
            get
            {
                return _subHeading;
            }
            set
            {
                _subHeading = value;
                RaisePropertyChanged(() => SubHeading);
            }
        }

        private string _description;

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        private string _unit;

        public string Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
                RaisePropertyChanged(() => Unit);
            }
        }

        private string _unitDesctiprion;

        public string UnitDescription
        {
            get { return _unitDesctiprion; }
            set
            {
                _unitDesctiprion = value;
                RaisePropertyChanged(() => UnitDescription);
            }
        }

        private string _unitScore;

        public string UnitScore
        {
            get
            {
                return _unitScore;
            }
            set
            {
                _unitScore = value;
                RaisePropertyChanged(() => UnitScore);
            }
        }

        private string _workingHours;

        public string WorkingHours
        {
            get
            {
                return _workingHours;
            }
            set
            {
                _workingHours = value;
                RaisePropertyChanged(() => WorkingHours);
            }
        }

        private string _phone;

        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                _phone = value;
                RaisePropertyChanged(() => Phone);
            }
        }

        private List<string> _phones;

        public List<string> Phones
        {
            get
            {
                return _phones;
            }
            set
            {
                _phones = value;
                RaisePropertyChanged(() => Phones);
            }
        }

        private string _mail;

        public string Mail
        {
            get
            {
                return _mail;
            }
            set
            {
                _mail = value;
                RaisePropertyChanged(() => Mail);
            }
        }

        private string _fbLink;

        public string FbLink
        {
            get
            {
                return _fbLink;
            }
            set
            {
                _fbLink = value;
                RaisePropertyChanged(() => FbLink);
            }
        }

        private string _webSite;

        public string Website
        {
            get
            {
                return _webSite;
            }
            set
            {
                _webSite = value;
                RaisePropertyChanged(() => Website);
            }
        }

        private string _numberToCall;

        public string NumberToCall
        {
            get
            {
                return _numberToCall;
            }
            set
            {
                _numberToCall = value;
                RaisePropertyChanged(() => NumberToCall);
            }
        }

        private MvvmCross.Core.ViewModels.MvxCommand<string> _itemSelectedCommand;

        public System.Windows.Input.ICommand ItemSelectedCommand
        {
            get
            {
                _itemSelectedCommand = _itemSelectedCommand ?? new MvvmCross.Core.ViewModels.MvxCommand<string>(DoSelectItem);
                return _itemSelectedCommand;
            }
        }

        private void DoSelectItem(string item)
        {
            NumberToCall = item;
        }

        #endregion

        #region Methods

        public void ShowObjects()
        {
            ShowViewModel<iMerchantsAroundMeViewModel>(new { orgId = _organisationId });
        }

        public void Init(int organisationId)
        {
            _organisationId = organisationId;
            Task.Run(() =>
            {
                PopulateData();
            });
        }

        public void InitData(int organisationId)
        {
            _organisationId = organisationId;
            Task.Run(() =>
            {
                PopulateData();
            });
        }


        private void PopulateData()
        {
            InvokeOnMainThread(() =>
            {
                _dialog.ShowProgressDialog(ApplicationStrings.Loading);
            });

            var data = _organisationService.GetOrganizationDetails(_organisationId);
            InvokeOnMainThread(() =>
            {
                _dialog.DismissProgressDialog();
            });

            if (data == null)
            {
                return;
            }
            if (!string.IsNullOrWhiteSpace(data.DisplayMessage))
            {
                InvokeOnMainThread(() =>
                {
                    _dialog.ShowToast(data.DisplayMessage);
                });
            }
            if (data.Success)
            {
                InvokeOnMainThread(() =>
                {
                    BindDataToUIFields(data.Result);
                });
            }
        }

        private void BindDataToUIFields(OrganizationDetailsModel model)
        {
            this.Description = model.ShortDescription;
            this.FbLink = model.FbAddress;
            this.ImageUrl = model.ImageUrl;
            this.Mail = model.Email;
            this.Name = model.Name;
            this.Phone = model.PhoneNumber;
            this.UnitScore = model.UnitScore;
            this.Unit = model.Unit;
            this.WorkingHours = model.WorkingHours;
            this.Website = model.Website;
            this.Phones = model.PhoneNumbers;
            this.UnitDescription = model.UnitDescription;
            DataPopulated = true;
        }

        #endregion

    }
}

