using System;
using AwwScrap_ISmeltedYourCrap.Thraxus.Common.Interfaces;

namespace AwwScrap_ISmeltedYourCrap.Thraxus.Common.BaseClasses
{
	public abstract class BaseLoggingClass : ICommon
	{
		public event Action<string, string> OnWriteToLog;
		public event Action<ICommon> OnClose;

		public bool IsClosed { get; private set; }

		public virtual void Close()
		{
			if (IsClosed) return;
			IsClosed = true;
			OnClose?.Invoke(this);
		}

		public virtual void Update(ulong tick) { }

		public void WriteToLog(string caller, string message)
		{
			OnWriteToLog?.Invoke(caller, message);
		}
	}
}