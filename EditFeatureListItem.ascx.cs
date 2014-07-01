// 
// DotNetNuke® - http://www.dotnetnuke.com 
// Copyright (c) 2002-2010 
// by DotNetNuke Corporation 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software. 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE. 
// 

using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Bitboxx.DNNModules.BBStore
{

    /// ----------------------------------------------------------------------------- 
    /// <summary> 
    /// The EditSampleModule class is used to manage content 
    /// </summary> 
    /// <remarks> 
    /// </remarks> 
    /// <history> 
    /// </history> 
    /// ----------------------------------------------------------------------------- 
    partial class EditFeatureListItem : PortalModuleBase
    {

        #region "Private Members"
        private BBStoreController Controller;
		private FeatureListItemInfo FeatureListItem = null;
    	private string _imageDir = "";

        #endregion

        #region "Public Properties"
		public int FeatureListItemId
		{
			get
			{
				if (ViewState["FeatureListItemId"] != null)
					return (int)ViewState["FeatureListItemId"];
				else
					return -1;
			}
			set
			{
				ViewState["FeatureListItemId"] = value;
			}
		}


		public int FeatureListId
		{
			get
			{
				if (Request.QueryString["FeatureList"] != null)
				{
					int result = -1;
					Int32.TryParse(Request.QueryString["FeatureList"], out result);
					return result;
				}
				return -1;
			}
		}
		public string EditState
		{
			get
			{
				if (ViewState["EditState"] != null)
					return (string)ViewState["EditState"];
				else
					return "update";
			}
			set
			{
				ViewState["EditState"] = value;
			}
		}

		protected string CurrentLanguage
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            }
        }
        protected string DefaultLanguage
        {
            get
            {
                return this.PortalSettings.DefaultLanguage;
            }
        }
        #endregion

        #region "Event Handlers"
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			string FileName = System.IO.Path.GetFileNameWithoutExtension(this.AppRelativeVirtualPath);
			if (this.ID != null)
				//this will fix it when its placed as a ChildUserControl 
				this.LocalResourceFile = this.LocalResourceFile.Replace(this.ID, FileName);
			else
				// this will fix it when its dynamically loaded using LoadControl method 
				this.LocalResourceFile = this.LocalResourceFile + FileName + ".ascx.resx";
		}

        protected void Page_Load(object sender, System.EventArgs e)
        {
			try
			{
				Controller = new BBStoreController();

				if (Request["adminmode"] != null)
					pnlBack.Visible = false;


				if (!IsPostBack)
				{
					List<FeatureListItemInfo> featureListItems = Controller.GetFeatureListItemsByListId(FeatureListId,CurrentLanguage, false);
					featureListItems.Insert(0, new FeatureListItemInfo() { FeatureListItem = "<Add new>", FeatureListItemId = -1 });
					lstFeatureListItems.DataSource = featureListItems;
					lstFeatureListItems.DataTextField = "FeatureListItem";
					lstFeatureListItems.DataValueField = "FeatureListItemId";
					lstFeatureListItems.DataBind();

		
					cmdDelete.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("DeleteConfirm.Text", this.LocalResourceFile) + "');");
					lblFLIDetailsCaption.Text = Localization.GetString("lblFLIDetailsCaption.Text", this.LocalResourceFile);
				}
			}
			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
        }

        protected void cmdBack_Click(object sender, EventArgs e)
        {
            try
            {
				Response.Redirect(Globals.NavigateURL(TabId,"","adminmode=featurelist"), true);
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
		protected void cmdUpdate_Click(object sender, EventArgs e)
		{
            lngFeatureListItems.UpdateLangs();
            switch (EditState)
			{
				case "new":
					FeatureListItemInfo fliNew = new FeatureListItemInfo();
					fliNew.FeatureListId = FeatureListId;
                    fliNew.Image = BBStoreHelper.GetRelativeFilePath(ImageSelector.Url);
					fliNew.ViewOrder = Convert.ToInt32(txtViewOrder.Text);
					int featureListItemId = Controller.NewFeatureListItem(fliNew);
					foreach (FeatureListItemLangInfo featureListItemLang in lngFeatureListItems.Langs)
					{
						featureListItemLang.FeatureListItemId = featureListItemId;
						Controller.NewFeatureListItemLang(featureListItemLang);
					}
					FeatureListItemId = featureListItemId;
					EditState = "update";
					break;

                case "update":
					FeatureListItemInfo fliUpdate = Controller.GetFeatureListItemById(FeatureListItemId);
                    fliUpdate.Image = BBStoreHelper.GetRelativeFilePath(ImageSelector.Url);
					fliUpdate.ViewOrder = Convert.ToInt32(txtViewOrder.Text);
					Controller.UpdateFeatureListItem(fliUpdate);

					Controller.DeleteFeatureListItemLangs(FeatureListItemId);
 					foreach (FeatureListItemLangInfo featureListItemLang in lngFeatureListItems.Langs)
					{
						featureListItemLang.FeatureListItemId = FeatureListItemId;
						Controller.NewFeatureListItemLang(featureListItemLang);
					}
					break;
				default:
					break;
			}
			if (Request.QueryString["adminmode"] != null)
				Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=featurelistitem", "featurelist=" + FeatureListId.ToString()), true);
			else
				Response.Redirect(EditUrl(),true);
		}

		protected void cmdCancel_Click(object sender, EventArgs e)
		{
			if (Request.QueryString["adminmode"] != null)
				Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=featurelistitem", "featurelist=" + FeatureListId.ToString()), true);
			else
				Response.Redirect(EditUrl(), true);
		}
		protected void cmdDelete_Click(object sender, EventArgs e)
		{
			Controller.DeleteFeatureListItem(FeatureListItemId);
			if (Request.QueryString["adminmode"] != null)
				Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=featurelistitem", "featurelist=" + FeatureListId.ToString()), true);
			else
				Response.Redirect(EditUrl(), true);
		}
		protected void imgRefreshImg_Click(object sender, EventArgs e)
		{
            imgImage.ImageUrl = BBStoreHelper.FileNameToImgSrc(ImageSelector.Url,PortalSettings);
		}

		protected void lstFeatureListItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			lstFeatureListItems.Attributes.Add("onClick", "javascript:alert('" + Localization.GetString("LstDisabled.Text", this.LocalResourceFile) + "');");
			
			// Determine new ProductId
			FeatureListItemId = Convert.ToInt32(lstFeatureListItems.SelectedValue);
			if (FeatureListItemId == -1)
			{
				EditState = "new";
                lngFeatureListItems.Langs = new List<ILanguageEditorInfo>();
				pnlFeatureListItemDetails.Visible = true;
				lblFLIDetails.Text = Localization.GetString("NewFeatureListItem.Text", this.LocalResourceFile);
			}


			// Enabling / Disabling Edit Controls
		    pnlFeatureListsItems.Visible = false;

			bool pnlVisible = (FeatureListItemId > -1 || EditState == "new");

			pnlFeatureListItemDetails.Visible = pnlVisible;
			cmdUpdate.Visible = pnlVisible;
			cmdCancel.Visible = true;
			cmdDelete.Visible = (EditState != "new");
			
			if (FeatureListItemId > 0)
				FeatureListItem = Controller.GetFeatureListItemById(FeatureListItemId, CurrentLanguage);

			if (FeatureListItem != null)
			{
				// Set Image Info
				
                int imageFileId = -1;
				if (!String.IsNullOrEmpty(FeatureListItem.Image))
				{
				    IFileInfo file = FileManager.Instance.GetFile(PortalId, FeatureListItem.Image);
				    if (file != null)
				        imageFileId = file.FileId;
				}

			    string imageUrl = "";
                if (imageFileId > -1)
					imageUrl = "FileID=" + imageFileId.ToString();
				else
					imageUrl = _imageDir + "This_fileName-Should_not_3xist";

			    ImageSelector.Url = imageUrl;
                imgImage.ImageUrl = BBStoreHelper.FileNameToImgSrc(imageUrl,PortalSettings);
				txtViewOrder.Text = FeatureListItem.ViewOrder.ToString();
				
				// Fill in the Language information
                List<ILanguageEditorInfo> dbLangs = new List<ILanguageEditorInfo>();
                foreach (FeatureListItemLangInfo featureListItemLang in Controller.GetFeatureListItemLangs(FeatureListItem.FeatureListItemId))
                {
                    dbLangs.Add(featureListItemLang);
                }
                lngFeatureListItems.Langs = dbLangs;
			}

			lblFLIDetails.Text = String.Format("{0} (ID:{1})",lstFeatureListItems.SelectedItem.Text,FeatureListItemId);
			lblFLIDetails.Visible = true;
			lblFLIDetailsCaption.Visible = true;
		}
		#endregion
	}
}
