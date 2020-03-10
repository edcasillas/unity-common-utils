using System;
using ExaGames.RestSdk;

namespace CommonUtils.RestSdk {
	/// <summary>
	/// Contract with methods that allow interaction with an API through GET, POST, PUT and DELETE methods.
	/// </summary>
	public interface IRestClient {
		/// <summary>
		/// Gets or sets the URL of the API this client will connect to.
		/// </summary>
		string ApiUrl { get; set; }
		
		/// <summary>
		/// Creates a GET request to the specified <paramref name="actionRelativePath"/> and receives a response of type <typeparamref name="TResult"/> in <paramref name="callback"/>.
		/// </summary>
		/// <typeparam name="TResult">Type of expected result.</typeparam>
		/// <param name="actionRelativePath">Action path to be called in the API.</param>
		/// <param name="callback">Method to receive the response.</param>
		void Get<TResult>(string actionRelativePath, Action<RestResponse<TResult>> callback);

		/// <summary>
		/// Creates a GET request to the specified <paramref name="actionRelativePath"/> and receive a response with a collection of <typeparamref name="TResult"/> objects in <paramref name="callback"/>.
		/// </summary>
		/// <typeparam name="TResult">Type of expected result.</typeparam>
		/// <param name="actionRelativePath">Action path to be called in the API.</param>
		/// <param name="callback">Method to receive the response.</param>
		void GetCollection<TResult>(string actionRelativePath, Action<RestResponse<DtoCollection<TResult>>> callback);

		/// <summary>
		/// Creates a GET request to the specified <paramref name="actionRelativePath"/> to retreive an object of type <typeparamref name="TResult"/> identified by <paramref name="id"/>, and receives the response in <paramref name="callback"/>.
		/// </summary>
		/// <typeparam name="TResult">Type of expected result.</typeparam>
		/// <param name="actionRelativePath">Action path to be called in the API.</param>
		/// <param name="id">Id of the requested resource.</param>
		/// <param name="callback">Method to receive the response.</param>
		void Get<TResult>(string actionRelativePath, object id, Action<RestResponse<TResult>> callback);

		/// <summary>
		/// Creates a POST request to the specified <paramref name="actionRelativePath"/> to send the <paramref name="data"/> and receive a response of type <typeparamref name="TResult"/> in <paramref name="callback"/>.
		/// </summary>
		/// <typeparam name="TResult">Type of expected result.</typeparam>
		/// <param name="actionRelativePath">Action path to be called in the API.</param>
		/// <param name="data">Data to be sent as POST body.</param>
		/// <param name="callback">Method to receive the response.</param>
		void Post<TResult>(string actionRelativePath, object data, Action<RestResponse<TResult>> callback);

		/// <summary>
		/// Creates a POST request to the specified <paramref name="actionRelativePath"/> to send the <paramref name="data"/> and receive a response in <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to be called in the API.</param>
		/// <param name="data">Data to be sent as POST body.</param>
		/// <param name="callback">Method to receive the response. If none specified, executes a fire-and-forget POST request.</param>
		void Post(string actionRelativePath, object data, Action<RestResponse> callback = null);

		void Put<TResult>(string controller, object id, object data, Action<RestResponse<TResult>> callback);
		void Put(string          controller, object id, object data, Action<RestResponse>          callback);

		void Delete<TResult>(string controller, object id, Action<RestResponse<TResult>> callback);
		void Delete(string          controller, object id, Action<RestResponse>          callback);
	}
}