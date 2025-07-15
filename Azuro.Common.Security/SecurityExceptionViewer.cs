using System;
using System.Security.Permissions;
using System.Security;
using System.Collections.Generic;

namespace Azuro.Security
{
	/// <summary>
	/// A helper class to view security exceptions. The class must have low 
	/// privilige requirements in order for it not to also cause a security
	/// exception.
	/// </summary>
	[SecurityPermission(SecurityAction.Assert,
		ControlEvidence = true, ControlPolicy = true)]
	public class SecurityExceptionViewer
	{
		/// <summary>
		/// This method unpacks the security exception into a string array
		/// for viewing.
		/// </summary>
		/// <param name="ex">The System.SecurityException to unpack.</param>
		/// <returns>The exception details.</returns>
		public static Dictionary<string, string> ViewException(SecurityException ex)
		{
			Dictionary<string, string> exDetails = new Dictionary<string, string>();

			exDetails.Add("Message", ex.Message);
			exDetails.Add("Source", ex.Source);
			exDetails.Add("Action", ex.Action.ToString());
			exDetails.Add("Zone", ex.Zone.ToString());
#if __TRY_ANY_SECURITY //	This code needs to check security
			exDetails.Add("Demand", ex.Demanded.ToString());
			exDetails.Add("GrantedSet", ex.GrantedSet);
			exDetails.Add("RefusedSet", ex.RefusedSet);
			exDetails.Add("StackTrace", ex.StackTrace);
			exDetails.Add("FirstPermissionThatFailed", ex.FirstPermissionThatFailed.ToString());
			exDetails.Add("PermissionState", ex.PermissionState);
#endif	//	__TRY_ANY_SECURITY
			return exDetails;
		}
	}
}

