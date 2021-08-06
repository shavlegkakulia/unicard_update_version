using System;
using MvvmCross;
using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;

namespace Kunicardus.Core.Plugins.Connectivity
{
	public class PluginLoader : IMvxPluginLoader
	{
		//ncrunch: no coverage start
		public static readonly PluginLoader Instance = new PluginLoader ();

		public void EnsureLoaded ()
		{
			var manager = Mvx.Resolve<IMvxPluginManager> ();
			manager.EnsurePlatformAdaptionLoaded<PluginLoader> ();
		}
		//ncrunch: no coverage end
	}
}

