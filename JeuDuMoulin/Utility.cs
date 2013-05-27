using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace JeuDuMoulin
{
	public interface IReturnValue
	{

	}

	public class EmergencyReturn : IReturnValue
	{
		private static EmergencyReturn _Instance;
		public static EmergencyReturn Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = new EmergencyReturn();
				}
				return _Instance;
			}
		}
		private EmergencyReturn() { }
	}

	public class Future<T> where T : IReturnValue
	{
		public static readonly Future<T> CancelledInstance = new Future<T>(default(T)) { Cancelled = true };

		public bool Cancelled { get; private set; }
		public T Value { get; private set; }
		public Future(T value)
		{
			this.Value = value;
		}
	}

	public class LockAndReturn<T> where T : IReturnValue
	{
		//keep reference of all current locks
		private static List<LockAndReturn<T>> currentLocks = new List<LockAndReturn<T>>();
		private static bool releasedAll = false;

		private ManualResetEvent m = new ManualResetEvent(false);
		private string endLog;
		private Future<T> returnValue;

		public LockAndReturn(string startLog = null, string endLog = null)
		{
			if (releasedAll) return; //avoid further actions
			currentLocks.Add(this);
			this.endLog = endLog;
			if (startLog != null) Console.WriteLine(startLog);
		}

		public void Release(T returnValue)
		{
			this.returnValue = new Future<T>(returnValue);
			m.Set();
		}

		//force exit
		private void Release()
		{
			this.returnValue = Future<T>.CancelledInstance;
			m.Set();
		}

		public Future<T> WaitFor()
		{
			if (releasedAll) return Future<T>.CancelledInstance; //avoid further lockings
			m.WaitOne(); //blocking call
			currentLocks.Remove(this); //clean up
			if (endLog != null && !releasedAll) Console.WriteLine(endLog);
			return this.returnValue;
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
