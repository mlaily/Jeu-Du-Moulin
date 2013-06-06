﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JeuDuMoulin
{
	public static class Logging
	{
		public static TextBox TextBoxLog { get; set; }
		public static void Log(string format, params object[] args)
		{
			if (TextBoxLog != null)
			{
				TextBoxLog.Invoke((Action)(() => TextBoxLog.AppendText(string.Format(format, args) + "\n")));
			}
			else
			{
				Console.WriteLine(format, args);
			}
		}
	}
}
