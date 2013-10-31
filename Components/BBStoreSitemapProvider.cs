using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Sitemap;

namespace Bitboxx.DNNModules.BBStore
{
	public class BBStoreSitemapProvider: SitemapProvider
	{
		#region Overrides of SitemapProvider

		public override List<SitemapUrl> GetUrls(int portalId, PortalSettings ps, string version)
		{
			CultureInfo current = Thread.CurrentThread.CurrentCulture;
			CultureInfo currentUI = Thread.CurrentThread.CurrentUICulture;

			BBStoreController controller = new BBStoreController();
			List<SitemapUrl> retVal = new List<SitemapUrl>();

            ModuleController moduleController = new ModuleController();
			ArrayList mods = moduleController.GetModules(portalId);

            // Lets build the Languages Collection
            LocaleController lc = new LocaleController();
            Dictionary<string, Locale> loc = lc.GetLocales(PortalSettings.Current.PortalId);
			
            // Productgroups
            foreach (var mod in mods)
            {
                ModuleInfo modInfo = (ModuleInfo)mod;
                if (modInfo.ModuleDefinition.FriendlyName == "BBStore Product Groups")
                {
                    bool fixedRoot = (modInfo.ModuleSettings["RootLevelFixed"] != null && Convert.ToBoolean(modInfo.ModuleSettings["RootLevelFixed"]));
                    int rootLevel = (modInfo.ModuleSettings["RootLevel"] != null ? Convert.ToInt32(modInfo.ModuleSettings["RootLevel"]) : -1);
                    List<ProductGroupInfo> productGroups = new List<ProductGroupInfo>();
                    if (rootLevel > -1 && fixedRoot)
                        productGroups.Add(controller.GetProductGroup(portalId, rootLevel));
                    else if (rootLevel > -1 && !fixedRoot)
                        productGroups = controller.GetProductSubGroupsByNode(portalId, "en-US", rootLevel, false, false, false);
                    else if (rootLevel == -1 && fixedRoot)
                        productGroups.Add(new ProductGroupInfo() {ProductGroupId = -1});
                    else
                        productGroups = controller.GetProductGroups(portalId);

                    List<ProductGroupInfo> sortedGroups = productGroups.OrderBy(p => p.ProductGroupId).ToList();
                    foreach (ProductGroupInfo productGroup in sortedGroups)
                    {
                        foreach (KeyValuePair<string, Locale> lang in loc)
                        {
                            // Set language to product Language
                            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang.Key);
                            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang.Key);

                            // Lets check if we already have this
                            string name = "", url = "";
                            ProductGroupLangInfo pgl = controller.GetProductGroupLang(productGroup.ProductGroupId, lang.Key);
                            if (pgl != null)
                                name = HttpUtility.UrlEncode(pgl.ProductGroupName);
                            if (name != String.Empty)
                                url = Globals.NavigateURL(modInfo.TabID, "", "productGroup=" + productGroup.ProductGroupId.ToString(),"name=" + name);
                            else
                                url = Globals.NavigateURL(modInfo.TabID, "", "productGroup=" + productGroup.ProductGroupId.ToString());
                            if (retVal.Find(r => r.Url == url) == null)
                            {
                                var pageUrl = new SitemapUrl
                                                  {
                                                      Url = url,
                                                      Priority = (float) 0.5,
                                                      LastModified = DateTime.Now.AddDays(-1),
                                                      ChangeFrequency = SitemapChangeFrequency.Daily
                                                  };
                                retVal.Add(pageUrl);
                            }
                        }
                    }
                }
            }

		    foreach (var mod in mods)
		    {
		        // First we need to know where our ProductModule sits
                ModuleInfo modInfo = (ModuleInfo) mod;
		        if (modInfo.ModuleDefinition.FriendlyName == "BBStore Product" || modInfo.ModuleDefinition.FriendlyName == "BBStore Simple Product")
		        {
                    int tabId = modInfo.TabID;

		            int productId = -1;
		            if (modInfo.ModuleSettings["ProductId"] != null)
		                productId = Convert.ToInt32(modInfo.ModuleSettings["ProductId"]);
 
		            if (productId < 0)
		            {
		                // We've got a dynamic module ! Here we show all products that are not disbaled !
                        List<SimpleProductInfo> products = controller.GetSimpleProducts(portalId).FindAll(p => p.Disabled == false);
                        foreach (SimpleProductInfo product in products)
                        {
                            foreach (KeyValuePair<string, Locale> lang in loc)
                            {
                                // Set language to product Language
                                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang.Key);
                                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang.Key);

                                // Lets check if we already have this
                                string name = "", url = "";
                                SimpleProductLangInfo spl = controller.GetSimpleProductLang(product.SimpleProductId, lang.Key);
                                if (spl != null)
                                    name = HttpUtility.UrlEncode(spl.Name);
                                if (name != string.Empty)
                                    url = Globals.NavigateURL(tabId, "", "productid=" + product.SimpleProductId.ToString(), "name=" + name);
                                else
                                    url = Globals.NavigateURL(tabId, "", "productid=" + product.SimpleProductId.ToString());

                                if (retVal.Find(r => r.Url == url) == null)
                                {
                                    var pageUrl = new SitemapUrl
                                    {
                                        Url = url,
                                        Priority = (float)0.3,
                                        LastModified = product.LastModifiedOnDate,
                                        ChangeFrequency = SitemapChangeFrequency.Daily
                                    };
                                    retVal.Add(pageUrl);
                                }
                            }
                        }
		            }
		            else
		            {
		                // This is a fixed module !
                        SimpleProductInfo product = controller.GetSimpleProductByProductId(portalId, productId);
		                foreach (KeyValuePair<string, Locale> lang in loc)
		                {
		                    // Set language to product Language
		                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang.Key);
		                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang.Key);

		                    // Lets check if we already have this
		                    string name = "", url = "";
		                    SimpleProductLangInfo spl = controller.GetSimpleProductLang(productId, lang.Key);
		                    if (spl != null)
		                        name = HttpUtility.UrlEncode(spl.Name);
		                    if (name != string.Empty)
		                        url = Globals.NavigateURL(tabId, "", "name=" + name);
		                    else
		                        url = Globals.NavigateURL(tabId);

		                    if (retVal.Find(r => r.Url == url) == null)
		                    {
		                        var pageUrl = new SitemapUrl
		                                          {
		                                              Url = url,
		                                              Priority = (float) 0.4,
		                                              LastModified = product.LastModifiedOnDate,
		                                              ChangeFrequency = SitemapChangeFrequency.Daily
		                                          };
		                        retVal.Add(pageUrl);
		                    }
		                }
		            }
		        }

                // Reset values
                Thread.CurrentThread.CurrentCulture = current;
                Thread.CurrentThread.CurrentUICulture = currentUI;
		    }
		    return retVal;
		}

		#endregion
	}
}