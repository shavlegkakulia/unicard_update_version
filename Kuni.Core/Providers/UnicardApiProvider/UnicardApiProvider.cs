using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Kuni.Core.Utilities.Logger;
using Kuni.Core.Utilities.Timing;
using System.Net.Http;
using Kuni.Core.Plugins.Connectivity;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using MvvmCross;
using Kuni.Core.Services.Abstract;
using MvvmCross.Plugin.File;
//using MvvmCross;

namespace Kuni.Core.UnicardApiProvider
{
	public class UnicardApiProvider : IUnicardApiProvider
	{
		private const int MaxRetryTime = 3;
		private readonly ILoggerService _loggerService;
		private readonly IConnectivityPlugin _networkService;
		private readonly IMvxFileStore _fileStore;
		ICustomSecurityProvider _securityProvider;
	
		private static readonly string TAG = "UnicardApiProvider";
		private HttpClient _httpClient;

		public UnicardApiProvider (
			ILoggerService logger,
			IConnectivityPlugin network,
			IMvxFileStore fileStore,
			ICustomSecurityProvider securityProvider
		)
		{
			_loggerService = logger;
			_networkService = network;
			_fileStore = fileStore;
			_httpClient = new HttpClient ();
			_httpClient.Timeout = TimeSpan.FromMilliseconds (30000);
			_securityProvider = securityProvider;
		}

