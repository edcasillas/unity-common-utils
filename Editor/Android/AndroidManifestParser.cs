using System.Xml;

namespace CommonUtils.Editor.Android {
	public class AndroidManifestParser {
		public static string GetMainActivityName(string manifestPath) {
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