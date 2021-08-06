using System;
using MvvmCross.Core.ViewModels;
using System.Windows.Input;
using Kunicardus.Core.ViewModels;
using Kunicardus.Core.Plugins.UIDialogPlugin;
using Kunicardus.Core.Helpers.AppSettings;
using Kunicardus.Core.Services.Abstract;
using Kunicardus.Core.Providers.LocalDBProvider;
using Kunicardus.Core.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;


namespace Kunicardus.Core
{
	public class BaseResetPasswordViewModel : BaseViewModel
	{
		private string _email;

		public string Email {
			get{ return _email; }
			set { _email = value; }
		}

		public void Init (string email)
		{
			this.Email = email;
		}
	}
}
