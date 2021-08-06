using System;

namespace Kuni.Core.Plugins.IUIThreadPlugin
{
	public interface IUIThreadPlugin
	{
		void InvokeUIThread (Action action);
	}
}

