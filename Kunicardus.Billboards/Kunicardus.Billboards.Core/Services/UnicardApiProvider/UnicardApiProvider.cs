using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using Kunicardus.Billboards.Core.Plugins;
using Kunicardus.Billboards.Core.Services.UnicardApiProvider;
using System.Diagnostics;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.Models;
using Kunicardus.Billboards.Core.Models.DataTransferObjects;
using Kunicardus.Billboards.Core.Helpers;

namespace Kunicardus.Billboards.Core.UnicardApiProvider
{
	public class UnicardApiProvider : IUnicardApiProvider
	{
		private const int MaxRetryTime = 3;

		private static readonly string TAG = "UnicardApiProvider";
		private HttpClient _httpClient;
		IConnectivityPlugin _networkService;
		ICustomSecurityProvider _securityProvider;

		public UnicardApiProvider (IConnectivityPlugin networkService, ICustomSecurityProvider securiryProvider)
		{
			_networkService = networkService;
			_httpClient = new HttpClient ();
			_httpClient.Timeout = TimeSpan.FromMilliseconds (30000);
			_securityProvider = securiryProvider;
		}

		//TODO Add functional test
		int tryCount = 0;

		public TResultObject Post<TResultObject> (string url, Dictionary<string, string> headers, string body) where TResultObject : UnicardApiBaseResponse
		{
			lock (_httpClient) {
				//using (var timing = new TimingUtility (Log, "POST " + url)) {

				if (!CanConnect ()) {

					var i = Activator.CreateInstance<TResultObject> ();
					i.ResultCode = "504";
					i.ResultCode = "Unreachable";
					i.DisplayMessage = "მოხდა შეცდომა, სერვერთან კავშირისას";
					return i;
				}

				var contentType = "application/json";
				var contentObject = Activator.CreateInstance<TResultObject> ();

				try {
					HttpContent content = null;
					content = new StringContent (body, Encoding.UTF8, contentType);
					_httpClient.DefaultRequestHeaders.Clear ();
					var defaultHeaders = new Dictionary<string, string> ();

					var hash = _securityProvider.CalculateHash (body);
					defaultHeaders.Add ("x-cmd5", hash);

					var hmac = _securityProvider.CalculateHMac (defaultHeaders.Values.ToList ());
					defaultHeaders.Add ("x-authorization", string.Format ("UNICARDAPI {0}:{1}", _securityProvider.GetKey (), hmac));

					var userInfo = _securityProvider.GetSecurityParams ();
					if (userInfo != null) {
						defaultHeaders.Add ("x-session", userInfo.SessionId);
						defaultHeaders.Add ("x-userid", userInfo.UserId);
					}

					defaultHeaders.Add ("x-source", "MOBAPP");
					foreach (var header in defaultHeaders) {
						_httpClient.DefaultRequestHeaders.Add (header.Key, header.Value);
					}

					HttpResponseMessage response = null;
					var retryCount = 1;

					while (retryCount <= MaxRetryTime) {
						var responseTask = _httpClient.PostAsync (url, content).Result;

						if (responseTask != null) {
							response = responseTask;
							if (response.StatusCode == HttpStatusCode.Unauthorized && tryCount < 1) {
								tryCount++;
								userInfo = UpdateSession ();

								if (userInfo != null) {
									Post<TResultObject> (url, headers, body);
									tryCount = 0;
								} else {
									var i = Activator.CreateInstance<TResultObject> ();
									i.ResultCode = HttpStatusCode.Unauthorized.ToString ();
									i.ResultCode = "Unauthorized";
									i.DisplayMessage = "არაავტორიზირებული მომხმარებელი";
									return i;
								}
							} else if (response.StatusCode == HttpStatusCode.Unauthorized) {
								var i = Activator.CreateInstance<TResultObject> ();
								i.ResultCode = HttpStatusCode.Unauthorized.ToString ();
								i.ResultCode = "Unauthorized";
								i.DisplayMessage = "არაავტორიზირებული მომხმარებელი";
								return i;
							}
							//Ignore in ncrunch until we mock HttpClient
							//ncrunch: no coverage start
							if (response.StatusCode == HttpStatusCode.GatewayTimeout) {
								var i = Activator.CreateInstance<TResultObject> ();
								i.ResultCode = "504";
								i.ResultCode = "Unreachable";
								i.DisplayMessage = "მოხდა შეცდომა, სერვერთან კავშირისას";
								return i;
							}
							//ncrunch: no coverage end

							break;
						}
						retryCount++;
					}
					//ncrunch: no coverage end

					if (response.IsSuccessStatusCode) {
						var contentString = response.Content.ReadAsStringAsync ().Result;
						contentObject = JsonConvert.DeserializeObject<TResultObject> (contentString);
					} else {
						contentObject.ResultCode = response.StatusCode.ToString ();
					}
				} catch (Exception ex) {
					var i = Activator.CreateInstance<TResultObject> ();
					i.ResultCode = "500";
					i.ResultCode = "Unknown Error";
					i.DisplayMessage = "მოხდა შეცდომა, სერვერთან კავშირისას";
					return i;
				}
				if (contentObject.ResultCode != "200"
				    && string.IsNullOrWhiteSpace (contentObject.DisplayMessage)
				    && !string.IsNullOrWhiteSpace (contentObject.ErrorMessage)) {
					contentObject.DisplayMessage = "მოხდა შეცდომა, სერვერთან კავშირისას";
				}
				var resultObject = (TResultObject)contentObject;
				return resultObject;
			}
		}

