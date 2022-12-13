using CommonUtils.Editor.Publitch;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.Publitch {
	public class PublitchParseStatusTests
	{
		[Test]
		public void ParseInitialOutput() {
			var butlerOutput = @"+---------+----------+--------------------------+---------+
| CHANNEL |  UPLOAD  |          BUILD           | VERSION |
+---------+----------+--------------------------+---------+
| html    | #6952149 | √ #673902 (from #673895) |      33 |
|         |          | ∙ #673904 (from #673902) |         |
+---------+----------+--------------------------+---------+
";
			var result = ButlerParser.TryParseStatus(butlerOutput, out var channelName, out var upload, out var build, out var version);
			Assert.IsTrue(result, $"Failed to parse : {butlerOutput}");
			Assert.AreEqual("html", channelName);
			Assert.AreEqual("#6952149", upload);
			Assert.AreEqual("√ #673902 (from #673895)", build);
			Assert.AreEqual("33", version);
		}
	}
}
