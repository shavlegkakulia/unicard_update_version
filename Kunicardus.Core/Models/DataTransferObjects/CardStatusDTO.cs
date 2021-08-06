using System;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class CardStatusDTO:ResultDTO
	{
		public bool IsRegistered{ get; set; }

		public bool CardStatus { get; set; }

		public bool HasTransaction { get; set; }
	}

	public class ResultDTO
	{
		public int Code { get; set; }

		public string ErrorText { get; set; }

		public string DisplayString { get; set; }
	}
}

