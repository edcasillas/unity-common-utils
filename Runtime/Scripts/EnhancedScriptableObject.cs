using CommonUtils.Verbosables;
using UnityEngine;

namespace CommonUtils {
	public abstract class EnhancedScriptableObject : ScriptableObject, IVerbosable {
		[SerializeField] private Verbosity verbosity = Verbosity.Error | Verbosity.Warning;

		public Verbosity Verbosity {
			get => verbosity;
			protected set => verbosity = value;
		}
	}
}