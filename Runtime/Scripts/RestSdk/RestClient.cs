using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ExaGames.RestSdk;
using UnityEngine;
using UnityEngine.Networking;

namespace CommonUtils.RestSdk {
	/// <summary>
	/// Class with methods that allow interaction with an API through GET, POST, PUT and DELETE methods.
	/// </summary>
	public class RestClient : IRestClient {
		/// <summary>
		/// Gets or sets the URL of the API this client will connect to, without trailing slash. 
		/// </summary>
		public string ApiUrl { get; set; }

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
		public void Get<TResult>(string actionRelativePath, Action<RestResponse<TResult>> callback)
			=> get(actionRelativePath, callback);

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
		/// <param name="data">Data to be sent.</param>
		/// <param name="callback">Callback method to receive the response.</param>
		/// <typeparam name="TResult">Type of DTO expected as result.</typeparam>
		public void Post<TResult>(string actionRelativePath, object data, Action<RestResponse<TResult>> callback) {
			#region Input validation
			if (string.IsNullOrEmpty(actionRelativePath)) throw new ArgumentNullException(nameof(actionRelativePath));
			if (callback == null) throw new ArgumentNullException(nameof(callback));
			#endregion

			var url      = $"{ApiUrl}/{actionRelativePath}";
			var postData = JsonUtility.ToJson(data);
			ExecutePost(url, postData, callback);
		}
		
		public void Post<TResult>(string actionRelativePath, Dictionary<string, object> data, Action<RestResponse<TResult>> callback) {
			#region Input validation
			if (string.IsNullOrEmpty(actionRelativePath)) throw new ArgumentNullException(nameof(actionRelativePath));
			if (callback == null) throw new ArgumentNullException(nameof(callback));
			#endregion

			var url      = $"{ApiUrl}/{actionRelativePath}";
			
			var form = new WWWForm();
			foreach (var kvp in data) {
				form.AddField(kvp.Key, kvp.Value.ToString());	
			}
			var www = UnityWebRequest.Post(url, form);
			Coroutiner.StartCoroutine(SendRequest(www, callback, true), $"REST POST {url}");
			//ExecutePost(url, postData, callback);
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
			Coroutiner.StartCoroutine(ExecutePost(url, postData, callback), $"REST POST {url}");
		}

		protected void ExecutePost<TResult>(string url, string postData, Action<RestResponse<TResult>> callback) {
			if (callback == null) throw new ArgumentNullException(nameof(callback));
			
			var  www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
			var bytes = Encoding.UTF8.GetBytes(postData);
			var uH = new UploadHandlerRaw(bytes) {contentType = "application/json"};
			www.uploadHandler = uH;
			www.downloadHandler = new DownloadHandlerBuffer();
			Coroutiner.StartCoroutine(SendRequest(www, callback), $"REST POST {url}");
		}

		protected IEnumerator ExecutePost(string url, string postData, Action<RestResponse> callback) {
			var response = new RestResponse();

			// HACK For some reason, UnityWebRequest arrives with null body to the server, so we handle this different than the other requests.
			var headers = new Dictionary<string, string> {
				{"Content-type", "application/json"}
			};
			var body = Encoding.UTF8.GetBytes(postData);
			var    www  = new WWW(url, body, headers);

			yield return www;

			if (callback != null) {
				response.ErrorMessage = www.error;

				if (!string.IsNullOrEmpty(www.error)) {
					Debug.LogError($"REST POST ERROR: [{www.error}{(!string.IsNullOrEmpty(www.text) ? $"; {www.text}" : string.Empty)}]");
					var err     = www.error.Substring(0, 3);
					response.StatusCode = int.TryParse(err, out var errCode) ? errCode : 500;
				} else {
					response.StatusCode = 200;
				}

				callback(response);
			} else if (!string.IsNullOrEmpty(www.error)) {
				// If a callback was not specified, then we should just fire-and-forget, so we only log the error, if any.
				Debug.LogError($"REST POST ERROR: [{www.url}: {www.error}]");
			}
		}

		#endregion

		#region PUT

