using CommonUtils.UnityComponents;
using CommonUtils.Verbosables;
using System;
using UnityEngine;

namespace CommonUtils {
	/// <summary>
	/// Derive from this class instead of <see cref="MonoBehaviour"/> to enable the following perks:
	/// - Expose properties and methods at runtime to the Inspector using the <see cref="ShowInInspectorAttribute"/>.
	/// - Implement <see cref="IUnityComponent"/> and <see cref="IVerbosable"/>.
	/// </summary>
	public abstract class EnhancedMonoBehaviour : MonoBehaviour, IUnityComponent, IVerbosable {
		[SerializeField] private Verbosity verbosity = Verbosity.Error | Verbosity.Warning;

		public Verbosity Verbosity {
			get => verbosity;
			protected set => verbosity = value;
		}
	}
}