using System;

namespace Kunicardus.Touch.Helpers.UI
{
	public static class Styles
	{
		public static readonly nfloat RegistrationFormSubHeadingTop = 100f;
// - 64f;

		public static class Colors
		{
			public static readonly string HeaderGreen = "#8dbd3b";
			public static readonly string LoginScreenBackGroundGreen = "#98cc42";
			public static readonly string PlaceHolderColor = "#CDF77C";
			public static readonly string Yellow = "#ffe154";
			public static readonly string Gray = "#ABABAB";
			public static readonly string LightGray = "#e6eced";
			public static readonly string Orange = "#f28e2e";
		}

		public static class Fonts
		{
			public static readonly string BPGExtraSquare = "BPG ExtraSquare Mtavruli";
		}

		public static class RegistrationNextButton
		{
			public static readonly nfloat Width = 90;
			public static readonly nfloat bottom = (Screen.IsTall ? 30 : 16);
		}
	}
}

