using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

namespace CommonUtils {
	/// <summary>
	/// Utility to retrieve the MAC address of the machine executing the game.
	/// </summary>
	public static class MacAddress {
		/// <summary>
		/// MAC Address of the machine executing the game.
		/// </summary>
		public static readonly string Current = getMacAddress();
		
		private static string getMacAddress() {
			var activeNic = NetworkInterface.GetAllNetworkInterfaces()
											.FirstOrDefault(nic => !string.IsNullOrEmpty(nic.GetPhysicalAddress().ToString()) &&
																   nic.OperationalStatus == OperationalStatus.Up &&
																   nic.NetworkInterfaceType !=
																   NetworkInterfaceType.Loopback);
			if (activeNic == null) {
				Debug.LogError($"Could not get the MAC Address of this machine.");
				return null;
			}

			var result = activeNic.GetPhysicalAddress().ToString().Replace("-", "").Replace(":", "");
			
			Debug.Log($"This machine's MAC Address is: {result}");
			return result;
		}
	}
}