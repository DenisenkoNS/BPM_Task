using BPM_Task.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BPM_Task.Controllers
{
	public class HomeController : Controller
	{
		private IEnumerable<Contact> contacts;
		public HomeController()
		{
			contacts = ContactService.GetOdataCollection();
		}
		public ActionResult Index()
		{
			
			return View(contacts);
		}
		public ViewResult Create()
		{
			return View(new Contact());
		}
		[HttpPost]
		public ActionResult Create(Contact newContact)
		{
				ContactService.CreateContact(newContact);
				return RedirectToAction("Index");
		}
		public ViewResult Edit(Guid contactId)
		{
			
			Contact contact = contacts.FirstOrDefault(p => p.Id == contactId);
			return View(contact);
		}
		[HttpPost]
		public ActionResult Save(Contact contact)
		{
				ContactService.UdateContact(contact);
				return RedirectToAction("Index");
		}
		public ActionResult Delete(Guid contactId)
		{
				ContactService.DeleteContact(contactId);
				return RedirectToAction("Index");
		}
		
	}
}