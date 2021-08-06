using System;
using MvvmCross.ViewModels;
using System.Windows.Input;
using Kuni.Core.ViewModels;
using Kuni.Core.Plugins.UIDialogPlugin;
using Kuni.Core.Helpers.AppSettings;
using Kuni.Core.Services.Abstract;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;


namespace Kuni.Core
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
