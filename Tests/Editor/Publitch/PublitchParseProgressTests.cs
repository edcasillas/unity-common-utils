using CommonUtils.Editor.Publitch;
using NUnit.Framework;

namespace CommonUtils.Tests.Editor.Publitch {
	public class PublitchParseProgressTests {
		[Test]
		public void ParseInitialOutput() {
			var butlerOutput = "∙ For channel `html`: last build is 673111, downloading its signature";
			var result = ButlerParser.TryParseProgress(butlerOutput, out _);
			Assert.AreEqual(ButlerParser.ParseResult.ExpectedNonProgress, result, $"Failed to parse : {butlerOutput}");
		}

		[Test]
		public void ParseMeasuresToUpload() {
			var butlerOutput = "∙ Pushing 123.30 MiB (27 files, 3 dirs, 0 symlinks)";
			var result = ButlerParser.TryParseProgress(butlerOutput, out _);
			Assert.AreEqual(ButlerParser.ParseResult.ExpectedNonProgress, result, $"Failed to parse : {butlerOutput}");
		}

		[Test]
		public void ParseWithNoExtraData_Passes() {
			var butlerOutput = "▐                   ▌   0.00%                                                   ";
			var result = ButlerParser.TryParseProgress(butlerOutput, out var parsed);
			Assert.AreEqual(ButlerParser.ParseResult.Ok, result, $"Failed to parse : {butlerOutput}");
			Assert.AreEqual(0f, parsed);
		}

		[Test]
		public void ParseWithExtraData_NetworkIdle_Passes() {
			var butlerOutput = "▐█░░                ▌   3.09%  - network idle, 117.66 MiB left                  ";
			var result = ButlerParser.TryParseProgress(butlerOutput, out var parsed);
			Assert.AreEqual(ButlerParser.ParseResult.Ok, result, $"Failed to parse : {butlerOutput}");
			Assert.AreEqual(3.09f, parsed);
		}

		[Test]
		public void ParseWithExtraData_ShowingNetworkSpeed_Passes() {
			var butlerOutput = "▐███░░░░░           ▌  25.97%  @ 11.60 MiB/s, 91.22 MiB left                    ";
			var result = ButlerParser.TryParseProgress(butlerOutput, out var parsed);
			Assert.AreEqual(ButlerParser.ParseResult.Ok, result, $"Failed to parse : {butlerOutput}");
			Assert.AreEqual(25.97f, parsed);
		}

		[Test]
		public void ParsePatchSize() {
			var butlerOutput = "√ 91.47 MiB patch (25.82% savings)";
			var result = ButlerParser.TryParseProgress(butlerOutput, out _);
			Assert.AreEqual(ButlerParser.ParseResult.ExpectedNonProgress, result, $"Failed to parse : {butlerOutput}");
		}

		[Test]
		public void ParseFirstEndingLine() {
			var butlerOutput = "∙ Build is now processing, should be up in a bit.";
			var result = ButlerParser.TryParseProgress(butlerOutput, out _);
			Assert.AreEqual(ButlerParser.ParseResult.ExpectedNonProgress, result, $"Failed to parse : {butlerOutput}");
		}
	}
}