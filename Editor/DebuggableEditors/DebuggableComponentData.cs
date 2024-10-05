using CommonUtils.DebuggableEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils.Editor.DebuggableEditors {
	internal class DebuggableComponentData : IComparable<DebuggableComponentData> {
		public MonoBehaviour Subject;

		public bool ShowConfig;
		public bool ShowDebug = true;

		public ICollection<ReflectedProperty> DebuggableProperties;
		public ICollection<ReflectedMethod> DebuggableMethods;

		public float Timestamp;

		public bool HasDebuggableMembers() => DebuggableMethods.Any() || DebuggableProperties.Any();

		public int CompareTo(DebuggableComponentData other) {
			if (ReferenceEquals(this, other))
				return 0;
			if (ReferenceEquals(null, other))
				return 1;
			return Timestamp.CompareTo(other.Timestamp);
		}
	}
}