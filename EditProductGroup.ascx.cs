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
using System.Collections;
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
    [DNNtc.PackageProperties("BBStore Product Groups")]
    [DNNtc.ModuleProperties("BBStore Product Groups")]
    [DNNtc.ModuleControlProperties("Edit", "Edit BBStore Product Groups", DNNtc.ControlType.Edit, "", false, false)]
    partial class EditProductGroup : PortalModuleBase
    {
        // Needed to add these because changeing something in ascx caused designer file to fallback to System.Web.UI.UserControl
        protected DotNetNuke.UI.UserControls.UrlControl ImageSelector;
        protected DotNetNuke.UI.UserControls.UrlControl IconSelector;

        #region "Private Members"
        private BBStoreController Controller;
		private ProductGroupInfo ProductGroup = null;
    	private string _imageDir = "";
		private string _iconDir = "";

        #endregion

        #region "Public Properties"
		public int ProductGroupId
		{
			get
			{
				if (ViewState["ProductGroupId"] != null)
					return (int)ViewState["ProductGroupId"];
				else
					return -1;
			}
			set
			{
				ViewState["ProductGroupId"] = value;
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

			    Hashtable storeSettings = Controller.GetStoreSettings(PortalId);
				if (storeSettings != null)
				{
					_imageDir = (string) (storeSettings["ProductGroupImageDir"] ?? "");
					_iconDir = (string) (storeSettings["ProductGroupIconDir"] ?? "");
				}

				if (Request["adminmode"] != null)
					pnlBack.Visible = false;

				ctlFeatureLists.AddButtonClick += ctlFeatureLists_AddButtonClick;
				ctlFeatureLists.AddAllButtonClick += ctlFeatureLists_AddAllButtonClick;
				ctlFeatureLists.RemoveButtonClick += ctlFeatureLists_RemoveButtonClick;
				ctlFeatureLists.RemoveAllButtonClick += ctlFeatureLists_RemoveAllButtonClick;

				if (!IsPostBack)
				{
					// styling treeview
					treeProductGroup.SelectedNodeStyle.BackColor = System.Drawing.Color.Red;
					treeProductGroup.SelectedNodeStyle.ForeColor = System.Drawing.Color.White;

					if (Request["productgroup"] != null)
						ProductGroupId = Convert.ToInt32(Request["productgroup"]);
					
					// Treeview Basenode
					TreeNode newNode = new TreeNode(Localization.GetString("treeProductGroups.Text", this.LocalResourceFile), "_-1");
					newNode.SelectAction = TreeNodeSelectAction.Select;
					newNode.PopulateOnDemand = true;
					newNode.ImageUrl = @"~\images\category.gif";
					newNode.ShowCheckBox = false;
					treeProductGroup.Nodes.Add(newNode);

					cmdDelete.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("DeleteConfirm.Text", this.LocalResourceFile) + "');");
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
            Response.Redirect(Globals.NavigateURL(), true);
        }
		protected void cmdAdd_Click(object sender, EventArgs e)
		{
			EditState = "new";

			ImageSelector.Url = _imageDir + "This_fileName-Should_not_3xist";
			IconSelector.Url = _iconDir + "This_fileName-Should_not_3xist";
			imgImage.ImageUrl = "";
			imgIcon.ImageUrl = "";
			urlTarget.Url = "-1";
			
			pnlProductGroupDetails.Visible = true;
            pnlProductGroupEditDetails.Visible = true;

			cmdAdd.Visible = false;
			cmdDelete.Visible = false;
		    lblFeatureLists.Visible = false;
		    ctlFeatureLists.Visible = false;
			cmdUpdate.Visible = true;
			cmdCancel.Visible = true;
			lblPGDetails.Text = Localization.GetString("NewGroup.Text", this.LocalResourceFile);

            List<ILanguageEditorInfo> dbLangs = new List<ILanguageEditorInfo>();
            dbLangs.Add(new ProductGroupLangInfo() { Language = CurrentLanguage });
            lngProductGroups.Langs = dbLangs;
		}
		protected void cmdUpdate_Click(object sender, EventArgs e)
		{
			lngProductGroups.UpdateLangs();
			switch (EditState)
			{
				case "new":
					ProductGroupInfo pgNew = new ProductGroupInfo();
					pgNew.ParentId = ProductGroupId;
					pgNew.PortalId = PortalId;
					pgNew.Image = BBStoreHelper.GetRelativeFilePath(ImageSelector.Url);
                    pgNew.Icon = BBStoreHelper.GetRelativeFilePath(IconSelector.Url);
					pgNew.ProductListTabId = Convert.ToInt32(urlTarget.Url);
					pgNew.Disabled = chkDisabled.Checked;
					pgNew.ViewOrder = Convert.ToInt32(txtViewOrder.Text);
					int productGroupId = Controller.NewProductGroup(pgNew);
					foreach (ProductGroupLangInfo pgl in lngProductGroups.Langs)
					{
						pgl.ProductGroupId = productGroupId;
						Controller.NewProductGroupLang(pgl);
					}
					ProductGroupId = productGroupId;
					EditState = "update";
					break;
				case "update":
					ProductGroupInfo pgUpdate = Controller.GetProductGroup(PortalId,ProductGroupId);
                    pgUpdate.Image = BBStoreHelper.GetRelativeFilePath(ImageSelector.Url);
                    pgUpdate.Icon = BBStoreHelper.GetRelativeFilePath(IconSelector.Url);
					pgUpdate.ProductListTabId = Convert.ToInt32(urlTarget.Url);
					pgUpdate.Disabled = chkDisabled.Checked;
					pgUpdate.ViewOrder = Convert.ToInt32(txtViewOrder.Text);
					Controller.UpdateProductGroup(pgUpdate);
					Controller.DeleteProductGroupLangs(ProductGroupId);
					foreach (ProductGroupLangInfo pgl in lngProductGroups.Langs)
					{
						pgl.ProductGroupId = ProductGroupId;
						Controller.NewProductGroupLang(pgl);
					}
					break;
				default:
					break;
			}
			if (Request.QueryString["adminmode"] != null)
				Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=productgroup"), true);
			else
				Response.Redirect(EditUrl(),true);
		}
		protected void cmdCancel_Click(object sender, EventArgs e)
		{
			if (Request.QueryString["adminmode"] != null)
				Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=productgroup"), true);
			else
				Response.Redirect(EditUrl(), true);
		}
		protected void cmdDelete_Click(object sender, EventArgs e)
		{
			Controller.DeleteProductGroup(ProductGroupId);
			if (Request.QueryString["adminmode"] != null)
				Response.Redirect(Globals.NavigateURL(TabId, "", "adminmode=productgroup"), true);
			else
				Response.Redirect(EditUrl(), true);
		}
		
        protected void treeProductGroup_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            // http://quickstarts.asp.net/QuickStartv20/aspnet/doc/ctrlref/navigation/treeview.aspx
            TreeNode parent = e.Node;
            int productGroupId;
            productGroupId = Convert.ToInt32(parent.Value.Substring(1));

            Controller = new BBStoreController();

            List<ProductGroupInfo> products = Controller.GetProductSubGroupsByNode(PortalId, CurrentLanguage, productGroupId, false,false,true);
            foreach (ProductGroupInfo p in products)
            {
                TreeNode newNode = new TreeNode(p.ProductGroupName, "_" + p.ProductGroupId.ToString());
                newNode.SelectAction = TreeNodeSelectAction.Select;
                newNode.PopulateOnDemand = true;
                newNode.ImageUrl = BBStoreHelper.FileNameToImgSrc(p.Icon, PortalSettings);
                parent.ChildNodes.Add(newNode);
            }
        }
		protected void treeProductGroup_SelectedNodeChanged(object sender, EventArgs e)
		{
			// Determine new ProductId
			string productGroupId = treeProductGroup.SelectedNode.Value;
			if (productGroupId.StartsWith("_"))
				ProductGroupId = Convert.ToInt32(productGroupId.Substring(1));

			// Enabling / Disabling Edit Controls
		    pnlProductGroupTree.Visible = false;
            pnlProductGroupDetails.Visible = true;

			cmdCancel.Visible = true;

			if (ProductGroupId > -1 || EditState == "new")
			{
			    pnlProductGroupEditDetails.Visible = true;
                cmdUpdate.Visible = true;
				BindFeatureListsData();
			}
			else
			{
			    pnlProductGroupEditDetails.Visible = false;
                cmdUpdate.Visible = false;
			}
			cmdAdd.Visible = (EditState != "new");

			// if product exists

			if (ProductGroupId > 0)
				ProductGroup = Controller.GetProductGroup(PortalId, CurrentLanguage, ProductGroupId);

			if (ProductGroup != null)
			{
				// Fill in the Language information
                List<ILanguageEditorInfo> dbLangs = new List<ILanguageEditorInfo>();
                foreach (ProductGroupLangInfo productGroupLang in Controller.GetProductGroupLangs(ProductGroup.ProductGroupId))
                {
                    dbLangs.Add(productGroupLang);
                }
                lngProductGroups.Langs = dbLangs;

				// Set Image Info
                int imageFileId = -1;
			    string imageUrl = "";
			    if (!String.IsNullOrEmpty(ProductGroup.Image))
			    {
			        IFileInfo imageFile = FileManager.Instance.GetFile(PortalId, ProductGroup.Image);
			        if (imageFile != null)
			            imageFileId = imageFile.FileId;
			    }
			    if (imageFileId > -1)
					imageUrl = "FileID=" + imageFileId.ToString();
				else
					imageUrl = _imageDir + "This_fileName-Should_not_3xist";

				// Set Icon Info
				int iconFileId = -1;
			    string iconUrl = "";
				if (!String.IsNullOrEmpty(ProductGroup.Icon))
                {
                    IFileInfo iconFile = FileManager.Instance.GetFile(PortalId, ProductGroup.Icon);
			        if (iconFile != null)
                        iconFileId = iconFile.FileId;
			    }
					
				if (iconFileId > -1)
					iconUrl = "FileID=" + iconFileId.ToString();
				else
					iconUrl = _iconDir + "This_fileName-Should_not_3xist";

			    ImageSelector.Url = imageUrl;
			    IconSelector.Url = iconUrl;
                imgImage.ImageUrl = BBStoreHelper.FileNameToImgSrc(imageUrl,PortalSettings);
                imgIcon.ImageUrl = BBStoreHelper.FileNameToImgSrc(iconUrl,PortalSettings);
				urlTarget.Url = ProductGroup.ProductListTabId.ToString();
				chkDisabled.Checked = ProductGroup.Disabled;
				txtViewOrder.Text = ProductGroup.ViewOrder.ToString();
			}
			else
			{
				ImageSelector.Url = "";
				IconSelector.Url = "";
				imgImage.ImageUrl = "";
				imgIcon.ImageUrl = "";
				urlTarget.Url = "";
				chkDisabled.Checked = false;
				txtViewOrder.Text = "0";
			}

			// If collapsed we have to expand first to see if there are childs
			if (treeProductGroup.SelectedNode.Expanded == false)
			{
				treeProductGroup.SelectedNode.Expand();
				treeProductGroup.SelectedNode.Collapse();
			}
			int childCount = treeProductGroup.SelectedNode.ChildNodes.Count;
			// We are only allowed to delete if no childs
			cmdDelete.Visible = (childCount == 0);
			lblPGDetails.Text = treeProductGroup.SelectedNode.Text + (ProductGroup == null ? "" : " (ID:" +ProductGroup.ProductGroupId.ToString() + ")");
			lblPGDetails.Visible = true;
		}
		protected void imgRefreshImg_Click(object sender, EventArgs e)
		{
            imgImage.ImageUrl = BBStoreHelper.FileNameToImgSrc(ImageSelector.Url,PortalSettings);
            imgIcon.ImageUrl = BBStoreHelper.FileNameToImgSrc(IconSelector.Url,PortalSettings);
		}

		void ctlFeatureLists_RemoveAllButtonClick(object sender, EventArgs e)
		{
			foreach (FeatureListInfo featureList in Controller.GetFeatureLists(PortalId))
			{
				Controller.DeleteProductGroupListItemsByProductGroupAndFeatureList(ProductGroupId, featureList.FeatureListId);
			}
			BindFeatureListsData();
		}

		void ctlFeatureLists_RemoveButtonClick(object sender, DotNetNuke.UI.WebControls.DualListBoxEventArgs e)
		{
			if (e.Items != null)
			{
				foreach (string featurelist in e.Items)
				{
					int featureListId = Convert.ToInt32(featurelist);
					Controller.DeleteProductGroupListItemsByProductGroupAndFeatureList(ProductGroupId, featureListId);
				}
			}
			BindFeatureListsData();
		}

		void ctlFeatureLists_AddAllButtonClick(object sender, EventArgs e)
		{
			foreach (FeatureListInfo featureList in Controller.GetFeatureLists(PortalId))
			{
				Controller.AddProductGroupListItemsByProductGroupAndFeatureList(ProductGroupId, featureList.FeatureListId);
			}
			BindFeatureListsData();
		}

		void ctlFeatureLists_AddButtonClick(object sender, DotNetNuke.UI.WebControls.DualListBoxEventArgs e)
		{
			if (e.Items != null)
			{
				foreach (string featurelist in e.Items)
				{
					int featureListId = Convert.ToInt32(featurelist);
					Controller.AddProductGroupListItemsByProductGroupAndFeatureList(ProductGroupId, featureListId);
				}
			}
			BindFeatureListsData();
		}
        #endregion

        #region "Helper methods"
		private void BindFeatureListsData()
		{
			ctlFeatureLists.Visible = false;
			lblFeatureLists.Visible = false;
			if (ProductGroupId > -1)
			{
				List<FeatureListInfo> allFeatureLists = Controller.GetFeatureLists(PortalId, CurrentLanguage);
				List<FeatureListInfo> selFeatureLists = Controller.GetSelectedFeatureListsByProductGroup(ProductGroupId, CurrentLanguage);

				foreach (FeatureListInfo featureList in selFeatureLists)
				{
					foreach (FeatureListInfo allFeatureList in allFeatureLists)
					{
						if (allFeatureList.FeatureListId == featureList.FeatureListId)
						{
							allFeatureLists.Remove(allFeatureList);
							break;
						}
					}
				}

				ctlFeatureLists.AvailableDataSource = allFeatureLists;
				ctlFeatureLists.SelectedDataSource = selFeatureLists;
				ctlFeatureLists.DataBind();
				ctlFeatureLists.Visible = true;
				lblFeatureLists.Visible = true;
			}
		}

        #endregion



	}
}
