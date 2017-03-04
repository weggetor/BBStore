using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.DynamicData;
using System.Xml.Linq;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Scheduling;


namespace Bitboxx.DNNModules.BBStore.Components
{
    public class BBStoreScheduler : SchedulerClient
    {
        public BBStoreScheduler(ScheduleHistoryItem oItem)
            : base()
        {
            this.ScheduleHistoryItem = oItem;
        }

        public override void DoWork()
        {
            try
            {
                this.Progressing();
                this.ScheduleHistoryItem.AddLogNote("Start purging old carts...<br>");
                BBStoreController ctrl = new BBStoreController();

                PortalController pc = new PortalController();
                var portals = pc.GetPortals();
                foreach (PortalInfo portal in portals)
                {
                    Hashtable cartsettings = ctrl.GetCartSettings(portal.PortalID);
                    
                    int interval = Convert.ToInt32(cartsettings["PurgeInterval"] ?? "0");
                    if (interval > 0)
                    {
                        ctrl.PurgeCarts(portal.PortalID, interval);
                        this.ScheduleHistoryItem.AddLogNote("Deleting carts in portal '" + portal.PortalName + 
                            "' older than " + DateTime.Today.AddDays((-1) * interval).ToShortDateString() + "...<br>");
                    }
                }
                
                this.ScheduleHistoryItem.AddLogNote("Purging carts finished.");
                this.ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote("Exception: " + ex.ToString());
                this.Errored(ref ex);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
            }
        }
    }
}