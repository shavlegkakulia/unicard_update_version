using System;

namespace Kuni.Core
{
	public interface IConvertUserNameService
	{
		void ChangeUserName (string currentUsername, string emailAddres);
	}
}

