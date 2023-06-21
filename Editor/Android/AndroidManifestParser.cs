using System.Xml;

namespace CommonUtils.Editor.Android {
	public class AndroidManifestParser {
		public static string GetMainActivityName(string manifestPath) {
			XmlDocument manifestXml = new XmlDocument();
			manifestXml.Load(manifestPath);

			XmlNamespaceManager namespaceManager = new XmlNamespaceManager(manifestXml.NameTable);
			namespaceManager.AddNamespace("android", "http://schemas.android.com/apk/res/android");

			XmlNode activityNode = manifestXml.SelectSingleNode(
				"//activity[intent-filter/action[@android:name='android.intent.action.MAIN']]",
				namespaceManager);
			if (activityNode != null) {
				XmlAttribute activityNameAttribute = activityNode.Attributes["android:name"];
				if (activityNameAttribute != null) {
					string activityName = activityNameAttribute.Value;
					if (!string.IsNullOrEmpty(activityName)) {
						return activityName;
					}
				}
			}

			// Fallback to the default main activity name defined by Unity
			return "com.unity3d.player.UnityPlayerActivity";
		}

	}
}