using System;

namespace Kunicardus.Core.Plugins.IUIThreadPlugin
{
	public interface IUIThreadPlugin
	{
		void InvokeUIThread (Action action);
	}
}

