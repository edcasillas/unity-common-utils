using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace CommonUtils.Editor {
	public static class IOSPostBuildUtils {
		public static void SetAutoSigning(PBXProject proj, string targetGuid, string teamId) {
			// Fallback to Player Settings if no teamId was provided (e.g., not found in external config)
			if (string.IsNullOrWhiteSpace(teamId)) teamId = UnityEditor.PlayerSettings.iOS.appleDeveloperTeamID;

			// Automatic signing
			proj.SetBuildProperty(targetGuid, "CODE_SIGN_STYLE", "Automatic");
			proj.SetTeamId(targetGuid, teamId);

			// Clear any stale manual settings that can fight with automatic
			proj.SetBuildProperty(targetGuid, "PROVISIONING_PROFILE_SPECIFIER", "");
			proj.SetBuildProperty(targetGuid, "PROVISIONING_PROFILE", "");
			proj.SetBuildProperty(targetGuid, "DEVELOPMENT_TEAM", teamId);
		}
	}
}
