using MvvmCross.Plugin.Color;
using Kuni.Core.Plugins;
using System;
using System.Globalization;
using System.Text;
using MvvmCross.Converters;
using MvvmCross;
//using MvvmCross.UI;
using MvvmCross.UI;

namespace Kuni.Core.Converters
{
	public class PointsValueConverter : MvxValueConverter<double, string>
	{
		protected override string Convert (double value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value > 0) {
				return string.Format ("+{0}", value).Replace (",", ".");
			}
			return value.ToString ();
		}
	}

	public class DateValueConverter : MvxValueConverter<DateTime, string>
	{
		protected override string Convert (DateTime value, Type targetType, object parameter, CultureInfo culture)
		{
			return DateConverter.Convert (value);
		}
	}

	public class TransactionsDateValueConverter : MvxValueConverter<DateTime, string>
	{
		protected override string Convert (DateTime value, Type targetType, object parameter, CultureInfo culture)
		{
			return value.ToString ("dd/MM/yyyy HH:mm");
		}
	}

	public class NewsDateValueConverter : MvxValueConverter<DateTime, string>
	{
		protected override string Convert (DateTime value, Type targetType, object parameter, CultureInfo culture)
		{
			string month = "";

			switch (value.Month) {
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

			return string.Format ("{0} {1}, {2}", value.Day, month, value.Year);
		}
	}

	public class StringValidationValueConverter : MvxValueConverter<string, string>
	{
		protected override string Convert (string value, Type targetType, object parameter, CultureInfo culture)
		{
			var shouldValidate = (bool)parameter;
			if (shouldValidate && string.IsNullOrWhiteSpace (value)) {
				return "Field is empty";
			} else {
				return string.Empty;
			}
		}
	}

	public class ProductPercentValueConverter : MvxValueConverter<int, bool>
	{
		protected override bool Convert (int value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value <= 0);
		}
	}
	//	public class SayPleaseVisibilityValueConverter : MvxBaseVisibilityValueConverter<string>
	//	{
	//		protected override MvxVisibility Convert(string value, object parameter, CultureInfo culture)
	//		{
	//			return (value == "Please) ? MvxVisibility.Visible : MvxVisibility.Collapsed;
	//		}
	//				}
	public class DistanceValueConverter : MvxValueConverter<int, string>
	{
		protected override string Convert (int value, Type targetType, object parameter, CultureInfo culture)
		{
			var distance = (int)value;
			if (distance < 1000) {
				return value.ToString ();
			} else if (distance >= 1000 && distance < 5000) {
				return ">1";
			} else if (distance >= 5000 && distance < 10000) {
				return ">5";
			} else if (distance >= 10000 && distance < 25000) {
				return ">10";
			} else if (distance >= 25000 && distance < 50000) {
				return ">25";
			} else if (distance >= 50000 && distance < 100000) {
				return ">50";
			} else {

				int dist = distance / 100000;
				return ">" + dist * 100;
			}
		}
	}

	public class UnitValueConverter : MvxValueConverter<string, string>
	{
		protected override string Convert (string value, Type targetType, object parameter, CultureInfo culture)
		{
			var distance = (double)parameter;
			if (distance < 1000) {
				return "მეტრი";
			} else {
				return "კმ";
			}
		}
	}

	public class ApiDateTimeValueConverter : MvxValueConverter<DateTime, string>
	{
		protected override string Convert (DateTime value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return "";
			return value.ToString (Constants.ApiDateTimeFormat);
		}
	}

	public class iOSPointsColorValueConverter : MvxColorValueConverter
	{
        protected override MvvmCross.UI.MvxColor Convert(object value, object parameter, CultureInfo culture)
        {
            double points = (double)value;

            if (points < 0)
            {
                return new MvvmCross.UI.MvxColor(242, 142, 45);
            }
            else
            {
                return new MvxColor(140, 189, 58);
            }
        }
    }

	public class NewsIsReadColorConverter : MvxColorValueConverter
	{
		protected override MvxColor Convert (object value, object parameter, CultureInfo culture)
		{
			bool isRead = (bool)value;

			if (isRead) {
				return new MvxColor (146, 145, 145);
			} else {
				return new MvxColor (0, 0, 0);
			}
		}
	}

	public class PointsColorValueConverter : MvxColorValueConverter
	{
		protected override MvxColor Convert (object value, object parameter, CultureInfo culture)
		{
			double points = (double)parameter;

			if (points < 0) {
				return new MvxColor (242, 142, 45);
			} else {
				return new MvxColor (140, 189, 58);
			}
		}
	}

	public class iOSValidationColorValueConverter : MvxColorValueConverter
	{
		private bool _shoudlValidate = false;

		public bool ShouldValidate {
			get {
				return _shoudlValidate;
			}
			set {
				_shoudlValidate = value;
			}
		}

		protected override MvxColor Convert (object value, object parameter, CultureInfo culture)
		{			
			if (ShouldValidate && (value == null || string.IsNullOrWhiteSpace (value.ToString ()))) {
				// red;
				return new MvxColor (255, 109, 64);
			} else {
				// green
//				if (!_shoudlValidate) {
//					_shoudlValidate = true;
//				}
				return new MvxColor (185, 240, 80);
			}						
		}
	}

	public class ValidationColorValueConverter : MvxColorValueConverter
	{
		protected override MvxColor Convert (object value, object parameter, CultureInfo culture)
		{
			var shouldValidate = (bool)parameter;
			if (shouldValidate && (value == null || string.IsNullOrWhiteSpace (value.ToString ()))) {
				
				// red;
				return new MvxColor (255, 109, 64);
			} else {
				// green
				return new MvxColor (185, 240, 80);
			}						
		}
	}

	public class CardNumberValidationColorValueConverter : MvxColorValueConverter
	{
		protected override MvxColor Convert (object value, object parameter, CultureInfo culture)
		{
			var shouldValidate = (bool)parameter;
			if (shouldValidate
			    &&
			    (value == null
			    ||
			    string.IsNullOrWhiteSpace (value.ToString ()))) {

				// red;
				return new MvxColor (255, 109, 64);
			} else if (shouldValidate
			           && value != null && value.ToString ().Length != 4) {
				// red;
				return new MvxColor (255, 109, 64);
			} else {
				// green
				return new MvxColor (185, 240, 80);
			}						
		}
	}

	public class CardnumberDisplayValueConverter : MvxValueConverter<string, string>
	{
		protected override string Convert (string value, Type targetType, object parameter, CultureInfo culture)
		{
			var builder = new StringBuilder ();
			int count = 0;
			foreach (var c in value) {
				builder.Append (c);
				if ((++count % 4) == 0) {
					builder.Append (' ');
				}
			}
			return builder.ToString ();
		}
	}
}
