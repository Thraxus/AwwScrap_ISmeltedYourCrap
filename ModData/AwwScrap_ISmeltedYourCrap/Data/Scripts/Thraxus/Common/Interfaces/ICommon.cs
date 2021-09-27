using System;

namespace AwwScrap_ISmeltedYourCrap.Thraxus.Common.Interfaces
{
	public interface ICommon
	{
		event Action<ICommon> OnClose;
		event Action<string, string> OnWriteToLog;

		void Update(ulong tick);

		bool IsClosed { get; }

		void Close();

		void WriteToLog(string caller, string message);
	}
}
