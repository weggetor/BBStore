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

using System;
using System.IO;
using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Modules;
using System.Text;
using DotNetNuke.Services.FileSystem;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections;
using System.Web.UI;
using DotNetNuke.UI.Skins.Controls;
using System.Threading;
using System.Web;
using System.Data;

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
    partial class EditFeatureList : PortalModuleBase
    {

        #region "Private Members"
        private BBStoreController Controller;
		private FeatureListInfo FeatureList = null;

        #endregion

        #region "Public Properties"
		public int FeatureListId
		{
			get
			{
				if (ViewState["FeatureListId"] != null)
					return (int)ViewState["FeatureListId"];
				else
					return -1;
			}
			set
			{
				ViewState["FeatureListId"] = value;
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
					List<FeatureListInfo> featureLists = Controller.GetFeatureLists(PortalId, CurrentLanguage);
					featureLists.Insert(0, new FeatureListInfo() {FeatureList = "<Add new>", FeatureListId = -1});
					lstFeatureList.DataSource = featureLists;
					lstFeatureList.DataTextField = "FeatureList";
					lstFeatureList.DataValueField = "FeatureListId";
					lstFeatureList.DataBind();

				    if (Request["featurelist"] != null)
				    {
				        FeatureListId = Convert.ToInt32(Request["featurelist"]);
				        lstFeatureList.SelectedValue = FeatureListId.ToString();
                        lstFeatureList_SelectedIndexChanged(lstFeatureList,new EventArgs());
				    }


				    cmdDelete.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("DeleteConfirm.Text", this.LocalResourceFile) + "');");
					lblFLDetailsCaption.Text = Localization.GetString("lblFLDetailsCaption.Text", this.LocalResourceFile);
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
				Response.Redirect(Globals.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
		protected void cmdUpdate_Click(object sender, EventArgs e)
		{
			lngFeatureLists.UpdateLangs();
            switch (EditState)
			{
				case "new":
					FeatureListInfo flNew = new FeatureListInfo();
					flNew.FeatureListId = FeatureListId;
					flNew.PortalID = PortalId;
					int featureListId = Controller.NewFeatureList(flNew);
					foreach (FeatureListLangInfo featureListLang in lngFeatureLists.Langs)
					{
						featureListLang.FeatureListId = featureListId;
						Controller.NewFeatureListLang(featureListLang);
					}
					FeatureListId = featureListId;
					EditState = "update";
					break;
				case "update":
					Controller.DeleteFeatureListLangs(FeatureListId);
					foreach (FeatureListLangInfo featureListLang in lngFeatureLists.Langs)
					{
						featureListLang.FeatureListId = FeatureListId;
						Controller.NewFeatureListLang(featureListLang);
					}
					break;
				default:
					break;
			}
			if (Request.QueryString["adminmode"] != null)
				Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=featurelist"), true);
			else
				Response.Redirect(EditUrl(),true);
		}
		protected void cmdEditItems_Click(object sender, EventArgs e)
		{
			if (Request.QueryString["adminmode"] != null)
				Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=featurelistitem", "featurelist=" + FeatureListId.ToString()), true);
			else
				Response.Redirect(EditUrl(), true);
		}
		protected void cmdCancel_Click(object sender, EventArgs e)
		{
			if (Request.QueryString["adminmode"] != null)
				Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=featurelist"), true);
			else
				Response.Redirect(EditUrl(), true);
		}
		protected void cmdDelete_Click(object sender, EventArgs e)
		{
			Controller.DeleteFeatureList(FeatureListId);
			if (Request.QueryString["adminmode"] != null)
				Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=featurelist"), true);
			else
				Response.Redirect(EditUrl(), true);
		}
		
		protected void lstFeatureList_SelectedIndexChanged(object sender, EventArgs e)
		{

			lstFeatureList.Attributes.Add("onClick", "javascript:alert('" + Localization.GetString("LstDisabled.Text", this.LocalResourceFile) + "');");
			
			// Determine new Id
			FeatureListId = Convert.ToInt32(lstFeatureList.SelectedValue);
			if (FeatureListId == -1)
			{
				EditState = "new";
                lngFeatureLists.Langs = new List<ILanguageEditorInfo>();
				pnlFeatureListDetails.Visible = true;
				lblFLDetails.Text = Localization.GetString("NewFeatureList.Text", this.LocalResourceFile);
			}


			// Enabling / Disabling Edit Controls
		    pnlFeatureLists.Visible = false;

			bool pnlVisible = (FeatureListId > -1 || EditState == "new");

			pnlFeatureListDetails.Visible = pnlVisible;
			cmdUpdate.Visible = pnlVisible;
			cmdCancel.Visible = true;
			cmdDelete.Visible = (EditState != "new");
			imgEditItems.Visible = (EditState != "new");
		
			if (FeatureListId > 0)
				FeatureList = Controller.GetFeatureListById(FeatureListId, CurrentLanguage);

			if (FeatureList != null)
			{
				// Fill in the Language information
                List<ILanguageEditorInfo> dbLangs = new List<ILanguageEditorInfo>();
                foreach (FeatureListLangInfo featureListLang in Controller.GetFeatureListLangs(FeatureList.FeatureListId))
			    {
			        dbLangs.Add(featureListLang);
			    }
			    lngFeatureLists.Langs = dbLangs;
			}

            lblFLDetails.Text = String.Format("{0} (ID:{1})",lstFeatureList.SelectedItem.Text,FeatureListId);
			lblFLDetails.Visible = true;
			lblFLDetailsCaption.Visible = true;
			UpdateList(CurrentLanguage);
		}
        #endregion

        #region "Helper methods"
        
		private void UpdateList(string language)
		{
			List<FeatureListItemInfo> lst = Controller.GetFeatureListItemsByListId(FeatureListId, language, false);
			pnlFeatureListItems.Visible = (lst.Count > 0);
			string html = "<ul>";
			foreach (FeatureListItemInfo featureListItem in lst)
			{
				html += "<li>" + featureListItem.FeatureListItem.Trim() + "</li>";
			}
			html += "</ul>";
			ltrListItems.Text = html;
		}

		public string FileNameToImgSrc(string FileName)
		{
			string sSrc = GetRelativeFilePath(FileName);
			return this.PortalSettings.HomeDirectory + sSrc;
		}
		public string GetRelativeFilePath(string FileName)
		{
		    if (FileName.StartsWith("FileID="))
			{
				int fileId = int.Parse(FileName.Substring(7));
				IFileInfo file = FileManager.Instance.GetFile(fileId);
				if (file != null)
				{
					return Path.Combine(file.Folder , file.FileName);
				}
			    return "";
			}
		    return FileName;
		}

        #endregion

		



	}
}