		public UserModel UpdateSession ()
		{
			var info = _securityProvider.GetCredentials ();
			var authRequest = new AuthRequest {
				UserName = info.UserName,
				Password = info.Password,
				FacebookId = info.FbToken
			};
			var url = string.Format ("{0}UserAuthorization", Constants.UnicardServiceUrl);
			var json = JsonConvert.SerializeObject (authRequest,
				           Formatting.None,
				           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
			var response = Post<AuthResponse> (url, null, json);

			if (response.Successful && !string.IsNullOrEmpty (response.SessionID)) {
				_securityProvider.SaveCredentials (response.UserId, authRequest.UserName, authRequest.Password, response.SessionID);
				var data = new UserModel ();
				data.UserId = response.UserId;
				data.SessionId = response.SessionID;
				return data;
			}

			return null;
		}

		public BillboardsBaseResponse<TResultObject> GetFromApi<TResultObject> (string url, Dictionary<string, string> headers) where TResultObject : class, new()
		{
			lock (_httpClient) {
				//using (var timing = new TimingUtility(Log, "GET " + url))
				//{

				if (!CanConnect ()) {

					return new BillboardsBaseResponse<TResultObject> () {
						ResultCode = "504",
						ErrorMessage = "Unreachable",
						Result = null
					};
				}

				var contentObject = new BillboardsBaseResponse<TResultObject> ();

				try {
					if (headers != null && headers.Count > 0) {
						foreach (var header in headers) {
							_httpClient.DefaultRequestHeaders.Add (header.Key, header.Value);
						}
					}
					var getResult = _httpClient.GetAsync (url).Result;
					if (getResult.IsSuccessStatusCode) {
						var contentString = getResult.Content.ReadAsStringAsync ().Result;

						contentObject.Result = JsonConvert.DeserializeObject<TResultObject> (contentString);
					} else {
						contentObject.ResultCode = getResult.StatusCode.ToString ();

					}

				}
				//ncrunch: no coverage start
				catch (Exception e) {

					return new BillboardsBaseResponse<TResultObject> () {
						ResultCode = "500",
						ErrorMessage = "Unknown",
						Result = null
					};
				}
				return contentObject;
			}
		}

		public BillboardsBaseResponse<TResultObject> PostToApi<TResultObject> (string url, Dictionary<string, string> headers, string body) where TResultObject : class, new()
		{
			lock (_httpClient) {
				//using (var timing = new TimingUtility (Log, "POST " + url)) {

				if (!CanConnect ()) {
					return new BillboardsBaseResponse<TResultObject> () {
						ResultCode = "504",
						ErrorMessage = "Unreachable",
						Result = null
					};
				}

				var contentObject = new BillboardsBaseResponse<TResultObject> ();

				try {
					HttpContent content = null;

					content = new StringContent (body, Encoding.UTF8, "application/json");

					if (headers == null) {
						headers = new Dictionary<string, string> ();
					}

					HttpResponseMessage response = null;
					var retryCount = 1;

					while (retryCount <= MaxRetryTime) {

						//Android.Util.Log.Debug("Ads Update Message: ", string.Format("Posting...{0}", url));
						var responseTask = _httpClient.PostAsync (url, content).Result;

						if (responseTask != null) {
							response = responseTask;

							//Ignore in ncrunch until we mock HttpClient
							//ncrunch: no coverage start
							if (response.StatusCode == HttpStatusCode.GatewayTimeout) {


								return new BillboardsBaseResponse<TResultObject> () {
									ResultCode = "504",
									ErrorMessage = "Unreachable",
									Result = null
								};
							}
							//ncrunch: no coverage end

							break;
						}

						//Ignore in ncrunch until we mock HttpClient
						//ncrunch: no coverage start

						retryCount++;
					}
					//ncrunch: no coverage end

					if (response.IsSuccessStatusCode) {
						var contentString = response.Content.ReadAsStringAsync ().Result;

						contentObject.Result = JsonConvert.DeserializeObject<TResultObject> (contentString);
					} else {
						contentObject.ResultCode = response.StatusCode.ToString ();

					}
				} catch (Exception ex) {  // TODO: Fix Exception handler


					return new BillboardsBaseResponse<TResultObject> () {
						ResultCode = "504",
						ErrorMessage = "Unreachable",
						Result = null
					};
				}
				if (contentObject.ResultCode != "200"
				    && string.IsNullOrWhiteSpace (contentObject.DisplayMessage)
				    && !string.IsNullOrWhiteSpace (contentObject.ErrorMessage)) {
					contentObject.DisplayMessage = "მოხდა შეცდომა, სერვერთან კავშირისას";
				}
				var resultObject = contentObject;
				return resultObject;
			}
		}

		private bool CanConnect ()
		{
			if (_networkService.IsWifiReachable || _networkService.IsNetworkReachable) {

				return true;
			}


			return false;
		}

		public TResultObject UnsecuredPost<TResultObject> (string url, Dictionary<string, string> headers, string body) where TResultObject : UnicardApiBaseResponse
		{
			lock (_httpClient) {

                var contentObject = Activator.CreateInstance<TResultObject>();

				try {
					HttpContent content = null;

					content = new StringContent (body, Encoding.UTF8, "application/json");

					if (headers == null) {
						headers = new Dictionary<string, string> ();
					}
					HttpResponseMessage response = null;
					var retryCount = 1;

					while (retryCount <= MaxRetryTime) {
						var responseTask = _httpClient.PostAsync (url, content).Result;

						if (responseTask != null) {
							response = responseTask;

							//Ignore in ncrunch until we mock HttpClient
							//ncrunch: no coverage start
							if (response.StatusCode == HttpStatusCode.GatewayTimeout) {
								var i = Activator.CreateInstance<TResultObject> ();
								i.ResultCode = "504";
								i.ResultCode = "Unreachable";
								i.DisplayMessage = "მოხდა შეცდომა, სერვერთან კავშირისას";
								return i;
							}
							//ncrunch: no coverage end

							break;
						}

						retryCount++;
					}

					//ncrunch: no coverage end
					if (response.IsSuccessStatusCode) {
						var contentString = response.Content.ReadAsStringAsync ().Result;
						contentObject = JsonConvert.DeserializeObject<TResultObject> (contentString);
					} else {
						contentObject.ResultCode = response.StatusCode.ToString ();
					}
				} catch (Exception ex) {  // TODO: Fix Exception handler
					var i = Activator.CreateInstance<TResultObject> ();
					i.ResultCode = "500";
					i.ResultCode = "Unknown Error";
					i.DisplayMessage = "მოხდა შეცდომა, სერვერთან კავშირისას";
					return i;
				}
				if (contentObject.ResultCode != "200"
				    && string.IsNullOrWhiteSpace (contentObject.DisplayMessage)
				    && !string.IsNullOrWhiteSpace (contentObject.ErrorMessage)) {
					contentObject.DisplayMessage = "მოხდა შეცდომა, სერვერთან კავშირისას";
				}
				var resultObject = (TResultObject)contentObject;
				return resultObject;
			}
		}

	}
}