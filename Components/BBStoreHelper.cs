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
    }
}