		int tryCount = 0;
		//TODO Add functional test
		public async Task<TResultObject> Post<TResultObject> (string url, Dictionary<string, string> headers, string body) where TResultObject : UnicardApiBaseResponse
		{
			lock (_httpClient) {
				using (var timing = new TimingUtility (_loggerService, "POST " + url)) {
					_loggerService.Debug (TAG, "Calling URL (POST): " + url);
					if (!CanConnect ()) {
						_loggerService.Error (TAG, "Returning unreachable");
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

						_loggerService.Debug ("Headers", _httpClient.DefaultRequestHeaders.ToString ());
						HttpResponseMessage response = null;
						var retryCount = 1;
	
						while (retryCount <= MaxRetryTime) {
							var responseTask = _httpClient.PostAsync (url, content).Result;
	
							if (responseTask != null) {
								response = responseTask;
								if (response.StatusCode == HttpStatusCode.Unauthorized && tryCount < 1) {
									tryCount++;
									userInfo = Mvx.IoCProvider.Resolve<IAuthService> ().UpdateSession ();

									if (userInfo != null) {
										var r = Post<TResultObject> (url, headers, body).Result;
										tryCount = 0;
										return r;
									} else {
										_loggerService.Error (TAG, "Gateway Timeout from server");
										var i = Activator.CreateInstance<TResultObject> ();
										i.ResultCode = HttpStatusCode.Unauthorized.ToString ();
										i.ResultCode = "Unauthorized";
										i.DisplayMessage = "არაავტორიზირებული მომხმარებელი";
										tryCount = 0;
										return i;
									}
								} else if (response.StatusCode == HttpStatusCode.Unauthorized) {
									_loggerService.Error (TAG, "Gateway Timeout from server");
									var i = Activator.CreateInstance<TResultObject> ();
									i.ResultCode = HttpStatusCode.Unauthorized.ToString ();
									i.ResultCode = "Unauthorized";
									i.DisplayMessage = "არაავტორიზირებული მომხმარებელი";
									tryCount = 0;
									return i;
								}
								//Ignore in ncrunch until we mock HttpClient
								//ncrunch: no coverage start
								if (response.StatusCode == HttpStatusCode.GatewayTimeout) {
									_loggerService.Error (TAG, "Gateway Timeout from server");
									var i = Activator.CreateInstance<TResultObject> ();
									i.ResultCode = "504";
									i.ResultCode = "Unreachable";
									i.DisplayMessage = "მოხდა შეცდომა, სერვერთან კავშირისას";
									return i;
								}
								//ncrunch: no coverage end
	
								break;
							}
	
							//Ignore in ncrunch until we mock HttpClient
							//ncrunch: no coverage start
							_loggerService.Error (TAG, "POST request timeout {0}", url);
							retryCount++;
						}
						//ncrunch: no coverage end
	
						if (response.IsSuccessStatusCode) {
							var contentString = response.Content.ReadAsStringAsync ().Result;
							_loggerService.Debug (TAG, $"Response: {contentString}");
							//double kb= contentString.Length * sizeof(Char) * 0.001;
							contentObject = JsonConvert.DeserializeObject<TResultObject> (contentString);
						} else {
							contentObject.ResultCode = response.StatusCode.ToString ();
							_loggerService.Debug (TAG, "Server returned error code " + response.StatusCode);
						}
					} catch (Exception ex) {  // TODO: Fix Exception handler
						_loggerService.Error (TAG, "Error connecting to unicard Api, error : {0}", ex.Message);
						_loggerService.Debug (TAG, ex.StackTrace);
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

		public async Task<TResultObject> Post<TResultObject> (string url, string body) where TResultObject : UnicardApiBaseResponse
		{
			lock (_httpClient) {
				using (var timing = new TimingUtility (_loggerService, "POST " + url)) {
					_loggerService.Debug (TAG, "Calling URL (POST): " + url);
					if (!CanConnect ()) {
						_loggerService.Error (TAG, "Returning unreachable");
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

					
						defaultHeaders.Add ("x-source", "MOBAPP");
						foreach (var header in defaultHeaders) {
							_httpClient.DefaultRequestHeaders.Add (header.Key, header.Value);
						}

						_loggerService.Debug ("Headers", _httpClient.DefaultRequestHeaders.ToString ());
						HttpResponseMessage response = null;
						var retryCount = 1;

						while (retryCount <= MaxRetryTime) {
							var responseTask = _httpClient.PostAsync (url, content).Result;

							if (responseTask != null) {
								response = responseTask;
						
//								if (response.StatusCode == HttpStatusCode.Unauthorized) {
//									_loggerService.Error (TAG, "Gateway Timeout from server");
//									var i = Activator.CreateInstance<TResultObject> ();
//									i.ResultCode = HttpStatusCode.Unauthorized.ToString ();
//									i.ResultCode = "Unauthorized";
//									i.DisplayMessage = "არაავტორიზირებული მომხმარებელი";
//									return i;
//								}
								//Ignore in ncrunch until we mock HttpClient
								//ncrunch: no coverage start
								if (response.StatusCode == HttpStatusCode.GatewayTimeout) {
									_loggerService.Error (TAG, "Gateway Timeout from server");
									var i = Activator.CreateInstance<TResultObject> ();
									i.ResultCode = "504";
									i.ResultCode = "Unreachable";
									i.DisplayMessage = "მოხდა შეცდომა, სერვერთან კავშირისას";
									return i;
								}
								//ncrunch: no coverage end

								break;
							}

							//Ignore in ncrunch until we mock HttpClient
							//ncrunch: no coverage start
							_loggerService.Error (TAG, "POST request timeout {0}", url);
							retryCount++;
						}
						//ncrunch: no coverage end

						if (response.IsSuccessStatusCode) {
							var contentString = response.Content.ReadAsStringAsync ().Result;
							_loggerService.Debug (TAG, "Response: {0}", contentString);
							contentObject = JsonConvert.DeserializeObject<TResultObject> (contentString);
						} else {
							contentObject.ResultCode = response.StatusCode.ToString ();
							_loggerService.Debug (TAG, "Server returned error code " + response.StatusCode);
						}
					} catch (Exception ex) {
                        System.Diagnostics.Debug.WriteLine(ex);// TODO: Fix Exception handler
						_loggerService.Error (TAG, "Error connecting to unicard Api, error : {0}", ex.Message);
						_loggerService.Debug (TAG, ex.StackTrace);
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

		private Dictionary<string, string> GetRequiredHeaders (string contentType, string content)
		{
			var result = new Dictionary<string,string> ();

			var hash = _securityProvider.CalculateHash (content);
			result.Add ("x-authorization", string.Format ("UNICARDAPI{0}", hash));

			return result;
		}

		private bool CanConnect ()
		{
			if (_networkService.IsWifiReachable || _networkService.IsNetworkReachable) {
				_loggerService.Debug (TAG, "Network detected.");
				return true;
			}
	
			_loggerService.Debug (TAG, "Network Not detected.");
			return false;
		}
	}
}

