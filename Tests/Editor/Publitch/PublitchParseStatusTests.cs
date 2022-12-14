using CommonUtils.Editor.Publitch;
using NUnit.Framework;

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
			var status = new ButlerStatus();
			var result = ButlerStatus.TryParse(butlerOutput, ref status);
			Assert.IsTrue(result, $"Failed to parse : {butlerOutput}");
			Assert.AreEqual("html", status.ChannelName);
			Assert.AreEqual("#6952149", status.Upload);
			Assert.AreEqual("√ #673902 (from #673895)", status.Build);
			Assert.AreEqual("33", status.Version);
		}
	}
}
