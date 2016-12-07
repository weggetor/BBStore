using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Log.EventLog;

namespace Bitboxx.DNNModules.BBStore.Components
{
    public class WebThumb
    {
        public string ModuleFolder {get; set; }
        public string PortalAlias { get; set; }

        public WebThumb(string moduleFolder, string portalAlias)
        {
            ModuleFolder = moduleFolder;
            PortalAlias = portalAlias;
        }
        public void MakeThumb(string html, string thumbfile)
        {
            EventLogController ctrl = new EventLogController();

            string tempPath = Path.Combine(Globals.ApplicationMapPath, ModuleFolder) + "\\WebThumb\\";

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            string tempFile = Guid.NewGuid() + ".htm";
            File.WriteAllText(tempPath + tempFile, html);
            string url = PortalAlias + "/" + ModuleFolder.Replace("\\","/") +"/WebThumb/" + tempFile;

            ctrl.AddLog("Webthumb", $"tempPath={tempPath}, tempFile={tempFile}, url={url}",PortalSettings.Current,-1, EventLogController.EventLogType.ADMIN_ALERT);

            Process process = new Process();
            process.StartInfo.FileName = Path.Combine(tempPath, "wkhtmltoimage.exe");
            process.StartInfo.Arguments = @"--width 1024 --disable-smart-width --load-error-handling ignore " + url + " " + thumbfile;
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit(10000);

            File.Delete(tempPath + tempFile);
        }
    }
}