using System;
using System.Collections.Generic;

namespace CommonUtils.RestSdk {
	/// <summary>
	/// Represents the response of a call to a REST API without any attached data.
	/// </summary>
	public class RestResponse {
		private static readonly List<RestResponseClass> successResponseTypes = new List<RestResponseClass>() {
			RestResponseClass.Informational, RestResponseClass.Success, RestResponseClass.Redirection
		};

		/// <summary>
		/// Gets or sets the response status code. (See: https://httpstatuses.com/)
		/// </summary>
		public long StatusCode{ get; set; }

		/// <summary>
		/// Gets or sets the error message sent by the API.
		/// </summary>
		public string ErrorMessage{ get; set; }

		/// <summary>
		/// Gets or sets additional information about the error, retrieved from the REST API.
		/// </summary>
		public string AdditionalInfo { get; set; }

		/// <summary>
		/// Gets a value indicating whether the response has a success status code.
		/// </summary>
		/// <returns><c>true</c> when the status code is a success status code, otherwise <c>false</c>.</returns>
		public bool IsSuccess => successResponseTypes.Contains(ResponseClass);

		/// <summary>
		/// Gets the class of this response. (See: https://httpstatuses.com/)
		/// </summary>
		public RestResponseClass ResponseClass {
			get {
				var strCode = StatusCode.ToString();
				if (string.IsNullOrEmpty(strCode) || strCode.Length != 3) {
					return RestResponseClass.Unknown;
				}

				if (!int.TryParse(strCode.Substring(0, 1), out var codeClassifier)) {
					// Get the first character of the status, which defines the class of response.
					return RestResponseClass.Unknown;
				}

				var result = (RestResponseClass) codeClassifier;
				return !Enum.IsDefined(typeof(RestResponseClass), result) ? RestResponseClass.Unknown : result;
			}
		}

		public override string ToString() => $"[{StatusCode}]";
	}

	/// <summary>
	/// Represents the response of a call to a REST API with attached data.
	/// </summary>
	public class RestResponse<TDto> : RestResponse {
		/// <summary>
		/// Gets or sets the data received in the response.
		/// </summary>
		public TDto Data{ get; set; }

		public override string ToString() => $"[RestResponse: StatusCode={StatusCode}, Data={Data}]";
	}
}