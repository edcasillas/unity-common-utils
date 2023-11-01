using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

namespace CommonUtils {
	/// <summary>
	/// Utility to retrieve the Mac and IP addresses of the device executing the game.
	/// </summary>
    public static class NetworkAddress {
		private static string currentIpV4;
		private static string currentMacAddress;

		static NetworkAddress() {
			currentIpV4 = getCurrentIpV4();
			currentMacAddress = getMacAddress();
		}

		/// <summary>
		/// Gets the current Mac address of the device executing the game.
		/// </summary>
		public static string CurrentMac {
			get {
				currentMacAddress ??= getMacAddress();
				return currentMacAddress;
			}
		}

		/// <summary>
		/// Gets the current IPV4 address of the device executing the game.
		/// </summary>
		public static string CurrentIpV4 {
			get {
				currentIpV4 = getCurrentIpV4();
				return currentIpV4;
			}
		}

		/// <summary>
		/// Clears the stored values of <see cref="CurrentMac"/> and <see cref="CurrentIpV4"/>
		/// so they will be calculated again on next usage.
		/// </summary>
		public static void ClearCache() => currentIpV4 = currentMacAddress = null;

		private static string getMacAddress() {
			var activeNic = NetworkInterface.GetAllNetworkInterfaces()
				.FirstOrDefault(nic => !string.IsNullOrEmpty(nic.GetPhysicalAddress().ToString()) &&
									   nic.OperationalStatus == OperationalStatus.Up &&
									   nic.NetworkInterfaceType !=
									   NetworkInterfaceType.Loopback);
			if (activeNic == null) {
				Debug.LogError("Could not get the MAC Address of this machine.");
				return null;
			}

			var result = activeNic.GetPhysicalAddress().ToString().Replace("-", "").Replace(":", "");

			Debug.Log($"This machine's MAC Address is: {result}");
			return result;
		}

		private static string getCurrentIpV4() {
			var ipAddress = "";

			try {
				var hostName = Dns.GetHostName();

				// Get the IP addresses associated with the host
				var addresses = Dns.GetHostAddresses(hostName);

				// Find a suitable IPv4 address
				foreach (var addr in addresses) {
					if (addr.AddressFamily != AddressFamily.InterNetwork) continue;
					ipAddress = addr.ToString();
					break;
				}
			} catch (Exception e) {
				Debug.LogError("Error getting IP address: " + e.Message);
			}

			return ipAddress;
		}
	}
}
