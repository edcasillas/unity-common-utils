namespace CommonUtils.Editor.Publitch {
	public struct ButlerStatus {
		public string ChannelName;
		public string Upload;
		public string Build;
		public string Version;

		public bool HasData { get; private set; }

		internal static bool TryParse(string butlerOutput, ref ButlerStatus status) {
			if (string.IsNullOrEmpty(butlerOutput)) return false;

			var lines = butlerOutput.Split('\n');
			if (lines.Length < 4) return false;

			var columns = lines[3].Split('|');
			if (columns.Length < 5) return false;

			status.ChannelName = columns[1].Trim();
			status.Upload = columns[2].Trim();
			status.Build = columns[3].Trim();
			status.Version = columns[4].Trim();
			status.HasData = true;

			return true;
		}

		public void Clear() => HasData = false;
	}
}