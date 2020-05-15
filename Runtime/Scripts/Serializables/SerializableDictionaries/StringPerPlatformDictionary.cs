using System;
using CommonUtils.Serializables;
using UnityEngine;

namespace CommonUtils.UI
{
	[Serializable]
	public class StringPerPlatformDictionary : AbstractSerializableDictionary<RuntimePlatform, string>{}
}