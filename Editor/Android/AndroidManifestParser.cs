using System.Xml;
using UnityEngine;

namespace CommonUtils.Editor.Android {
	public class AndroidManifestParser {
		public static string GetMainActivityNameFromApk(string apkPath) {
			string mainActivityName = string.Empty;

			using (var packageManager = new AndroidJavaClass("android.content.pm.PackageManager")) {
				using (var packageInfo =
					   packageManager.CallStatic<AndroidJavaObject>("getPackageArchiveInfo", apkPath, 0)) {
					var applicationInfo = packageInfo.Get<AndroidJavaObject>("applicationInfo");
					var activityInfoArray = packageInfo.Get<AndroidJavaObject[]>("activities");

					if (activityInfoArray.Length > 0) {
						var mainActivityInfo = activityInfoArray[0];
						mainActivityName = mainActivityInfo.Get<string>("name");
					}
				}
			}

			if (string.IsNullOrEmpty(mainActivityName)) {
				mainActivityName = "Main activity not found";
			}

			return mainActivityName;
		}

		public static string GetMainActivityNameFromManifest(string manifestPath) {
			var manifestXml = new XmlDocument();
			manifestXml.Load(manifestPath);

			var namespaceManager = new XmlNamespaceManager(manifestXml.NameTable);
			namespaceManager.AddNamespace("x", "urn:AndroidManifest-schema");
			namespaceManager.AddNamespace("android", "http://schemas.android.com/apk/res/android");

			var activityNode = manifestXml.SelectSingleNode("//activity[intent-filter/action[@android:name='android.intent.action.MAIN']]", namespaceManager);
			var activityNameAttribute = activityNode?.Attributes?["android:name"];
			var activityName = activityNameAttribute?.Value;
			if (!string.IsNullOrEmpty(activityName)) {
				return activityName;
			}

			// Fallback to the default main activity name defined by Unity
			return "com.unity3d.player.UnityPlayerActivity";
		}
	}
}