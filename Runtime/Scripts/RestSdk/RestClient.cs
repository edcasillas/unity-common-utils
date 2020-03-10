using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ExaGames.RestSdk;
using UnityEngine;
using UnityEngine.Networking;

namespace CommonUtils.RestSdk {
	/// <summary>
	/// Clase que permite la interacción con un API mediante métodos GET, POST, PUT y DELETE.
	/// </summary>
	/// <last-change>$LastChangedBy: ed $</last-change>
	public class RestClient : IRestClient {

/*#if DEBUG
		private const string apiConfigPath = "Config/ApiConfigDev"; // Para utilizar el servicio ejecutado desde Visual Studio.
#else
		private const string apiConfigPath = "Config/ApiConfig";	// Para ejecutar el API instalada en IIS.
#endif*/

		/*private static readonly string apiUrl;

		static RestProxy() {
			Debug.LogFormat("Cargando la configuración de la API desde '{0}'", apiConfigPath);
			var cfg = Resources.Load<ApiConfig>(apiConfigPath);
			if(cfg == null || string.IsNullOrEmpty(cfg.APIUrl)) {
				Debug.LogError("No se ha establecido la URL del API a utilizar.");
			} else {
				apiUrl = cfg.APIUrl;
				Debug.LogFormat("Conectando al API en {0}", apiUrl);
			}
		}*/

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
		public void Get<TResult>(string actionRelativePath, Action<RestResponse<DtoCollection<TResult>>> callback)
			=> get(actionRelativePath, callback);

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
			if (string.IsNullOrEmpty(ApiUrl))
				throw new InvalidOperationException("No requests can be performed when ApiUrl is null or empty.");
			if (callback == null) throw new ArgumentNullException(nameof(callback));
			var url =
				$"{ApiUrl}{(!string.IsNullOrWhiteSpace(actionRelativePath) ? $"/{actionRelativePath}" : string.Empty)}";
			ExecuteGet(url, callback);
		}

		protected void ExecuteGet<TResult>(string url, Action<RestResponse<TResult>> callback) {
			var www = UnityWebRequest.Get(url);
			Coroutiner.StartCoroutine(SendRequest(www, callback));
		}

		#endregion

		#region POST

		/// <summary>
		/// Hace una petición POST al servidor (en la ruta especificada por <paramref name="controller"/>) para publicar (crear) los datos especificados en <paramref name="data"/>,
		/// y obtiene los resultados de tipo <typeparamref name="TResult"> en el <paramref name="callback"/>
		/// </summary>
		/// <param name="controller">Ruta del método POST.</param>
		/// <param name="data">Datos a enviar.</param>
		/// <param name="callback">Callback para recibir la respuesta.</param>
		/// <typeparam name="TResult">Tipo de resultado esperado.</typeparam>
		public void Post<TResult>(string controller, object data, Action<RestResponse<TResult>> callback) {
			#region Input validation
			if (string.IsNullOrEmpty(controller)) throw new ArgumentNullException("route");
			if (callback == null) throw new ArgumentNullException("callback");
			#endregion

			string url      = string.Format("{0}/{1}", ApiUrl, controller);
			string postData = JsonUtility.ToJson(data);
			Coroutiner.StartCoroutine(ExecutePost(url, postData, callback));
		}

		/// <summary>
		/// Hace una petición POST al servidor (en la ruta especificada por <paramref name="controller"/>) para publicar (crear) los datos especificados en <paramref name="data"/>,
		/// y obtiene una respuesta simple (sin datos) en el <paramref name="callback"/>.
		/// </summary>
		/// <param name="controller">Ruta del método POST.</param>
		/// <param name="data">Datos a enviar.</param>
		/// <param name="callback">Callback para recibir la respuesta.</param>
		public void Post(string controller, object data, Action<RestResponse> callback = null) {
			if (string.IsNullOrEmpty(controller)) throw new ArgumentNullException("route");

			string url      = string.Format("{0}/{1}", ApiUrl, controller);
			string postData = JsonUtility.ToJson(data);
			Coroutiner.StartCoroutine(ExecutePost(url, postData, callback));
		}

