using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
namespace BPM_Task.Models
{
	public class LoginClass
	{
		private static Uri serverUri = new Uri("http://shedko.beesender.com/ServiceModel/AuthService.svc/Login");
		public static CookieContainer AuthCookie = new CookieContainer();
		public static bool TryLogIn (string userName ,string userPassword)
		{
			var authRequest = WebRequest.Create(serverUri) as HttpWebRequest;
			authRequest.Method = "POST";
			authRequest.ContentType = "application/json";
			authRequest.CookieContainer = AuthCookie;
			using(var requestStream = authRequest.GetRequestStream())
			{
				using (var writer = new StreamWriter(requestStream))
				{
					writer.Write(@"{
                    ""UserName"":""" + userName + @""",
                    ""UserPassword"":""" + userPassword + @"""
                    }");
				}
			}
			ResponseStatus status = null;
			using (var response = (HttpWebResponse)authRequest.GetResponse())
			{
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					// Десериализация HTTP-ответа во вспомогательный объект.
					string responseText = reader.ReadToEnd();
					status = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ResponseStatus>(responseText);
				}

			}

			if (status != null)
			{
				if (status.Code == 0)
				{
					return true;
				}
				
			}
			return false;

		}



	}
	
	class ResponseStatus
	{
		public int Code { get; set; }
		public string Message { get; set; }
		public object Exception { get; set; }
		public object PasswordChangeUrl { get; set; }
		public object RedirectUrl { get; set; }
	}
}
