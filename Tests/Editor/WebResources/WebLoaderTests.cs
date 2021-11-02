using CommonUtils.WebResources;
using NUnit.Framework;

namespace CommonUtils.Tests.Editor {
    public class WebLoaderTests {
        // A Test behaves as an ordinary method
        [Test]
        public void GetStatusCodeFromMessage_ParsesCorrectlyWithoutErrorCode() {
            var errorMessage = "Cannot connect to destination host";
            
            var errorCode = WebLoader.GetStatusCodeFromMessage(errorMessage, out var outputMessage);

            Assert.AreEqual(500, errorCode);
            Assert.AreEqual(errorMessage, outputMessage);
        }
    }
}