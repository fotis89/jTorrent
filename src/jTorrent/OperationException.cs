using System;

namespace jTorrent
{
	public class OperationException : Exception
	{
		public string Reason { get; }

		public OperationException(string message)
		{
			Reason = message;
		}
	}
}
