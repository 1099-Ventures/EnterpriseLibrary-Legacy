using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Azuro.MSMQ
{
	public class QueueHelper
	{
		private static Logger Logger = LogManager.GetCurrentClassLogger();

		public static void Insert(string queueName, object msg)
		{
			Insert(queueName, msg, null);
		}

		public static void Insert(string queueName, object msg, Type msgType)
		{
			using (System.Messaging.MessageQueue mq = new System.Messaging.MessageQueue(queueName, System.Messaging.QueueAccessMode.Send))
			{
				using (System.Messaging.MessageQueueTransaction mqt = new System.Messaging.MessageQueueTransaction())
				{
					try
					{
						System.Messaging.Message m = null;
						if (msgType != null)
							m = new System.Messaging.Message(msg, new System.Messaging.XmlMessageFormatter(new Type[] { msgType }));
						else
							m = new System.Messaging.Message(msg);

						mqt.Begin();
						mq.Send(m, mqt);
						mqt.Commit();
						mq.Close();
					}
					catch (Exception ex)
					{
						Logger.Error(ex, "Exception Inserting the Queue Message into the [{0}] queue.", queueName);
						mqt.Abort();
					}
				}
			}
		}
	}
}
