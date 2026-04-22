using UnityEngine;

namespace CommonUtils
{
    internal static class MigrationUtils
    {
		public static TObject FindObjectOfType<TObject>() where TObject : Object
		{
			#if UNITY_6000
			return Object.FindAnyObjectByType<TObject>();
			#else
			return Object.FindObjectOfType<TObject>();
			#endif
		}
    }
}
