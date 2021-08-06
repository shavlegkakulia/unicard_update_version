using System;
using Kunicardus.Core.Plugins.IUIThreadPlugin;
using UIKit;

namespace Kunicardus.Touch
{
	public class UIThreadPlugin : IUIThreadPlugin
	{
		#region IUIThreadPlugin implementation

		public void InvokeUIThread (Action action)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (action);
		}

		#endregion
	}
}

