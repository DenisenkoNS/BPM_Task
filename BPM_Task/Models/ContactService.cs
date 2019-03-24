using System;
using System.Data.Services.Client;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Web.Script.Serialization;

namespace BPM_Task.Models
{
	public class ContactService
	{
		private static Uri serverUri = new Uri("http://shedko.beesender.com/0/serviceModel/entitydataservice.svc/");
		private static BPMonline context = new BPMonline(serverUri);
		private const string authServiceUtri = "http://shedko.beesender.com/ServiceModel/AuthService.svc/Login";

		private static readonly XNamespace ds = "http://schemas.microsoft.com/ado/2007/08/dataservices";
		private static readonly XNamespace dsmd = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
		private static readonly XNamespace atom = "http://www.w3.org/2005/Atom";
		public static void OnSendingRequestCookie(object sender, SendingRequestEventArgs e)
		{
			LoginClass.TryLogIn("Supervisor", "Supervisor");
			var req = e.Request as HttpWebRequest;
			// Добавление аутентификационных cookie в запрос на получение данных.
			req.CookieContainer = LoginClass.AuthCookie;
			e.Request = req;
			// Добавление CSRF-токена в заголовок запроса.
			CookieCollection cookieCollection = LoginClass.AuthCookie.GetCookies(new Uri("http://shedko.beesender.com/ServiceModel/AuthService.svc/Login"));
			string csrfToken = cookieCollection["BPMCSRF"].Value;
			((HttpWebRequest)e.Request).Headers.Add("BPMCSRF", csrfToken);
		}

		public static IEnumerable<Contact> GetOdataCollection()
		{
			context.SendingRequest += new EventHandler<SendingRequestEventArgs>(OnSendingRequestCookie);
			var allContacts = from contacts in context.ContactCollection select contacts;
			return allContacts;
		}
		
		public static void CreateContact(Contact contact)
		{
			var content = new XElement(dsmd + "properties",
						  new XElement(ds + "Name", contact.Name),
						  new XElement(ds + "MobilePhone", contact.MobilePhone),
						  new XElement(ds + "Dear", contact.Dear),
						  new XElement(ds + "JobTitle", contact.JobTitle),
						  new XElement(ds + "BirthDate", contact.BirthDate));
			var entry = new XElement(atom + "entry",
						new XElement(atom + "content",
						new XAttribute("type", "application/xml"), content));
			Console.WriteLine(entry.ToString());
			var request = (HttpWebRequest)HttpWebRequest.Create(serverUri + "ContactCollection/");
			request.Credentials = new NetworkCredential("Supervisor", "Supervisor");
			request.Method = "POST";
			request.Accept = "application/atom+xml";
			request.ContentType = "application/atom+xml;type=entry";
			using (var writer = XmlWriter.Create(request.GetRequestStream()))
			{
				entry.WriteTo(writer);
			}
			using (WebResponse response = request.GetResponse())
			{
			}

		}
		public static void UdateContact(Contact contact)
		{
			string contactId = contact.Id.ToString();
			var content = new XElement(dsmd + "properties",
						  new XElement(ds + "Name", contact.Name),
						  new XElement(ds + "MobilePhone", contact.MobilePhone),
						  new XElement(ds + "Dear", contact.Dear),
						  new XElement(ds + "JobTitle", contact.JobTitle),
						  new XElement(ds + "BirthDate", contact.BirthDate));
			var entry = new XElement(atom + "entry",
						new XElement(atom + "content",
						new XAttribute("type", "application/xml"), content));
			var request = (HttpWebRequest)HttpWebRequest.Create(serverUri
					+ "ContactCollection(guid'" + contactId + "')");
			request.Credentials = new NetworkCredential("Supervisor", "Supervisor");
			request.Method = "MERGE";
			request.Accept = "application/atom+xml";
			request.ContentType = "application/atom+xml;type=entry";
			using (var writer = XmlWriter.Create(request.GetRequestStream()))
			{
				entry.WriteTo(writer);
			}
			using (WebResponse response = request.GetResponse())
			{
			}
		}
		public static void DeleteContact(Guid ID)
		{
			string contactId = ID.ToString();
			var request = (HttpWebRequest)HttpWebRequest.Create(serverUri
					+ "ContactCollection(guid'" + contactId + "')");
			request.Credentials = new NetworkCredential("Supervisor", "Supervisor");
			request.Method = "DELETE";
			using (WebResponse response = request.GetResponse())
			{
			}
		}

	}
}