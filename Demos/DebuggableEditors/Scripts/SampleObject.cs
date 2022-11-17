using CommonUtils;
using System.Collections.Generic;
using UnityEngine;

namespace Demos.DebuggableEditors {
	public class SampleObject : MonoBehaviour {
		[ShowInInspector] public int IntValue => 5;

		[ShowInInspector]
		public IEnumerable<string> ListOfStrings => new[] { "This", "is", "a", "list", "of", "strings" };

		[ShowInInspector] public void Echo(string input = "Hello") => Debug.Log(input);

		[ShowInInspector] public int AddOne(int input) => input + 1;
	}
}
