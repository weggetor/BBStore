using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.FileSystem;

namespace Bitboxx.DNNModules.BBStore.Controls
{
    public partial class SelectFolderControl : PortalModuleBase
    {
        public string Permission { get; set; }

        public string Text
        {
            get { return ddlFolders.Text; }
            set { ddlFolders.Text = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            ddlFolders.Items.Clear();
            List<IFolderInfo> folders = FolderManager.Instance.GetFolders(PortalId, Permission, UserId).ToList();
            foreach (FolderInfo folder in folders)
            {
                ListItem folderItem = new ListItem();
                if (folder.FolderPath == Null.NullString)
                {
                    folderItem.Text = "/";
                }
                else
                {
                    folderItem.Text = folder.FolderPath;
                }
                folderItem.Value = folder.FolderPath;
                ddlFolders.Items.Add(folderItem);
            }
            
            base.OnInit(e);

            string FileName = System.IO.Path.GetFileNameWithoutExtension(this.AppRelativeVirtualPath);
            if (this.ID != null)
                //this will fix it when its placed as a ChildUserControl 
                this.LocalResourceFile = this.LocalResourceFile.Replace(this.ID, FileName);
            else
                // this will fix it when its dynamically loaded using LoadControl method 
                this.LocalResourceFile = this.LocalResourceFile + FileName + ".ascx.resx";
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            cmdCancel.Text = LocalizeString("cmdCancel.Text");
            cmdSave.Text = LocalizeString("cmdSave.Text");
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            if (txtNewFolder.Text != String.Empty)
            {
                string newFolder = ddlFolders.Text + txtNewFolder.Text;
                try
                {
                    FolderManager.Instance.AddFolder(PortalId,newFolder);
                    ddlFolders.Items.Add(new ListItem(newFolder+"/"));
                    ddlFolders.Text = newFolder + "/";
                    lblError.Visible = false;
                    lblError.Text = "";
                    txtNewFolder.Text = "";
                    pnlAddFolder.Visible = false;
                }
                catch (Exception ex)
                {
                    lblError.Text = ex.Message;
                    lblError.Visible = true;
                }
            }
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            txtNewFolder.Text = "";
            pnlAddFolder.Visible = false;
            lblError.Visible = false;
            lblError.Text = "";
        }

        protected void cmdAdd_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            pnlAddFolder.Visible = true;
        }
    }
}