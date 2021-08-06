using System;

namespace Kuni.Core.Helpers.Device
{
	public interface IDevice
	{
		string DeviceId { get; }

		string DocumentsPath { get; }

		string DataPath { get; }

		string Platform { get; }

		string Type { get; }

		void ResetApplication ();
	}
}

