using System;
using System.Collections.Generic;
using System.Text;

namespace Azuro.Logging
{
	/// <summary>
	/// 
	/// </summary>
	public class CategorySink
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>
        /// Is this category disabled
        /// </summary>
        /// <value>The name.</value>
        public bool Disabled { get; set; }

        /// <summary>
		/// Gets or sets the config.
		/// </summary>
		/// <value>The config.</value>
		public CfgLogCategory Config { get; set; }

		/// <summary>
		/// Gets or sets the log destinations.
		/// </summary>
		/// <value>The log destinations.</value>
		public List<DestinationSink> LogDestinations { get; set; }
	}
}