		/// <summary>
		/// Hace una petición PUT al servidor en el endpoint especificado por <paramref name="controller"/> para actualizar los datos de una entidad con el identificador
		/// especificado por <paramref name="id"/> con los datos especificados en <paramref name="data"/>, y obtiene los resultados de tipo <typeparamref name="TResult">
		/// en el <paramref name="callback"/>.
		/// </summary>
		/// <param name="controller">Controlador (endpoint) al que se enviará la petición.</param>
		/// <param name="id">Identificador de la entidad a modificar.</param>
		/// <param name="data">Datos de la entidad a modificar.</param>
		/// <param name="callback">Callback para recibir la respuesta.</param>
		/// <typeparam name="TResult">Tipo de resultado esperado.</typeparam>
		public void Put<TResult>(string controller, object id, object data, Action<RestResponse<TResult>> callback) {
			if (string.IsNullOrEmpty(controller))
				throw new ArgumentNullException("controller");
			if (id == null)
				throw new ArgumentNullException("id");
			if (data == null)
				throw new ArgumentNullException("data");
			if (callback == null)
				throw new ArgumentNullException("callback");
			string url     = string.Format("{0}/{1}/{2}", ApiUrl, controller, id);
			string putData = JsonUtility.ToJson(data);
			ExecutePut(url, putData, callback);
		}

		/// <summary>
		/// Hace una petición PUT al servidor en el endpoint especificado por <paramref name="controller"/> para actualizar los datos de una entidad con el identificador
		/// especificado por <paramref name="id"/> con los datos especificados en <paramref name="data"/>, y obtiene una respuesta simple (sin datos)
		/// en el <paramref name="callback"/>.
		/// </summary>
		/// <param name="controller">Controlador (endpoint) al que se enviará la petición.</param>
		/// <param name="id">Identificador de la entidad a modificar.</param>
		/// <param name="data">Datos de la entidad a modificar.</param>
		/// <param name="callback">Callback para recibir la respuesta.</param>
		public void Put(string controller, object id, object data, Action<RestResponse> callback) {
			if (string.IsNullOrEmpty(controller))
				throw new ArgumentNullException("controller");
			if (id == null)
				throw new ArgumentNullException("id");
			if (data == null)
				throw new ArgumentNullException("data");
			if (callback == null)
				throw new ArgumentNullException("callback");
			string url     = string.Format("{0}/{1}/{2}", ApiUrl, controller, id);
			string putData = JsonUtility.ToJson(data);
			ExecutePut(url, putData, callback);
		}

		protected void ExecutePut<TResult>(string url, string putData, Action<RestResponse<TResult>> callback) {
			byte[] putBody = Encoding.UTF8.GetBytes(putData);
			var    www     = UnityWebRequest.Put(url, putBody);
			Coroutiner.StartCoroutine(SendRequest<TResult>(www, callback));
		}

		protected void ExecutePut(string url, string putData, Action<RestResponse> callback) {
			byte[] putBody = Encoding.UTF8.GetBytes(putData);
			var    www     = UnityWebRequest.Put(url, putBody);
			Coroutiner.StartCoroutine(SendRequest(www, callback));
		}

		#endregion

		#region DELETE

		/// <summary>
		/// Hace una petición DELETE al servidor en el endpoint especificado por <paramref name="controller"/> para eliminar la entidad con el identificador especificado por
		/// <paramref name="id"/> y obtiene los resultados de tipo <typeparamref name="TResult"> en el <paramref name="callback"/>.
		/// </summary>
		/// <param name="controller">Controlador (endpoint) al que se enviará la petición.</param>
		/// <param name="id">Identificador de la entidad a eliminar.</param>
		/// <param name="callback">Callback para recibir la respuesta.</param>
		/// <typeparam name="TResult">Tipo de resultado esperado.</typeparam>
		public void Delete<TResult>(string controller, object id, Action<RestResponse<TResult>> callback) {
			if (string.IsNullOrEmpty(controller))
				throw new ArgumentNullException("controller");
			if (id == null)
				throw new ArgumentNullException("id");
			if (callback == null)
				throw new ArgumentNullException("callback");
			string url = string.Format("{0}/{1}/{2}", ApiUrl, controller, id);
			ExecuteDelete<TResult>(url, callback);
		}