		protected IEnumerator
			ExecutePost<TResult>(string url, string postData, Action<RestResponse<TResult>> callback) {
			if (callback == null) throw new ArgumentNullException("callback");

			var requestId = Guid.NewGuid().ToString(); // An id to recognize this request in the log.
			Debug.LogFormat("{0} Executing REST request {1}: [POST {2}; data: {3}]",
							DateTime.Now,
							requestId,
							url,
							postData);

			var response = new RestResponse<TResult>();

			// HACK For some reason, UnityWebRequest arrives with null body to the server, so we handle this different than the other requests.
			if (string.IsNullOrEmpty(MacAddress.Current)) {
				response.ErrorMessage = "No se ha podido verificar la identidad de esta máquina.";
			} else {
				Dictionary<string, string> headers = new Dictionary<string, string> {
					{"MacAddress", MacAddress.Current},
					{"Content-type", "application/json"}
				};
				byte[] body = Encoding.UTF8.GetBytes(postData);
				WWW    www  = new WWW(url, body, headers);

				yield return www;

				response.ErrorMessage = www.error;

				if (!string.IsNullOrEmpty(www.error)) {
					Debug.LogErrorFormat("{0} REST POST ERROR {1}: [{2}{3}]",
										 DateTime.Now,
										 requestId,
										 www.error,
										 !string.IsNullOrEmpty(www.text) ?
											 string.Format("; {0}", www.text) :
											 string.Empty);
					var err     = www.error.Substring(0, 3);
					var errCode = 0;
					if (int.TryParse(err, out errCode)) {
						response.StatusCode = errCode;
					} else {
						response.StatusCode = 500;
					}

					response.AdditionalInfo = www.text;
					if (!string.IsNullOrEmpty(response.AdditionalInfo) && response.AdditionalInfo.StartsWith("\"") &&
						response.AdditionalInfo.EndsWith("\"")) {
						response.AdditionalInfo =
							response.AdditionalInfo.Substring(1, response.AdditionalInfo.Length - 2);
					}
				} else {
					response.StatusCode = 200;
					var data = www.text;
					if (!string.IsNullOrEmpty(data)) {
						Debug.LogFormat("{0} REST POST SUCCESS {1}: {2}", DateTime.Now, requestId, data);
						response.Data = JsonUtility.FromJson<TResult>(data);
					}
				}
			}

			callback(response);
		}

		protected IEnumerator ExecutePost(string url, string postData, Action<RestResponse> callback) {
			var response = new RestResponse();
			if (string.IsNullOrEmpty(MacAddress.Current)) {
				if (callback != null) {
					response.ErrorMessage = "No se ha podido verificar la identidad de esta máquina.";
					callback(response);
				} else {
					Debug.LogError("No se ha podido obtener la Mac Address de esta máquina.");
				}
			} else {
				// HACK Por alguna razón, UnityWebRequest llega con valores nulos en el body al servidor, por lo que aquí se maneja diferente a todos los demás.
				var headers = new Dictionary<string, string> {
					{"MacAddress", MacAddress.Current},
					{"Content-type", "application/json"}
				};
				byte[] body = Encoding.UTF8.GetBytes(postData);
				var    www  = new WWW(url, body, headers);

				yield return www;

				if (callback != null) {
					response.ErrorMessage = www.error;

					if (!string.IsNullOrEmpty(www.error)) {
						Debug.LogErrorFormat("El servidor contestó con el error: {0}{1}",
											 www.error,
											 !string.IsNullOrEmpty(www.text) ?
												 string.Format("; {0}", www.text) :
												 string.Empty);
						var err     = www.error.Substring(0, 3);
						var errCode = 0;
						if (int.TryParse(err, out errCode)) {
							response.StatusCode = errCode;
						} else {
							response.StatusCode = 500;
						}
					} else {
						response.StatusCode = 200;
					}

					callback(response);
				} else if (!string.IsNullOrEmpty(www.error)) {
					// Si no se especificó un callback, se está solicitando ejecutar un fire-and-forget, por lo que solo registramos si hay algún error en el log.
					Debug.LogErrorFormat("Error en POST {0}: {1}", www.url, www.error);
				}
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
		protected IEnumerator SendRequest<TResult>(UnityWebRequest request, Action<RestResponse<TResult>> callback) {
			#region Input validation
			if (request  == null) throw new ArgumentNullException(nameof(request));
			if (callback == null) throw new ArgumentNullException(nameof(callback));
			if (request.isDone) throw new InvalidOperationException("Can't execute a request once is done.");
			#endregion

			RestResponse<TResult> response = null;

			using (request) {
				try {
					SetAuthRequestHeaders(request);
				} catch (InvalidOperationException ex) {
					response = new RestResponse<TResult>() {StatusCode = -1, ErrorMessage = ex.Message};
				}

				if (response == null) {
					yield return request.SendWebRequest();
					response = GetResponseFrom<TResult>(request);
				}
			}

			callback(response);
		}

		/// <summary>
		/// Ejecuta un <paramref name="request"/> al API y obtiene la respuesta en el <paramref name="callback"/> especificado.
		/// </summary>
		/// <param name="request">Request a ejecutar.</param>
		/// <param name="callback">Callback donde se recibirá la respuesta del servidor.</param>
		protected IEnumerator SendRequest(UnityWebRequest request, Action<RestResponse> callback) {
			#region Input validation
			if (request == null)
				throw new ArgumentNullException("request");
			if (request.isDone)
				throw new InvalidOperationException("No se puede ejecutar un request una vez que ha terminado.");
			#endregion

			RestResponse response = null;

			using (request) {
				try {
					SetAuthRequestHeaders(request);
				} catch (InvalidOperationException ex) {
					response = new RestResponse() {StatusCode = -1, ErrorMessage = ex.Message};
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

		protected virtual void SetAuthRequestHeaders(UnityWebRequest www) => www.SetRequestHeader("Content-type", "application/json");

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