using CommonUtils.Coroutines;
using CommonUtils.Extensions;
using CommonUtils.Verbosables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace CommonUtils.RestSdk {
	/// <summary>
	/// Class with methods that allow interaction with an API through GET, POST, PUT and DELETE methods.
	/// </summary>
	public class RestClient : IRestClient {
		public Verbosity Verbosity { get; set; }

		/// <summary>
		/// Gets the URL of the API this client will connect to.
		/// </summary>
		public string ApiUrl { get; }

		/// <summary>
		/// Creates a new instance of <see cref="RestClient"/>.
		/// </summary>
		/// <param name="apiUrl">URL of the API this client will connect to, without trailing slash.</param>
		public RestClient(string apiUrl, Verbosity verbosity = Verbosity.Warning | Verbosity.Error) {
			ApiUrl = apiUrl ?? throw new ArgumentNullException(nameof(apiUrl));
			Verbosity = verbosity;
		}

		#region PING
		public void Ping(string actionRelativePath, Action<RestResponse> callback) {
			var url = $"{ApiUrl}{(!string.IsNullOrWhiteSpace(actionRelativePath) ? $"/{actionRelativePath}" : string.Empty)}";
			var www = UnityWebRequest.Get(url);
			Coroutiner.StartCoroutine(SendRequest(www, callback));
		}

		public async Task<RestResponse> PingAsync(string actionRelativePath) {
			var url = $"{ApiUrl}{(!string.IsNullOrWhiteSpace(actionRelativePath) ? $"/{actionRelativePath}" : string.Empty)}";
			var www = UnityWebRequest.Get(url);
			return await SendRequestAsync(www);
		}
		#endregion

		#region GET
		/// <summary>
		/// Executes a GET request to obtain a collection of results.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		/// <typeparam name="TResult">Type of DTO requested.</typeparam>
		public void GetCollection<TResult>(string actionRelativePath, Action<RestResponse<DtoCollection<TResult>>> callback) => get(actionRelativePath, callback);

		/// <summary>
		/// Executes a GET request to obtain a single result.
		/// </summary>
		/// <typeparam name="TResult">Type of DTO requested.</typeparam>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		public void Get<TResult>(string actionRelativePath, Action<RestResponse<TResult>> callback) => get(actionRelativePath, callback);

		/// <summary>
		/// Executes a GET request to obtain result identified by <paramref name="id"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="id">Id of the requested item.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		/// <typeparam name="TResult">Type of DTO requested.</typeparam>
		public void Get<TResult>(string actionRelativePath, object id, Action<RestResponse<TResult>> callback) {
			if (id == null) throw new ArgumentNullException(nameof(id));

			var url = $"{actionRelativePath}/{id}";
			get(url, callback);
		}

		private void get<TResult>(string actionRelativePath, Action<RestResponse<TResult>> callback) {
			if (string.IsNullOrEmpty(ApiUrl)) throw new InvalidOperationException("No requests can be performed when ApiUrl is null or empty.");
			if (callback == null) throw new ArgumentNullException(nameof(callback));
			var url = $"{ApiUrl}{(!string.IsNullOrWhiteSpace(actionRelativePath) ? $"/{actionRelativePath}" : string.Empty)}";
			ExecuteGet(url, callback);
		}

		protected void ExecuteGet<TResult>(string url, Action<RestResponse<TResult>> callback) {
			var www = UnityWebRequest.Get(url);
			Coroutiner.StartCoroutine(SendRequest(www, callback), $"REST GET {url}");
		}

		#endregion

		#region POST
		/// <summary>
		/// Sends a POST request to the specified <paramref name="actionRelativePath"/> of the API to publish the specified <paramref name="data"/> and retrieves results
		/// of type <typeparamref name="TResult"/> in the specified <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="data">Data to be sent. This object will be serialized to JSON.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		/// <typeparam name="TResult">Type of DTO expected as result.</typeparam>
		public void Post<TResult>(string actionRelativePath, object data, Action<RestResponse<TResult>> callback) {
			var postData = JsonUtility.ToJson(data);
			Post(actionRelativePath, postData, callback);
		}

		/// <summary>
		/// Sends a POST request to the specified <paramref name="actionRelativePath"/> of the API to publish the specified <paramref name="data"/> and retrieves results
		/// of type <typeparamref name="TResult"/> in the specified <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="data">Raw data to be sent.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		/// <typeparam name="TResult">Type of DTO expected as result.</typeparam>
		/// <exception cref="ArgumentNullException">When the data is incomplete.</exception>
		public void Post<TResult>(string actionRelativePath, string data, Action<RestResponse<TResult>> callback) {
			#region Input validation
			if (string.IsNullOrEmpty(actionRelativePath)) throw new ArgumentNullException(nameof(actionRelativePath));
			if (callback == null) throw new ArgumentNullException(nameof(callback));
			#endregion

			var url = $"{ApiUrl}/{actionRelativePath}";
			ExecutePost(url, data, callback);
		}

		/// <summary>
		/// Sends a POST request to the specified <paramref name="actionRelativePath"/> of the API to publish the specified <paramref name="data"/> and retrieves results
		/// of type <typeparamref name="TResult"/> in the specified <paramref name="callback"/>.
		/// Data will be sent as a www-form.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="data">Data to be sent.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		/// <typeparam name="TResult">Type of DTO expected as result.</typeparam>
		public void Post<TResult>(string actionRelativePath, Dictionary<string, object> data, Action<RestResponse<TResult>> callback) {
			if (callback == null) throw new ArgumentNullException(nameof(callback));

			var url = string.IsNullOrEmpty(actionRelativePath) ? ApiUrl : $"{ApiUrl}/{actionRelativePath}";

			var form = new WWWForm();
			foreach (var kvp in data) {
				form.AddField(kvp.Key, kvp.Value.ToString());
			}
			var www = UnityWebRequest.Post(url, form);
			Coroutiner.StartCoroutine(SendRequest(www, callback, true), $"REST POST {url}");
		}

		/// <summary>
		/// Sends a POST request to the specified <paramref name="actionRelativePath"/> of the API to publish the specified <paramref name="data"/> and retrieves a simple response
		/// (without attached data) in the specified <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="data">Data to be sent.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		public void Post(string actionRelativePath, object data, Action<RestResponse> callback = null) {
			if (string.IsNullOrEmpty(actionRelativePath)) throw new ArgumentNullException(nameof(actionRelativePath));

			var url      = $"{ApiUrl}/{actionRelativePath}";
			var postData = JsonUtility.ToJson(data);
			ExecutePost(url, postData, callback);
		}

		private UnityWebRequest preparePostRequest(string url, string postData) {
			var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
			var bytes = Encoding.UTF8.GetBytes(postData);
			var uH    = new UploadHandlerRaw(bytes) {contentType = "application/json"};
			request.uploadHandler = uH;
			request.downloadHandler = new DownloadHandlerBuffer();
			return request;
		}

		protected void ExecutePost<TResult>(string url, string postData, Action<RestResponse<TResult>> callback) {
			if (callback == null) throw new ArgumentNullException(nameof(callback));
			var www = preparePostRequest(url, postData);
			Coroutiner.StartCoroutine(SendRequest(www, callback), $"REST POST {url}");
		}

		protected void ExecutePost(string url, string postData, Action<RestResponse> callback) {
			var www = preparePostRequest(url, postData);
			Coroutiner.StartCoroutine(SendRequest(www, callback), $"REST POST {url}");
		}

		#endregion

		#region PUT
		/// <summary>
		/// Sends a PUT request to the specified <paramref name="actionRelativePath"/> to update an entity with the specified <paramref name="id"/> and <paramref name="data"/>, and
		/// retrieves a result of type <typeparamref name="TResult"/> in the <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="id">Identifier of the entity to modify.</param>
		/// <param name="data">Data to be set to the entity. This object will be serialized to JSON.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		/// <typeparam name="TResult">Type of expected result.</typeparam>
		public void Put<TResult>(string actionRelativePath, object id, object data, Action<RestResponse<TResult>> callback) {
			var putData = JsonUtility.ToJson(data);
			Put(actionRelativePath, id, putData, callback);
		}

		/// <summary>
		/// Sends a PUT request to the specified <paramref name="actionRelativePath"/> to update an entity with the specified <paramref name="id"/> and <paramref name="data"/>, and
		/// retrieves a result of type <typeparamref name="TResult"/> in the <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="id">Identifier of the entity to modify.</param>
		/// <param name="data">The raw data to be sent.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		/// <typeparam name="TResult">Type of expected result.</typeparam>
		public void Put<TResult>(string actionRelativePath, object id, string data, Action<RestResponse<TResult>> callback) {
			if (string.IsNullOrEmpty(actionRelativePath)) throw new ArgumentNullException(nameof(actionRelativePath));
			if (id == null) throw new ArgumentNullException(nameof(id));
			if (data == null) throw new ArgumentNullException(nameof(data));
			if (callback == null) throw new ArgumentNullException(nameof(callback));
			var url = $"{ApiUrl}/{actionRelativePath}/{id}";
			ExecutePut(url, data, callback);
		}

		/// <summary>
		/// Sends a PUT request to the specified <paramref name="actionRelativePath"/> to update an entity with the specified <paramref name="id"/> and <paramref name="data"/>, and
		/// retrieves a simple response with no attached data in the <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="id">Identifier of the entity to modify.</param>
		/// <param name="data">Data to be set to the entity.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		public void Put(string actionRelativePath, object id, object data, Action<RestResponse> callback) {
			if (string.IsNullOrEmpty(actionRelativePath)) throw new ArgumentNullException(nameof(actionRelativePath));
			if (id == null) throw new ArgumentNullException(nameof(id));
			if (data == null) throw new ArgumentNullException(nameof(data));
			if (callback == null) throw new ArgumentNullException(nameof(callback));
			var url     = $"{ApiUrl}/{actionRelativePath}/{id}";
			var putData = JsonUtility.ToJson(data);
			ExecutePut(url, putData, callback);
		}

		protected void ExecutePut<TResult>(string url, string putData, Action<RestResponse<TResult>> callback) {
			byte[] putBody = Encoding.UTF8.GetBytes(putData);
			var    www     = UnityWebRequest.Put(url, putBody);
			Coroutiner.StartCoroutine(SendRequest(www, callback), $"REST PUT {url}");
		}

		protected void ExecutePut(string url, string putData, Action<RestResponse> callback) {
			byte[] putBody = Encoding.UTF8.GetBytes(putData);
			var    www     = UnityWebRequest.Put(url, putBody);
			Coroutiner.StartCoroutine(SendRequest(www, callback), $"REST PUT {url}");
		}

		#endregion

		#region DELETE

		/// <summary>
		/// Sends a DELETE request to the specified <paramref name="actionRelativePath"/> to remove an entity with the specified <paramref name="id"/> and retrieves
		/// a result of type <typeparamref name="TResult"/> in the <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Action path to call in the API.</param>
		/// <param name="id">Identifier of the entity to delete.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		/// <typeparam name="TResult">Type of expected result.</typeparam>
		public void Delete<TResult>(string actionRelativePath, object id, Action<RestResponse<TResult>> callback) {
			if (string.IsNullOrEmpty(actionRelativePath)) throw new ArgumentNullException(nameof(actionRelativePath));
			if (id == null) throw new ArgumentNullException(nameof(id));
			if (callback == null) throw new ArgumentNullException(nameof(callback));
			var url = $"{ApiUrl}/{actionRelativePath}/{id}";
			ExecuteDelete(url, callback);
		}

		/// <summary>
		/// Sends a DELETE request to the specified <paramref name="actionRelativePath"/> to remove an entity with the specified <paramref name="id"/> and retrieves
		/// a simple response with no attached data in the <paramref name="callback"/>.
		/// </summary>
		/// <param name="actionRelativePath">Controlador (endpoint) al que se enviará la petición.</param>
		/// <param name="id">Identificador de la entidad a eliminar.</param>
		/// <param name="callback">Callback para recibir la respuesta.</param>
		public void Delete(string actionRelativePath, object id, Action<RestResponse> callback = null) {
			#region Input validation
			if (string.IsNullOrEmpty(actionRelativePath)) throw new ArgumentNullException(nameof(actionRelativePath));
			if (id == null) throw new ArgumentNullException(nameof(id));
			if (callback == null) {
				this.LogNoContext($"A callback was not specified for the DELETE request to {actionRelativePath}; will fire and forget.");
			}
			#endregion

			var url = $"{ApiUrl}/{actionRelativePath}/{id}";
			ExecuteDelete(url, callback);
		}

		protected void ExecuteDelete<TResult>(string url, Action<RestResponse<TResult>> callback) {
			var www = UnityWebRequest.Delete(url);
			Coroutiner.StartCoroutine(SendRequest(www, callback));
		}

		protected void ExecuteDelete(string url, Action<RestResponse> callback) {
			var www = UnityWebRequest.Delete(url);
			Coroutiner.StartCoroutine(SendRequest(www, callback));
		}

		#endregion

		#region Private methods
		/// <summary>
		/// Sends a <paramref name="request"/> to the API and obtains the response in the specified <paramref name="callback"/>.
		/// </summary>
		/// <param name="request">Request to send.</param>
		/// <param name="callback">Callback to receive the response.</param>
		/// <typeparam name="TResult">Type of expected result.</typeparam>
		protected IEnumerator SendRequest<TResult>(UnityWebRequest request, Action<RestResponse<TResult>> callback, bool omitHeaders = false) {
			#region Input validation
			if (request  == null) throw new ArgumentNullException(nameof(request));
			if (callback == null) throw new ArgumentNullException(nameof(callback));
			if (request.isDone) throw new InvalidOperationException("Can't execute a request once is done.");
			#endregion

			this.LogNoContext($"Sending {request.method} {request.url}");

			RestResponse<TResult> response = null;

			using (request) {
				if (!omitHeaders) {
					try {
						SetRequestHeaders(request);
					} catch (InvalidOperationException ex) {
						callback(new RestResponse<TResult>() {StatusCode = -1, ErrorMessage = ex.Message});
						yield break;
					}
				}

				yield return request.SendWebRequest();
				response = GetResponseFrom<TResult>(request);
			}

			callback(response);
		}

		protected async Task<RestResponse<TResult>> SendRequestAsync<TResult>(UnityWebRequest request, bool omitHeaders = false) {
			#region Input validation
			if (request  == null) throw new ArgumentNullException(nameof(request));
			if (request.isDone) throw new InvalidOperationException("Can't execute a request once is done.");
			#endregion

			this.LogNoContext($"Sending {request.method} {request.url}");

			RestResponse<TResult> response = null;

			using (request) {
				if (!omitHeaders) {
					try {
						SetRequestHeaders(request);
					} catch (InvalidOperationException ex) {
						return new RestResponse<TResult>() {StatusCode = -1, ErrorMessage = ex.Message};
					}
				}

				request.SendWebRequest();
				await AsyncUtils.WaitUntilAsync(() => request.isDone);

				response = GetResponseFrom<TResult>(request);
			}

			return response;
		}

		/// <summary>
		/// Sends a <paramref name="request"/> to the API and obtains the response in the specified <paramref name="callback"/>.
		/// </summary>
		/// <param name="request">Request to send.</param>
		/// <param name="callback">Callback to receive the response</param>
		protected IEnumerator SendRequest(UnityWebRequest request, Action<RestResponse> callback, bool omitHeaders = false) {
			#region Input validation
			if (request == null) throw new ArgumentNullException(nameof(request));
			if (request.isDone) throw new InvalidOperationException("Can't execute a request once is done.");
			#endregion

			this.LogNoContext($"Sending {request.method} {request.url}");

			RestResponse response = null;

			using (request) {
				if (!omitHeaders) {
					try {
						SetRequestHeaders(request);
					} catch (InvalidOperationException ex) {
						response = new RestResponse() {StatusCode = -1, ErrorMessage = ex.Message};
					}
				}

				if (response == null) {
					yield return request.SendWebRequest();

					#if UNITY_2020_2_OR_NEWER
					if(request.result != UnityWebRequest.Result.Success)
					#else
					if (request.isNetworkError || request.isHttpError)
					#endif
						Debug.LogError($"REST ERROR: ({request.method}): {request.error}");
					response = GetResponseFrom(request);
				}
			}

			callback?.Invoke(response);
		}

		protected async Task<RestResponse> SendRequestAsync(UnityWebRequest request, bool omitHeaders = false) {
			#region Input validation
			if (request == null) throw new ArgumentNullException(nameof(request));
			if (request.isDone) throw new InvalidOperationException("Can't execute a request once is done.");
			#endregion

			this.LogNoContext($"Sending {request.method} {request.url}");

			RestResponse response = null;

			using (request) {
				if (!omitHeaders) {
					try {
						SetRequestHeaders(request);
					} catch (InvalidOperationException ex) {
						return new RestResponse() {StatusCode = -1, ErrorMessage = ex.Message};
					}
				}

				//yield return request.SendWebRequest();
				request.SendWebRequest();
				await AsyncUtils.WaitUntilAsync(() => request.isDone);

				if(request.FinishedWithError()) this.LogNoContext($"REST ERROR: ({request.method}): {request.error}", LogLevel.Error);
				response = GetResponseFrom(request);
			}

			return response;
		}

		protected virtual void SetRequestHeaders(UnityWebRequest www) => www.SetRequestHeader("Content-type", "application/json");

		protected virtual RestResponse<string> GetRawResponseFrom(UnityWebRequest www) {
			if (!www.isDone) {
				Debug.LogError("Failed trying to get the value from a service response which is not yet done. ABORTING");
				return null;
			}

			var result = new RestResponse<string> {
				StatusCode = www.responseCode
			};

			if (!result.IsSuccess) {
				Debug.LogError($"REST ERROR: [{www.method} {www.url}: {www.responseCode}{(!string.IsNullOrEmpty(www.error) ? $": {www.error}" : string.Empty)}]");
				result.ErrorMessage = www.error;
				if (www.downloadHandler != null) {
					result.AdditionalInfo = www.downloadHandler.text;
				}
			} else if (www.downloadHandler != null) {
				result.Data = www.downloadHandler.text;
			}

			return result;
		}

		/// <summary>
		/// Extracts and deserializes a response from a finished <see cref="UnityWebRequest"/> and converts it to <see cref="RestResponse"/>.
		/// </summary>
		/// <param name="www">Finished request.</param>
		/// <typeparam name="TDto">Expected data type included in the response.</typeparam>
		/// <returns>Deserialized response.</returns>
		protected virtual RestResponse<TDto> GetResponseFrom<TDto>(UnityWebRequest www) {
			var rawResponse = GetRawResponseFrom(www);
			var result = new RestResponse<TDto> {
				AdditionalInfo = rawResponse.AdditionalInfo,
				ErrorMessage   = rawResponse.ErrorMessage,
				StatusCode     = rawResponse.StatusCode
			};
			if (rawResponse.IsSuccess && !string.IsNullOrEmpty(rawResponse.Data)) {
				try {
					result.Data = JsonUtility.FromJson<TDto>(rawResponse.Data);
				} catch (Exception ex) {
					Debug.LogError($"Could not deserialize response to type {typeof(TDto).Name}. Raw response was: {rawResponse.Data}. Error: {ex.Message}");
					Debug.LogException(ex);
					result.StatusCode   = 0;
					result.ErrorMessage = ex.Message;
				}
			}

			return result;
		}

		/// <summary>
		/// Deserializes a response contained in a finished <see cref="UnityWebRequest"/> and packs it into a <see cref="RestResponse"/> instance.
		/// </summary>
		/// <param name="www">Finished request.</param>
		/// <returns>Deserialized response..</returns>
		protected RestResponse GetResponseFrom(UnityWebRequest www) {
			if (!www.isDone) {
				Debug.LogError("Trying to get the value of a response from an unfinished request is not allowed. ABORTING.");
				return null;
			}

			var response = new RestResponse {
				StatusCode = www.responseCode,
				ErrorMessage = www.error
			};
			return response;
		}
		#endregion
	}
}