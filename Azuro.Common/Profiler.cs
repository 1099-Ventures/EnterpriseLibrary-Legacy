using System;
using System.Diagnostics;

namespace Azuro.Common
{
	/// <summary>
	/// This is a little helper class that can be used for performance profiling.
	/// Simply place it in a using statement around the section of code to be profiled.
	/// </summary>
	/// <code></code>
	public class Profiler : IDisposable
	{
		DateTime m_startTime;
		string m_timerName = "Some Timer";
		string m_filePath;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name for this profiler instance.</param>
		public Profiler(string name)
		{
			StartTiming(name);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name for this profiler instance.</param>
		/// <param name="path">A file path to which profiling data is to be written.</param>
		public Profiler(string name, string path)
		{
			FilePath = path;
			StartTiming(name);
		}

		/// <summary>
		/// The File path to which output should be written.
		/// </summary>
		public string FilePath
		{
			get { return !string.IsNullOrEmpty(m_filePath)?m_filePath:string.Empty; }
			set { m_filePath = value; }
		}

		/// <summary>
		/// Start timing. This method is called in the constructor.
		/// </summary>
		/// <param name="name">The name of this profiling instance.</param>
		/// <remarks>This method is conditional on it being a debug compile.</remarks>
		[Conditional("DEBUG")]
		private void StartTiming(string name)
		{
			m_startTime = DateTime.Now;
			m_timerName = name;
		}

		/// <summary>
		/// Stop timing and write the results. This is called in the dispose method of the class.
		/// </summary>
		/// <remarks>This method is conditional on it being a debug compile.</remarks>
		[Conditional("DEBUG")]
		private void WriteTiming()
		{
			string fileName = "";
			if ( FilePath.Length > 0 )
				fileName = string.Concat(FilePath, FilePath.EndsWith("\\")?string.Empty:"\\", "Profiler.log");
			else
				fileName = "Profiler.log";
			System.IO.TextWriter tw = System.IO.File.AppendText(fileName);
			tw.WriteLine("{0}: Execution completed in {1} seconds", m_timerName, (DateTime.Now-m_startTime).ToString());
			tw.Flush();
			tw.Close();
		}

		#region IDisposable Members

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose()
		{
			WriteTiming();
		}

		#endregion
	}
}
