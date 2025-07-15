using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azuro.Common.Messaging
{
	public interface IMessageHandler<T> where T : SerializedMessage<T>
	{
		void ProcessMessage(T msg);
	}
}
