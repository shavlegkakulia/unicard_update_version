using System;

namespace Kuni.Core.Plugins.UIDialogPlugin
{
	public interface IUIDialogPlugin
	{
		void ShowToast (string message);

		void ShowProgressDialog (string message);

		void DismissProgressDialog ();

	}
}

