using System;

namespace Kuni.Core
{
	public static class GAServiceHelper
	{
		public static class Events
		{
			public static string AppEvent { get { return "AppEvent"; } }

			public static string MapClicked { get { return "Map Clicked"; } }

			public static string CatalogClicked { get { return "Catalog Clicked"; } }

			public static string NewsClicked { get { return "News Clicked"; } }

			public static string CatalogDetailClicked { get { return "CatalogDetail Clicked"; } }

			public static string PartnersDetailClicked { get { return "PartnerDetails Clicked"; } }

			public static string Authorization { get { return "Authorization"; } }

			public static string FBAuthorization { get { return "FB Authorization"; } }

			public static string CardClicked { get { return "Card Clicked"; } }

			public static string SetPin { get { return "Set Pin"; } }

			public static string RemovePin { get { return "Remove Pin"; } }

			public static string ChangePin { get { return "Change Pin"; } }

			public static string ApplicationStart { get { return "Application Start"; } }

		}

		public static class From
		{
			public static string Application { get { return "Application"; } }

			public static string FromPartnersDetails { get { return "from partnerDetails"; } }

			public static string FromPartnersList { get { return "from partners list"; } }

			public static string FromHomePage { get { return "from homePage"; } }

			public static string FromNotifications { get { return "from notifications"; } }

			public static string FromMenu { get { return "from menu"; } }

			public static string FromMap { get { return "from map"; } }

			public static string FromDialog { get { return "from dialog"; } }

			public static string FromCatalogList { get { return "from catalogList"; } }

			public static string Settings { get { return "from settings"; } }

		}

		public static class Page
		{
			public static string CatalogDetail { get { return "CatalogDetail"; } }

			public static string PartnersDetails { get { return "PartnerDetails"; } }

			public static string LoginAuthPage { get { return "AuthPage"; } }
		}

		public enum Pagenames:int
		{
			HomePage = 0,
			MyPage = 1,
			Catalog = 2,
			AroundMe = 3,
			Partners = 4,
			News = 5,
			AboutUs = 6,
			Settings = 7,
			LogInPage = 8
		}
	}
}

