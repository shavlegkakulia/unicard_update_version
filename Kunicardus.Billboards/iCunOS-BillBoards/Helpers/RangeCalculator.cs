using System;

namespace iCunOS.BillBoards
{
	public static class RangeCalculator
	{
		public static bool InRange (decimal bearing, decimal startBearing, decimal endBearing)
		{
			return (startBearing <= bearing && endBearing >= bearing);
		}
	}
}

