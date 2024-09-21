using System;

namespace CommonUtils {
	/// <summary>
	/// Utility to retrieve the MAC address of the machine executing the game.
	/// </summary>
	[Obsolete("Use NetworkAddress.")]
	public static class MacAddress {
		/// <summary>
		/// MAC Address of the machine executing the game.
		/// </summary>
		[Obsolete("Use NetworkAddress.CurrentMac")]
		public static string Current => NetworkAddress.CurrentMac;
	}
}