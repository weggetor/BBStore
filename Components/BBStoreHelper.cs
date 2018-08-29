using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.FileSystem;

namespace Bitboxx.DNNModules.BBStore
{
    public class BBStoreHelper
    {
        public static string FileNameToImgSrc(string fileName, PortalSettings settings)
        {
            string sSrc = GetRelativeFilePath(fileName);
            return settings.HomeDirectory + sSrc;
        }

        public static string GetRelativeFilePath(string fileName)
        {
            if (fileName.StartsWith("FileID="))
            {
                int fileId = int.Parse(fileName.Substring(7));
                IFileInfo file = FileManager.Instance.GetFile(fileId);
                if (file != null)
                {
                    return Path.Combine(file.Folder, file.FileName);
                }
                return "";
            }
            return fileName;
        }

        /// <summary>
        /// Creates a log type and config
        /// </summary>
        /// <param name="logTypeKey">Unique key for the event type</param>
        /// <param name="logTypeFriendlyName">Display in log viewer</param>
        /// <param name="logTypeDescription">Description</param>
        /// <param name="logTypeCSSClass">The CSS class to render in the event viewer</param>
        /// <param name="logTypeOwner">The event class type</param>
        public static void CreateLogType(string logTypeKey, string logTypeFriendlyName, string logTypeDescription, string logTypeCssClass, string logTypeOwner)
        {
            try
            {
                DotNetNuke.Services.Log.EventLog.LogController lc = new DotNetNuke.Services.Log.EventLog.LogController();
                DotNetNuke.Services.Log.EventLog.LogTypeInfo lti = new DotNetNuke.Services.Log.EventLog.LogTypeInfo();
                lti.LogTypeCSSClass = logTypeCssClass;
                lti.LogTypeDescription = logTypeDescription;
                lti.LogTypeFriendlyName = logTypeFriendlyName;
                lti.LogTypeOwner = logTypeOwner;// "DotNetNuke.Logging.EventLogType";
                lti.LogTypeKey = logTypeKey;
                lc.AddLogType(lti);
                DotNetNuke.Services.Log.EventLog.LogTypeConfigInfo ltci = new DotNetNuke.Services.Log.EventLog.LogTypeConfigInfo();
                ltci.LogTypeKey = logTypeKey;
                ltci.LoggingIsActive = true;
                ltci.MailFromAddress = "";
                ltci.MailToAddress = "";
                ltci.EmailNotificationIsActive = false;
                ltci.KeepMostRecent = "10";
                ltci.LogTypePortalID = "-1";
                lc.AddLogTypeConfigInfo(ltci);
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
            }
        }

    }
}