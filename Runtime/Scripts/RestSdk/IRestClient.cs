using CommonUtils.Verbosables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonUtils.RestSdk {
	/// <summary>
	/// Contract with methods that allow interaction with an API through GET, POST, PUT and DELETE methods.
	/// </summary>
	public interface IRestClient : IVerbosable {
		/// <summary>
		/// Gets the URL of the API this client will connect to.
		/// </summary>
		string ApiUrl { get; }

		/// <summary>
		/// Creates a GET request to the specified <paramref name="actionRelativePath"/> and receives a response without data in <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to be called in the API.</param>
		/// <param name="callback">Method to receive the response.</param>
		void Ping(string actionRelativePath, Action<RestResponse> callback);

		Task<RestResponse> PingAsync(string actionRelativePath);

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
		/// Sends a POST request to the specified <paramref name="actionRelativePath"/> of the API to publish the specified <paramref name="data"/> and retrieves results
		/// of type <typeparamref name="TResult"/> in the specified <paramref name="callback"/>.
		/// Data will be sent as a www-form.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="data">Data to be sent.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		/// <typeparam name="TResult">Type of DTO expected as result.</typeparam>
		void Post<TResult>(string actionRelativePath, Dictionary<string, object> data, Action<RestResponse<TResult>> callback);

		/// <summary>
		/// Creates a POST request to the specified <paramref name="actionRelativePath"/> to send the <paramref name="data"/> and receive a response in <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to be called in the API.</param>
		/// <param name="data">Data to be sent as POST body.</param>
		/// <param name="callback">Method to receive the response. If none specified, executes a fire-and-forget POST request.</param>
		void Post(string actionRelativePath, object data, Action<RestResponse> callback = null);

		/// <summary>
		/// Sends a PUT request to the specified <paramref name="actionRelativePath"/> to update an entity with the specified <paramref name="id"/> and <paramref name="data"/>, and
		/// retrieves a result of type <typeparamref name="TResult"/> in the <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="id">Identifier of the entity to modify.</param>
		/// <param name="data">Data to be set to the entity.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		/// <typeparam name="TResult">Type of expected result.</typeparam>
		void Put<TResult>(string actionRelativePath, object id, object data, Action<RestResponse<TResult>> callback);

		/// <summary>
		/// Sends a PUT request to the specified <paramref name="actionRelativePath"/> to update an entity with the specified <paramref name="id"/> and <paramref name="data"/>, and
		/// retrieves a simple response with no attached data in the <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="id">Identifier of the entity to modify.</param>
		/// <param name="data">Data to be set to the entity.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		void Put(string          actionRelativePath, object id, object data, Action<RestResponse>          callback);

		/// <summary>
		/// Sends a DELETE request to the specified <paramref name="actionRelativePath"/> to remove an entity with the specified <paramref name="id"/> and retrieves
		/// a result of type <typeparamref name="TResult"/> in the <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="id">Identifier of the entity to delete.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		/// <typeparam name="TResult">Type of expected result.</typeparam>
		void Delete<TResult>(string actionRelativePath, object id, Action<RestResponse<TResult>> callback);

		/// <summary>
		/// Sends a DELETE request to the specified <paramref name="actionRelativePath"/> to remove an entity with the specified <paramref name="id"/> and retrieves
		/// a simple response with no attached data in the <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Controlador (endpoint) al que se enviará la petición.</param>
		/// <param name="id">Identificador de la entidad a eliminar.</param>
		/// <param name="callback">Callback para recibir la respuesta.</param>
		void Delete(string actionRelativePath, object id, Action<RestResponse> callback = null);
	}
}