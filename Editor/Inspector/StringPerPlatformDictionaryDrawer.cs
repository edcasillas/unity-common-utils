using CommonUtils.UI;
using ExaGames.Common;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Inspector {
	[CustomPropertyDrawer(typeof(StringPerPlatformDictionary))]
	public class StringPerPlatformDictionaryDrawer : AbstractSerializableDictionaryDrawer<StringPerPlatformDictionary, RuntimePlatform, string> { }
}