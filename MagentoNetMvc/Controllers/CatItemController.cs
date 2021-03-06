﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
//using MagentoNetMvc.MagentoNetCategoryClient;
using MagentoNetMvc.mg_dev.mdoresourcing.com;
using System.ServiceModel;
using System.Configuration;
using MagentoNetMvc.Models;

//using tempuri.org;
namespace MagentoNetMvc.Models
{
    public class CatItemModelContainer {
        public catalogCategoryEntity[] catItems;
        public CatItem catItem;
    }
}
namespace MagentoNetMvc.Controllers
{
	public class CatItemController : Controller
	{
        CatItemModelContainer catItemModelContainer = new CatItemModelContainer ();
		static String sessionId = null;
        static MagentoService client = null;

        void initMagentoService(){
            try{
                var soapConnectionString = System.Configuration.ConfigurationManager.AppSettings ["MySOAPConnectionString"];
                client = new MagentoService(soapConnectionString);
                if (String.IsNullOrEmpty(sessionId)){
                    var soapUsername = System.Configuration.ConfigurationManager.AppSettings ["MySOAPUserName"];
                    var soapPassword = System.Configuration.ConfigurationManager.AppSettings ["MySOAPPassword"];
                    sessionId = client.login (soapUsername, soapPassword);
                }
            } catch(Exception e){
                sessionId = "0";
            }
        }

        void fetchCatItems(MagentoService client, string sessionId, string topCategory){
            catalogCategoryTree catTree = client.catalogCategoryTree(sessionId, topCategory, "1");
            catItemModelContainer.catItems = catTree.children;
        }

        public ActionResult Get() {
            CatItem catItem = new CatItem ();
            return Json(catItem, JsonRequestBehavior.AllowGet);
        }
            

        public ActionResult List ()
        {
            initMagentoService ();
            if (client != null) {
                try {
                    this.fetchCatItems (client, sessionId, "2");
                } catch (Exception ex) {
                    // do ntohign for now
                }
            }

            CatItem catItem = new CatItem ();
            catItemModelContainer.catItem = catItem;

            ViewBag.sessionId = sessionId;
            return View ("Index", catItemModelContainer);
        }

		public ActionResult Index (int? id)
		{
			ViewBag.Message = "Welcome to ASP.NET MVC on Mono! (viewbag)";
			//			ViewData ["Message"] = "Welcome to ASP.NET MVC on Mono!";

/*
			// keeping this endpoint reference here in preps for when this service is ready for use
			// ... until then we will use the SOAP services provided by the Magento-php
			var binding = new BasicHttpBinding ();
			var address = new EndpointAddress( "http://192.168.1.109:8001/MagentoNetCategory.svc");

			ICategoryContract client = new CategoryContractClient (binding, address);
			GetCatItemResult catItem = client.GetCatItem(id);
*/
        
            initMagentoService ();
            CatItem catItem = new CatItem ();
            if (client != null) {
                try {
                    this.fetchCatItems(client, sessionId, "2");
                } catch (Exception ex) {
                    // do ntohign for now
                }
                if (id != null) {
                    try{
                        catalogCategoryInfo catInfo = client.catalogCategoryInfo (sessionId, (int)id, "1", null);
                        catItem.ID = 1;
                        catItem.Title = "test Title";
                        catItem.Name = catInfo.name;
                        catItem.Description = catInfo.description;

                    } catch (Exception ex) {
                        // do ntohign for now
                    }
                }
            }
            catItemModelContainer.catItem = catItem;

            ViewBag.sessionId = sessionId;
            return View (catItemModelContainer);
		}
    

	}
}

