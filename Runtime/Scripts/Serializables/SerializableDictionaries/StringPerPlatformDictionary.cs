using System;
using UnityEngine;

namespace CommonUtils.Serializables.SerializableDictionaries
{
	[Serializable]
	public class StringPerPlatformDictionary : AbstractSerializableDictionary<RuntimePlatform, string>{}
}