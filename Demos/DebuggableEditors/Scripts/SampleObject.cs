using CommonUtils;
using System.Collections.Generic;
using UnityEngine;

namespace Demos.DebuggableEditors {
	public class SampleObject : MonoBehaviour {

		[ShowInInspector(Tooltip = "An integer value getter. This property does not have a setter.")]
		public int IntValue => 5;

		[ShowInInspector]
		public IEnumerable<string> ListOfStrings => new[] { "This", "is", "a", "list", "of", "strings" };

		[ShowInInspector(Tooltip = "Prints the input to the console.")] public void Echo(string input = "Hello") => Debug.Log(input);

		public void MethodWithoutAttribute(string input = "Goodbye") => Debug.Log(input);

		[ShowInInspector] public int AddOne(int input) => input + 1;
	}
}
