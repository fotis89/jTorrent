using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
