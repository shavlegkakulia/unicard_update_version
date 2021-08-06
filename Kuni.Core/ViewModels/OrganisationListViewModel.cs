using System;
using Kuni.Core.Services.Abstract;
using System.Collections.Generic;
using Kuni.Core.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.ViewModels;
using System.Linq;
using Kuni.Core.Providers.LocalDBProvider;
using MvvmCross;
using MvvmCross.Commands;
//using MvvmCross;

namespace Kuni.Core.ViewModels
{
	public class OrganisationListViewModel : BaseViewModel
	{
		
		#region Variables

		private IOrganizationService _organizationService;
		private IGoogleAnalyticsService _gaService;

		#endregion

		#region Properties

		private bool _dataPopulated;

		public bool DataPopulated {
			get { return _dataPopulated; }
			set {
				_dataPopulated = value;
				RaisePropertyChanged (() => DataPopulated);
			}
		}

		public List<string> OrganisationNames { get; private set; }

		private List<OrganizationModel> _organisations;

		public List<OrganizationModel> Organisations {
			get { 
				return _organisations;
			}
			set {
				_organisations = value;
				RaisePropertyChanged (() => Organisations);
			}
		}

		private ICommand _itemClick;

		public ICommand ItemClick {
			get {
				_itemClick = _itemClick ?? new MvvmCross.Commands.MvxCommand<OrganizationModel> (OrgItemClick);
				return _itemClick;
			}
		}

		#endregion

		#region Constructor implementation

		public OrganisationListViewModel (IOrganizationService organisationService, IGoogleAnalyticsService gaService)
		{
			_organizationService = organisationService;
			_gaService = gaService;	
		}

		#endregion

		#region Methods

		private void OrgItemClick (OrganizationModel org)
		{
			_gaService.TrackEvent (GAServiceHelper.From.FromPartnersList, GAServiceHelper.Events.PartnersDetailClicked);
			_gaService.TrackScreen (GAServiceHelper.Page.PartnersDetails);
			NavigationCommand<OrganizationDetailsViewModel> (new{organisationId = org.OrganizationId});
		}

		public void Dispose ()
		{
			this.Dispose ();
		}

		public void Filter (string searchTerm = "", bool isRefreshing = false)
		{
			Task.Run (() => {
				GetOrganisations (isRefreshing, searchTerm);
			});
		}

		public int GetOrgId (int position)
		{
			return Organisations [position].OrganizationId;
		}

		public void GetOrganisations (bool isRefreshing, string searchTerm = "")
		{		
			using (var dbProvider = Mvx.IoCProvider.Resolve<ILocalDbProvider> ()) {
				if (!isRefreshing) {
					var query = "Select * from OrganizationModel";
					if (!string.IsNullOrEmpty (searchTerm)) {
						query += " where lower(Name) Like '%" + searchTerm.ToLower () + "%'";
					}

					Organisations = dbProvider.Query<OrganizationModel> (query);
					OrganisationNames = Organisations.Select (x => x.Name).ToList ();
					DataPopulated = true;
				}

				var data = _organizationService.GetOrganizations (1, false, int.MaxValue - 1, null, null, false);
				if (data.Success) {
					var values = data.Result.Organizations;

					dbProvider.Execute ("delete from OrganizationModel;");
					dbProvider.Insert<OrganizationModel> (values);

					InvokeOnMainThread (() => {
						if (string.IsNullOrEmpty (searchTerm))
							Organisations = values;
						else
							Organisations = values.Where (x => x.Name.ToLower ().Contains (searchTerm.ToLower ())).ToList ();
					});
				}
			}
			DataPopulated = true;
		}

		#endregion
	}

}

