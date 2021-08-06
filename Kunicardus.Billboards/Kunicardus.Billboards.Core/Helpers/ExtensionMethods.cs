using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Net;

namespace Kunicardus.Billboards.Core.Helpers
{
	public static class ExtensionMethods
	{
		public static string ToGeoString (this DateTime date)
		{
			string month = "";
			switch (date.Month) {
			case 1:
				month = "იანვარი";
				break;
			case 2:
				month = "თებერვალი";
				break;
			case 3:
				month = "მარტი";
				break;
			case 4:
				month = "აპრილი";
				break;
			case 5:
				month = "მაისი";
				break;
			case 6:
				month = "ივნისი";
				break;
			case 7:
				month = "ივლისი";
				break;
			case 8:
				month = "აგვისტო";
				break;
			case 9:
				month = "სექტემბერი";
				break;
			case 10:
				month = "ოქტომბერი";
				break;
			case 11:
				month = "ნოემბერი";
				break;
			case 12:
				month = "დეკემბერი";
				break;
			default:
				break;
			}

			return string.Format ("{0} {1}", date.Day, month);
		}

		/// <summary>
		/// Extenstion Method For Downloading Images In Base64 String.
		/// Returns Null if url string is in wrong format.
		/// </summary>
		/// <param name="url">Image url to download</param>
		public static string ToBase64String (this string url)
		{
			using (var webClient = new WebClient ()) {
				var uri = new Uri (url);
				byte[] imageBytes = null;

				try {
					imageBytes = webClient.DownloadData (uri);
					return Convert.ToBase64String (imageBytes);//DownloadDataTaskAsync(uri);
				} catch (Exception e) {
					//Toast.MakeText(parentActivity, Resource.String.errorImageLoading, ToastLength.Short).Show();
					return null;
				}

			}
		}
	}
}