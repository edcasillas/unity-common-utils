using CommonUtils.Extensions;
using System.Reflection;

namespace CommonUtils.Editor.DebuggableEditors {
	public abstract class AbstractReflectedMember<TMember> where TMember : MemberInfo {
		public string RealName { get; }
		public string DisplayName { get; }
		public TMember MemberInfo { get; }
		public bool Fold { get; set; }
		public abstract System.Type Type { get; }

		protected AbstractReflectedMember(TMember memberInfo, string displayName) {
			MemberInfo = memberInfo;
			RealName = memberInfo.Name;
			DisplayName = !string.IsNullOrWhiteSpace(displayName) ? displayName : RealName.PascalToTitleCase();
		}

		public void RenderDebugInfoIfAny(){}
	}
}