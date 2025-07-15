using System;
using System.Collections.Generic;
using System.Messaging;
using System.Reflection;
using Azuro.Common;
using Azuro.Common.Configuration;
using Azuro.Common.Messaging;
using Azuro.MSMQ;
using NLog;
using System.Threading;

namespace Azuro.MSMQ
{
	//  TODO: Implement Transactioned MSMQ
	//  TODO: Create an Implementation model that is Queue agnostic. That way we can implement Azure Service Bus as well, and use the same paradigm to access it.
	public class MSMQProcessor
	{
		private static Logger Logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Some kind of trickery requires this nested class structure. I think I was smarter in my twenties! :P
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public class QueueProcessor<T> where T : Azuro.Common.Messaging.SerializedMessage<T>
		{
			private static Logger Logger = LogManager.GetCurrentClassLogger();
			private static int ExceptionCount = 0;

			private IMessageHandler<T> _handler;

			private QueueProcessorConfigurationSection Config { get; set; }
			private string QueueName { get { return Config.Name; } }
			private MessageQueue Queue { get; set; }

			public event EventHandler QueueMessageProcessed;

			public IMessageHandler<T> Handler { get { return _handler ?? (_handler = MakeMessageHandler()); } }

			private IMessageHandler<T> MakeMessageHandler()
			{
				return Util.CreateObject<IMessageHandler<T>>(Config.HandlerType);
			}

			public QueueProcessor(QueueProcessorConfigurationSection qpcs)
			{
				Config = qpcs;
			}

			public void Process()
			{
				try
				{
					//	Open Queue
					Logger.Trace("Entering Process, creating Queue - [{0}]", QueueName);
					Queue = new MessageQueue(QueueName, QueueAccessMode.Receive);

					Queue.PeekCompleted += Queue_PeekCompleted;
					Queue.BeginPeek();
				}
				catch (Exception ex)
				{
					Logger.Error(ex, "Exception in Process - [{0}]", QueueName);
				}
			}

			private void Queue_PeekCompleted(object sender, PeekCompletedEventArgs e)
			{
				Logger.Trace("Entering Queue_PeekCompleted - [{0}]", QueueName);
				Message m = null;
				var q = (MessageQueue)sender;
				try
				{
					var peekMsg = q.EndPeek(e.AsyncResult);
					if (q.CanRead)
					{
						Logger.Trace("Receiving the Message - [{0}]", QueueName);
						m = q.Receive();
						Logger.Trace("Calling ProcessMessage - [{0}]", QueueName);
						ProcessMessage(m);
					}

					ExceptionCount = 0;
				}
				catch (MessageQueueException mqex)
				{
					Console.WriteLine($"MQ Exception in Queue_PeekCompleted{Environment.NewLine} {mqex.MessageQueueErrorCode} : {mqex.Message}{Environment.NewLine}{mqex.StackTrace}");
					ExceptionCount++;
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in Queue_PeekCompleted{0}{1}", Environment.NewLine, Util.UnpackException(ex));
					ExceptionCount++;
				}
				finally
				{
					//	Slow the processor down whenever an error is experienced.
					Thread.Sleep(1000 * ExceptionCount ^ 2);
					q.BeginPeek();
				}
			}

			private void ProcessMessage(Message m)
			{
				// TODO: Add Transaction Handling
				Logger.Trace("Entering ProcessMessage - [{0}]", QueueName);

				string msg = null;
				try
				{
					msg = new System.IO.StreamReader(m.BodyStream).ReadToEnd();

					Logger.Trace("New message received on [{0}] - [{1}]", QueueName, msg);

					var msgObj = Azuro.Common.Messaging.SerializedMessage<T>.Deserialize(msg);

					Logger.Trace("Calling process message for [{0}]", QueueName);

					Handler.ProcessMessage(msgObj);

					if (QueueMessageProcessed != null)
						QueueMessageProcessed(this, EventArgs.Empty);
				}
				catch (Exception ex)
				{
					Logger.Error(ex, "Exception in ProcessMessage{0}{1}{0}{2}", Environment.NewLine, ex.Message, ex.StackTrace);
					//  Push errors to the ErrorQueue
					PushErrorMessage(msg);
				}
			}

			private void PushErrorMessage(string msg)
			{
				try
				{
					if (!string.IsNullOrEmpty(Config.ErrorQueueName))
						QueueHelper.Insert(Config.ErrorQueueName, msg);
				}
				catch (Exception ex)
				{
					Logger.Error(ex, "Exception in PushErrorMessage");
				}
			}

			private void Queue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
			{
				Logger.Trace("Entering Queue_ReceiveCompleted - [{0}]", Queue.QueueName);
				Message m = null;
				try
				{
					m = Queue.EndReceive(e.AsyncResult);
					ProcessMessage(m);
				}
				catch (Exception ex)
				{
					Logger.Error(ex, "Exception in ProcessMessage{0}{1}{0}{2}", Environment.NewLine, ex.Message, ex.StackTrace);
				}
				finally
				{
					Queue.BeginReceive();
				}
			}
		}

		public event EventHandler QueueMessageProcessed;

		Dictionary<string, object> _handlers;
		Dictionary<string, object> Handlers
		{
			get { return _handlers ?? (_handlers = new Dictionary<string, object>()); }
		}

		MSMQConfigurationSection _config;
		private MSMQConfigurationSection Config
		{
			get { return _config ?? (_config = ConfigurationHelper.GetSection<MSMQConfigurationSection>(MSMQConfigurationSection.SectionName)); }
		}

		public void ProcessMessages()
		{
			foreach (QueueProcessorConfigurationSection qpcs in Config.QueueProcessorConfigurations)
			{
				LoadQueueProcessor(qpcs);
			}
		}

		private void LoadQueueProcessor(QueueProcessorConfigurationSection qpcs)
		{
			object qp = GenericTypeCreator.CreateObject("Azuro.MSMQ.MSMQProcessor+QueueProcessor`1, Azuro.MSMQ", new Type[] { qpcs.MessageType }, qpcs);

			Logger.Trace("Loading Handler: {0} - {1}", qpcs.Name, qp);
			Handlers.Add(qpcs.Name, qp);

			Type qpt = qp.GetType();

			EventInfo ei = qpt.GetEvent("QueueMessageProcessed");
			ei.AddEventHandler(qp, new EventHandler(QueueMessageProcessedHandler));

			MethodInfo mi = qpt.GetMethod("Process");
			mi.Invoke(qp, null);
		}

		public void QueueMessageProcessedHandler(object sender, EventArgs e)
		{
			if (QueueMessageProcessed != null)
				QueueMessageProcessed(sender, e);
		}
	}
}
