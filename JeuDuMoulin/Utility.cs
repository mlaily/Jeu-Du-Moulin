using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace JeuDuMoulin
{
	public class Lock : IDisposable
	{
		//keep reference of all current locks
		private static List<Lock> currentLocks = new List<Lock>();
		private static bool releasedAll = false;

		ManualResetEvent m = new ManualResetEvent(false);
		string endLog;

		public Lock()
		{
			currentLocks.Add(this);
		}
		public Lock(string startLog, string endLog = null)
			: this()
		{
			if (releasedAll) return; //avoid further actions
			this.endLog = endLog;
			Console.WriteLine(startLog);
		}

		public void Release()
		{
			m.Set();
		}

		void IDisposable.Dispose()
		{
			if (releasedAll) return; //avoid further lockings
			m.WaitOne(); //blocking call
			currentLocks.Remove(this); //clean up
			if (endLog != null && !releasedAll) Console.WriteLine(endLog);
		}

		public static void ReleaseAll()
		{
			releasedAll = true;
			foreach (var item in currentLocks)
			{
				item.Release();
			}
		}
	}
}
