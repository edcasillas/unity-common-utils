namespace CommonUtils.RestSdk {
	/// <summary>
	/// Types of response from a REST API according to their classification.
	/// </summary>
	public enum RestResponseClass {
		/// <summary>
		/// Response type is unknown, it's likely that the request has not yet been processed.
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// The request was received, continuing process.
		/// </summary>
		Informational = 1,
		/// <summary>
		/// The request was successfully received, understood, and accepted.
		/// </summary>
		Success = 2,
		/// <summary>
		/// Further action needs to be taken in order to complete the request.
		/// </summary>
		Redirection = 3,
		/// <summary>
		/// The request contains bad syntax or cannot be fulfilled.
		/// </summary>
		ClientError = 4,
		/// <summary>
		/// The server failed to fulfil an apparently valid request.
		/// </summary>
		ServerError = 5
	}
}