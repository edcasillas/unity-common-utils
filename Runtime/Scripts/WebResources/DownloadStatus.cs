namespace CommonUtils.WebResources {
	/// <summary>
	/// Download status for web resources.
	/// </summary>
	public enum DownloadStatus {
		/// <summary>
		/// Downloading has not yet started.
		/// </summary>
		NotInited = 0,

		/// <summary>
		/// The resource is being downloaded.
		/// </summary>
		Loading,

		/// <summary>
		/// The resource has been successfully downloaded.
		/// </summary>
		Loaded,

		/// <summary>
		/// The resource couldn't be downloaded.
		/// </summary>
		Error
	}
}