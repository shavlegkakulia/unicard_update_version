using System;

namespace Kunicardus.Core
{
	public interface IConvertUserNameService
	{
		void ChangeUserName (string currentUsername, string emailAddres);
	}
}

