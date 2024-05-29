using CommonUtils;
using CommonUtils.Verbosables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Demos.DebuggableEditors {
	public class SampleObject : EnhancedMonoBehaviour {
		[ShowInInspector(Tooltip = "An integer value getter. This property does not have a setter.")]
		public int IntValue => 5;

		[ShowInInspector]
		public IEnumerable<string> ListOfStrings => new[] { "This", "is", "a", "list", "of", "strings" };

		[ShowInInspector(Tooltip = "Prints the input to the console.")] public void Echo(string input = "Hello") => this.Log(input);

		public void MethodWithoutAttribute(string input = "Goodbye") => this.Log(input);

		[ShowInInspector] public int AddOne(int input) => input + 1;

		[ShowInInspector] public void MethodWithException(string text = "echo") => throw new Exception(text);

		[ShowInInspector]
		public async Task AsyncTask(int millisecondsDelay = 2000) {
			this.Log("Starting async task");
			await Task.Delay(millisecondsDelay);
			this.Log("Finished async task");
		}

		[ShowInInspector]
		public async Task<string> AsyncTaskWithResult(string text = "echo", int millisecondsDelay = 2000) {
			await Task.Delay(millisecondsDelay);
			return text;
		}

		[ShowInInspector]
		public async Task AsyncTaskWithException(string text = "echo", int millisecondsDelay = 2000) {
			await Task.Delay(millisecondsDelay);
			throw new Exception(text);
		}
	}
}