		/// <summary>
		/// Hace una petición DELETE al servidor en el endpoint especificado por <paramref name="controller"/> para eliminar la entidad con el identificador especificado por
		/// <paramref name="id"/> y obtiene una respuesta simple (sin datos) en el <paramref name="callback"/>.
		/// </summary>
		/// <param name="controller">Controlador (endpoint) al que se enviará la petición.</param>
		/// <param name="id">Identificador de la entidad a eliminar.</param>
		/// <param name="callback">Callback para recibir la respuesta.</param>
		public void Delete(string controller, object id, Action<RestResponse> callback) {
			#region Input validation
			if (string.IsNullOrEmpty(controller)) throw new ArgumentNullException("controller");
			if (id == null) throw new ArgumentNullException("id");
			if (callback == null) {
				Debug.LogWarningFormat("No se estableció un callback para la petición DELETE a {0}; se ejecutará la llamada sin respuesta.",
									   controller);
			}
			#endregion

			string url = string.Format("{0}/{1}/{2}", ApiUrl, controller, id);
			ExecuteDelete(url, callback);
		}

		protected void ExecuteDelete<TResult>(string url, Action<RestResponse<TResult>> callback) {
			var www = UnityWebRequest.Delete(url);
			Coroutiner.StartCoroutine(SendRequest<TResult>(www, callback));
		}

		protected void ExecuteDelete(string url, Action<RestResponse> callback) {
			var www = UnityWebRequest.Delete(url);
			Coroutiner.StartCoroutine(SendRequest(www, callback));
		}

		#endregion

		#region Private methods
		/// <summary>
		/// Ejecuta un <paramref name="request"/> al API y obtiene la respuesta en el <paramref name="callback"/> especificado.
		/// Sends a <paramref name="request"/> to the API and obtains the response in the specified <paramref name="callback"/>.
		/// </summary>
		/// <param name="request">Request to send..</param>
		/// <param name="callback">Callback to receive the response..</param>
		/// <typeparam name="TResult">Type of expected result.</typeparam>
		protected IEnumerator SendRequest<TResult>(UnityWebRequest request, Action<RestResponse<TResult>> callback, bool omitHeaders = false) {
			#region Input validation
			if (request  == null) throw new ArgumentNullException(nameof(request));
			if (callback == null) throw new ArgumentNullException(nameof(callback));
			if (request.isDone) throw new InvalidOperationException("Can't execute a request once is done.");
			#endregion

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

		/// <summary>
		/// Ejecuta un <paramref name="request"/> al API y obtiene la respuesta en el <paramref name="callback"/> especificado.
		/// </summary>
		/// <param name="request">Request a ejecutar.</param>
		/// <param name="callback">Callback donde se recibirá la respuesta del servidor.</param>
		protected IEnumerator SendRequest(UnityWebRequest request, Action<RestResponse> callback, bool omitHeaders = false) {
			#region Input validation
			if (request == null)
				throw new ArgumentNullException("request");
			if (request.isDone)
				throw new InvalidOperationException("No se puede ejecutar un request una vez que ha terminado.");
			#endregion

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
					yield return request.Send();
					if (request.isNetworkError)
						Debug.LogErrorFormat("Error de REST ({0}): {1}", request.method, request.error);
					response = GetResponseFrom(request);
				}
			}

			if (callback != null) callback(response);
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
		/// Deserializa una <see cref="UnityWebRequest"/> terminada y la convierte en un <see cref="RestResponse"/>
		/// </summary>
		/// <returns>Respuesta deserializada.</returns>
		/// <param name="www">Respuesta serializada.</param>
		protected RestResponse GetResponseFrom(UnityWebRequest www) {
			if (!www.isDone) {
				Debug.LogError("Se está intentando obtener el valor de una respuesta de un servicio que no ha terminado de contestar. ABORTANDO.");
				return null;
			}

			var response = new RestResponse {
				StatusCode = www.responseCode
			};
			return response;
		}
		#endregion
	}
}