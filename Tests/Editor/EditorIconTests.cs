using CommonUtils.Editor.BuiltInIcons;
using NUnit.Framework;
using System;

namespace CommonUtils.Tests.Editor
{
	public class EditorIconTests
	{
		[Test]
		public void TestAllEditorIcons() {
			foreach (var val in Enum.GetValues(typeof(EditorIcon))) {
				var icon = (EditorIcon)val;
				icon.ToGUIContent();
				// Any icons not loading will be displayed in the console when this test is executed, and should be removed from the enum.
			}
		}
	}
}
