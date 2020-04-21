// 
// DotNetNuke® - http://www.dotnetnuke.com 
// Copyright (c) 2002-2009 
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bitboxx.License;
using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Skins.Controls;

using DotNetNuke.Services.GeneratedImage;
using DotNetNuke.Services.FileSystem;

namespace Bitboxx.DNNModules.BBStore
{

	/// ----------------------------------------------------------------------------- 
	/// <summary> 
	/// The ViewBBStore class displays the content 
	/// </summary> 
	/// <remarks> 
	/// </remarks> 
	/// <history> 
	/// </history> 
	/// ----------------------------------------------------------------------------- 
    [DNNtc.PackageProperties("BBStore Product Groups", 3, "BBStore Product Groups", "BBStore Product Groups", "BBStore.png", "Torsten Weggen", "bitboxx solutions", "http://www.bitboxx.net", "info@bitboxx.net", false)]
    [DNNtc.ModuleProperties("BBStore Product Groups", "BBStore Product Groups", 0)]
    [DNNtc.ModuleControlProperties("", "BBStore Product Groups", DNNtc.ControlType.View, "", false, false)]
	partial class ViewProductGroup : PortalModuleBase, IActionable
	{
		#region Private Members
		private const string Currency = "EUR";
		
		BBStoreController _controller;

		private Label _lblProductCount;
		private GeneratedImage _imgProductGroup;
		private bool _isConfigured = false;
		private bool _setTitle = false;
        private bool _isVisible = true;
		private Image _imgIcon;
		private int _productGroupsInRow = 1;
	    private string _template = "";
		#endregion

		#region Public Properties
		public Guid FilterSessionId
		{
			get
			{
				string _filterSessionId;
				if (Request.Cookies["BBStoreFilterSessionId_"+ PortalId.ToString()] != null)
					_filterSessionId = (string)(Request.Cookies["BBStoreFilterSessionId_" + PortalId.ToString()].Value);
				else
				{
					_filterSessionId = Guid.NewGuid().ToString();
					HttpCookie keks = new HttpCookie("BBStoreFilterSessionId_" + PortalId.ToString());
					keks.Value = _filterSessionId;
					keks.Expires = DateTime.Now.AddDays(1);
					Response.AppendCookie(keks);
				}
				return new Guid(_filterSessionId);
			}
		}
		public BBStoreController Controller
		{
			get
			{
				if (_controller == null)
					_controller = new BBStoreController();
				return _controller;
			}
		}
		
		public int ProductGroupId
		{
			get
			{
				if (ViewState["ProductGroupId"] != null)
					return Convert.ToInt32(ViewState["ProductGroupId"]);
                else
					return -1;
			}
			set
			{
				ViewState["ProductGroupId"] = value;
			}
		}
		public List<ProductGroupInfo> ProductGroups
		{
			get
			{
				if (ViewState["ProductGroups"] != null)
					return (List<ProductGroupInfo>)ViewState["ProductGroups"];
				else
					return Controller.GetProductSubGroupsByNode(PortalId,CurrentLanguage,ProductGroupId, ShowProductCount,IncludeChilds,IncludeDisabled);
			}
			set
			{
				ViewState["ProductGroups"] = value;
			}
		}

        public ModuleKindEnum ModuleKind
        {
            get { return ModuleKindEnum.ProductGroup; }
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
		protected bool ShowProductCount
		{
			get
			{
				if (Settings["ShowProductCount"] != null)
					return Convert.ToBoolean((string)Settings["ShowProductCount"]);
				else
					return false;
			}
		}
		protected bool ShowIcons
		{
			get
			{
				if (Settings["ShowIcons"] != null)
					return Convert.ToBoolean((string)Settings["ShowIcons"]);
				else
					return false;
			}
		}
		protected bool IncludeChilds
		{
			get
			{
				bool includeChilds = false;
				if (Request.QueryString["includechilds"] != null)
					Boolean.TryParse(Request.QueryString["includechilds"], out includeChilds);
				if (!includeChilds && Settings["IncludeChilds"] != null)
					includeChilds = Convert.ToBoolean((string)Settings["IncludeChilds"]);
				return includeChilds;
			}
		}
		
		protected int ShowLevels
		{
			get
			{
				if (Settings["ShowLevels"] != null)
					return Convert.ToInt32((string)Settings["ShowLevels"]);
				else
					return 0;
			}
		}
		protected bool IncludeDisabled
		{
			get
			{
				UserInfo user = UserController.Instance.GetCurrentUserInfo();
				return (user.IsInRole("Administrators") && IsEditable);
			}
		}

	    protected string Template
	    {
	        get
	        {
	            if (_template == string.Empty)
	            {
                    TemplateControl tp = LoadControl("controls/TemplateControl.ascx") as TemplateControl;
                    tp.Key = "ProductGroup";
                    _template = tp.GetTemplate((string)Settings["Template"]);
	            }
	            return _template;
	        }
	    }
		#endregion

        #region "Optional Interfaces"

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Registers the module actions required for interfacing with the portal framework 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        /// <history> 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                actions.Add(GetNextActionID(), Localization.GetString("cmdEdit.Text", this.LocalResourceFile), ModuleActionType.EditContent, "", "edit.gif", EditUrl(), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                return actions;
            }
        }
        #endregion

		#region Event Handlers
		protected void Page_Init(object sender, System.EventArgs e)
		{
			// Are we in Admin mode ?
			UserInfo user = UserController.Instance.GetCurrentUserInfo();
			if (user.IsInRole("Administrators") && IsEditable)
				pnlAdmin.Visible = true;

			if (Settings["ViewMode"] != null)
				MultiView1.ActiveViewIndex = Int32.Parse((string)Settings["ViewMode"]);
			else
				MultiView1.ActiveViewIndex = 1;

			if (Settings["SetTitle"] != null)
				_setTitle = Convert.ToBoolean((string)Settings["SetTitle"]);

			if (Settings["ProductGroupsInRow"] != null  )
			{
				if (!Int32.TryParse((string)Settings["ProductGroupsInRow"], out _productGroupsInRow))
					_productGroupsInRow = 1;

				_isConfigured = true;
				lstProductGroups.GroupItemCount = _productGroupsInRow;
				lstProductGroupsBS3.GroupItemCount = _productGroupsInRow;
				if (Settings["RootLevel"] != null && Settings["RootLevelFixed"] != null && Convert.ToBoolean(Settings["RootLevelFixed"]) == true)
				{
					ProductGroupId = Convert.ToInt32(Settings["RootLevel"]);
				}
				else
				{
					if (Request["productgroup"] != null)
					{
						ProductGroupId = Convert.ToInt32(Request["productgroup"]);
					}
					else
					{
						ProductGroupId = -1;

                        if (Settings["RootLevel"] != null)
							ProductGroupId = Convert.ToInt32(Settings["RootLevel"]);

						List<ProductFilterInfo> fi = Controller.GetProductFilter(PortalId, FilterSessionId, "ProductGroup");

						if (fi.Count > 0)
						{
							string[] values = fi[0].FilterValue.Split('|');
							int productGroupId = Convert.ToInt32(values[0]);

							if (productGroupId > -1)
							{
								if (ProductGroupId > -1)
								{
									// we need to check if productGroupId is in path 
									string path = Controller.GetProductGroupPath(PortalId, productGroupId, CurrentLanguage, true, "|", "", "");

									if (path.Contains("|_" + ProductGroupId) || path.Contains("_" + ProductGroupId + "|") ||
									    path == "_" + ProductGroupId)
										ProductGroupId = productGroupId;
								}
								else
									ProductGroupId = productGroupId;
							}
						}

					}
                    if (ProductGroupId == -1 && Settings["DefaultProductGroup"] != null)
                        ProductGroupId = Convert.ToInt32(Settings["DefaultProductGroup"]);
                }

                SetFilter(ProductGroupId, IncludeChilds);

				if (Settings["WrapNode"] != null)
					treeProductGroup.NodeWrap = Convert.ToBoolean(Settings["WrapNode"]);
				treeProductGroup.NodeStyle.CssClass = "bbstore-productgroup-treenode";
                treeProductGroup.SelectedNodeStyle.CssClass = "bbstore-productgroup-selectedtreenode";
                treeProductGroup.HoverNodeStyle.CssClass = "bbstore-productgroup-hovertreenode";
			}
			else
			{
				string message = Localization.GetString("Configure.Message", this.LocalResourceFile);
				DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.YellowWarning);
			}
		}
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
                if (_isConfigured)
				{
					switch (MultiView1.ActiveViewIndex)
					{
						case 0:
						case 3:
							//if (!Page.IsPostBack)
							{
								List<ProductGroupInfo> productGroups = new List<ProductGroupInfo>();
                                ProductGroupInfo thisgroup = Controller.GetProductGroup(PortalId, CurrentLanguage, ProductGroupId);

                                if (Settings["ShowUpNavigation"] != null && Convert.ToBoolean(Settings["ShowUpNavigation"]))
								{
									if (thisgroup != null)
									{
										if (thisgroup.ParentId != -1)
										{
											ProductGroupInfo parentGroup = Controller.GetProductGroup(PortalId, CurrentLanguage, thisgroup.ParentId);
											if (parentGroup != null)
											{
												//parentGroup.Image = (string) Settings["Image"] ?? "";
												productGroups.Add(parentGroup);
											}
										}
										else
										{
											ProductGroupInfo dummyParent = new ProductGroupInfo();
											dummyParent.ParentId = -1;
											dummyParent.PortalId = PortalId;
											dummyParent.Image = (string) Settings["AllGroupsImage"] ?? "";
											dummyParent.ProductCount = 0;
											dummyParent.ProductGroupId = -1;
											dummyParent.ProductGroupName = Localization.GetString("AllGroups.Text", this.LocalResourceFile);
											dummyParent.ViewOrder = 0;

											productGroups.Add(dummyParent);
										}
									}
								}
							    
                                if (thisgroup!= null && Settings["ShowThisNode"] != null && Convert.ToBoolean(Settings["ShowThisNode"]))
							    {
							       productGroups.Add(thisgroup); 
							    }

                                if (Settings["ShowSubNodes"] == null || (Settings["ShowSubNodes"] != null && Convert.ToBoolean(Settings["ShowSubNodes"])))
							    {
							        productGroups.AddRange(Controller.GetProductSubGroupsByNode(PortalId, CurrentLanguage, ProductGroupId, ShowProductCount, IncludeChilds, IncludeDisabled));
							    }
							    ProductGroups = productGroups;
								if (ProductGroups.Count > 0)
								{
									if (MultiView1.ActiveViewIndex == 0)
									{
										lstProductGroups.DataSource = ProductGroups;
										lstProductGroups.DataBind();
									} else
									{
										lstProductGroupsBS3.DataSource = ProductGroups;
										lstProductGroupsBS3.DataBind();
									}
									
								}
								else
									_isVisible = false;
							}
							break;
						case 1:
							if (!Page.IsPostBack)
							{
								if (Settings["ShowExpandCollapse"] != null)
									treeProductGroup.ShowExpandCollapse = Convert.ToBoolean(Settings["ShowExpandCollapse"]);

								// Treeview Basenodes
								List<ProductGroupInfo> lpg = Controller.GetProductSubGroupsByNode(PortalId, CurrentLanguage, -1, ShowProductCount, IncludeChilds, IncludeDisabled);
								foreach (ProductGroupInfo pg in lpg)
								{
									string nodeName = pg.ProductGroupName +
										(ShowProductCount && pg.ProductCount > 0 ? " (" + pg.ProductCount + ")" : "");
									TreeNode newNode = new TreeNode(nodeName, "_" + pg.ProductGroupId.ToString());
									newNode.SelectAction = TreeNodeSelectAction.SelectExpand;
									newNode.PopulateOnDemand = true;
									if (ShowIcons)
										newNode.ImageUrl = BBStoreHelper.FileNameToImgSrc(pg.Icon,PortalSettings);
									treeProductGroup.Nodes.Add(newNode);
								}

								// And now we need to expand all nodes to the selected node
								string treePath = Controller.GetProductGroupPath(PortalId, ProductGroupId);
								string[] bread = treePath.Split('/');
								string valuePath = "";
								for (int i = 0; i < bread.Length; i++)
								{
									if (i > 0)
										valuePath += "/";
									valuePath += bread[i];
									TreeNode node = treeProductGroup.FindNode(valuePath);
									if (node != null)
									{
										node.Expand();
										//if (ProductGroupId != -1 && node.Value == "_" + ProductGroupId.ToString())
										//    node.Select();
									}
								}
							}
							break;
						case 2:
							string pgPath = Controller.GetProductGroupPath(PortalId, ProductGroupId);
							if (pgPath == string.Empty)
								pgPath = "_-1";
							else
								pgPath = "_-1/" + pgPath;
							string[] pgArr = pgPath.Split('/');

							for (int Level = 0; Level < pgArr.Length; Level++)
							{
								int Value = -1;
								if (Level < pgArr.Length - 1)
									Value = Convert.ToInt32(pgArr[Level + 1].Substring(1));
								int productGroup = Convert.ToInt32(pgArr[Level].Substring(1));
								DropDownList ddl = ProductGroupCombo(productGroup, Value, Level);
								if (ddl != null)
								{
									phDropDown.Controls.Add(ddl);
									phDropDown.Controls.Add(new LiteralControl("<br />"));
								}
							}

							break;
						default:
							break;
					}
				}
				else
				{
				    MultiView1.Visible = false;
				}
				
			}
			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			try
			{
                // We can set the Title of our Module
			    if (_setTitle)
			    {
			        bool breadcrumb = (Settings["ShowBreadcrumb"] != null ? Convert.ToBoolean((string) Settings["ShowBreadcrumb"]) : false);

			        string productGroupPath = "";
			        string root = Localization.GetString("MainProductGroup.Text", this.LocalResourceFile);
			        ProductGroupInfo pg = Controller.GetProductGroup(PortalId, CurrentLanguage, ProductGroupId);

			        if (pg != null)
			        {
			            int productGroupId = pg.ProductGroupId;
			            if (breadcrumb)
			            {
			                string link = Globals.NavigateURL(TabId, "", "productgroup={1}");
			                string linkTemplate = "<a href=\"" + link + "\">{0}</a>";
			                productGroupPath = Controller.GetProductGroupPath(PortalId, pg.ProductGroupId, CurrentLanguage, false, " > ", linkTemplate, root);
			            }
			            else if (productGroupId > -1)
			            {
			                productGroupPath = pg.ProductGroupName;
			            }
			        }


			        string titleLabelName = DotNetNukeContext.Current.Application.Version.Major < 6 ? "lblTitle" : "titleLabel";
			        Control ctl = Globals.FindControlRecursiveDown(this.ContainerControl, titleLabelName);
			        if (ctl != null)
			        {
			            ((Label) ctl).Text = productGroupPath != string.Empty ? productGroupPath : root;
			        }
			    }

			    if (!_isVisible && !IsEditable)
                    this.ContainerControl.Visible = false;
			}
			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}

            // Check licensing
            LicenseDataInfo license = Controller.GetLicense(PortalId, false);
            Controller.CheckLicense(license, this, ModuleKind);
		}
		protected void cmdEdit_Click(object sender, EventArgs e)
		{
			Response.Redirect(EditUrl());
		}
		protected void lstProductGroups_ItemCreated(object sender, ListViewItemEventArgs e)
		{

			if (_isConfigured)
			{
				
				ListView lv = sender as ListView;
				ListViewDataItem item = e.Item as ListViewDataItem;
				ProductGroupInfo productGroup = item.DataItem as ProductGroupInfo;
				if (productGroup != null)
				{
				    var ctrl = RenderItem(productGroup);

				    PlaceHolder ph = e.Item.FindControl("productGroupPlaceholder") as PlaceHolder;
					ph.Controls.Add(ctrl);
				}
			}
			else
			{
				lstProductGroups.Visible = false;
			}

		}

	    private Control RenderItem(ProductGroupInfo productGroup)
	    {
            int imageWidth = 200;
            string template = Template;
	        template = template.Replace("[PRODUCTGROUPNAME]", productGroup.ProductGroupName);
	        template = template.Replace("[PRODUCTGROUPSHORTDESCRIPTION]", productGroup.ProductGroupShortDescription);
	        template = template.Replace("[PRODUCTGROUPDESCRIPTION]", productGroup.ProductGroupDescription);

            template = template.Replace("[IMAGE]", "<img src=\"" + PortalSettings.HomeDirectory + productGroup.Image + "\" alt=\"" + productGroup.ProductGroupName + "\" />");

            int imageCnt = 0;
	        Queue<int> imageWidths = new Queue<int>();
	        while (template.Contains("[IMAGE:"))
	        {
	            imageCnt++;

	            string width = template.Substring(template.IndexOf("[IMAGE:") + 7);
	            width = width.Substring(0, width.IndexOf("]"));
	            if (Int32.TryParse(width, out imageWidth) == false)
	                imageWidth = 200;
	            imageWidths.Enqueue(imageWidth);

                template = template.ReplaceFirst("[IMAGE:" + width + "]", "<asp:PlaceHolder ID=\"phimgProductGroup" + imageCnt.ToString() + "\" runat=\"server\" />");
	        }

	        while (template.IndexOf("[RESOURCE:") > -1)
	        {
	            string resKey = template.Substring(template.IndexOf("[RESOURCE:") + 10);
	            resKey = resKey.Substring(0, resKey.IndexOf("]"));
	            template = template.Replace("[RESOURCE:" + resKey + "]",
	                Localization.GetString(resKey, this.LocalResourceFile));
	        }
	        template = template.Replace("[ICON]", "<asp:Image ID=\"imgIcon\" runat=\"server\" />");
	        template = template.Replace("[PRODUCTCOUNT]", "<asp:Label ID=\"lblProductCount\" runat=\"server\" />");

	        int linkCnt = 0;
	        while (template.Contains("[LINK]"))
	        {
	            linkCnt++;
	            template = template.ReplaceFirst("[LINK]", "<asp:Literal ID=\"ltrLink" + linkCnt.ToString() + "\" runat=\"server\" />");
	        }

	        Control ctrl = ParseControl(template);

	        imageCnt = 1;
            PlaceHolder phimgProductGroup = FindControlRecursive(ctrl, "phimgProductGroup" + imageCnt.ToString()) as PlaceHolder;
	        while (phimgProductGroup != null && productGroup.Image != null)
	        {
	            string fileName =
	                PortalSettings.HomeDirectoryMapPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, "") +
	                productGroup.Image.Replace('/', '\\');

	            _imgProductGroup = new GeneratedImage();
	            _imgProductGroup.ImageHandlerUrl = "~/dnnImageHandler.ashx";
	            imageWidth = imageWidths.Dequeue();
                if (imageWidth > 0)
	                _imgProductGroup.Parameters.Add(new ImageParameter() {Name = "w", Value = imageWidth.ToString()});
	            _imgProductGroup.Parameters.Add(new ImageParameter() {Name = "file", Value = fileName});
				_imgProductGroup.Parameters.Add(new ImageParameter() { Name = "mode", Value = "file" });
				// TODO: Watermark
				//if (false)
				//{
				//    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkText", Value = Localization.GetString("Sold.Text", this.LocalResourceFile) });
				//    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontFamily", Value = "Verdana" });
				//    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontColor", Value = "Red" });
				//    imgProduct.Parameters.Add(new ImageParameter() { Name = "WatermarkFontSize", Value = "20" });
				//}
				phimgProductGroup.Controls.Add(_imgProductGroup);

                imageCnt++;
                phimgProductGroup = FindControlRecursive(ctrl, "phimgProduct" + imageCnt.ToString()) as PlaceHolder;
            }

	        _imgIcon = FindControlRecursive(ctrl, "imgIcon") as Image;
	        if (_imgIcon != null)
	            _imgIcon.ImageUrl = productGroup.Icon;
	        _lblProductCount = FindControlRecursive(ctrl, "lblProductCount") as Label;
	        if (_lblProductCount != null)
	            _lblProductCount.Text = productGroup.ProductCount.ToString();

	        for (int i = 1; i < linkCnt + 1; i++)
	        {
	            Literal ltrLink = FindControlRecursive(ctrl, "ltrLink" + i.ToString()) as Literal;
	            if (ltrLink != null)
	            {
	                if (productGroup.ProductListTabId != -1)
	                    ltrLink.Text = Globals.NavigateURL(productGroup.ProductListTabId, "", "productgroup=" + productGroup.ProductGroupId.ToString());
	                else
	                {
	                    int dynamicTab = Convert.ToInt32(Settings["DynamicPage"] ?? TabId.ToString());
	                    ltrLink.Text = Globals.NavigateURL(dynamicTab, "", "productgroup=" + productGroup.ProductGroupId.ToString());
	                }
	            }
	        }
	        return ctrl;
	    }

	    protected void treeProductGroup_TreeNodePopulate(object sender, TreeNodeEventArgs e)
		{
			// http://quickstarts.asp.net/QuickStartv20/aspnet/doc/ctrlref/navigation/treeview.aspx
			TreeNode parent = e.Node;

			if (ShowLevels > 0)
			{
				string[] valPath = parent.ValuePath.Split('/');
				if (valPath.Length >= ShowLevels)
					return;
			}
			ProductGroupId = Convert.ToInt32(parent.Value.Substring(1));

			String CurrentLanguage = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
			List<ProductGroupInfo> products = Controller.GetProductSubGroupsByNode(PortalId, CurrentLanguage, ProductGroupId, ShowProductCount, IncludeChilds, IncludeDisabled);
			foreach (ProductGroupInfo p in products)
			{
				string nodeName = p.ProductGroupName +
					(ShowProductCount && p.ProductCount > 0 ? " (" + p.ProductCount + ")" : "");
				TreeNode newNode = new TreeNode(nodeName, "_" + p.ProductGroupId.ToString());
				newNode.SelectAction = TreeNodeSelectAction.SelectExpand;
				newNode.PopulateOnDemand = true;
				if (ShowIcons)
					newNode.ImageUrl = BBStoreHelper.FileNameToImgSrc(p.Icon,PortalSettings);

				parent.ChildNodes.Add(newNode);
			}
		}
		protected void lstProductGroups_SelectedIndexChanging(object sender, ListViewSelectEventArgs e)
		{
			ProductGroupId = (int)lstProductGroups.DataKeys[e.NewSelectedIndex].Value;

            SetFilter(ProductGroupId, IncludeChilds);

			ProductGroupInfo pgi = Controller.GetProductGroup(PortalId, CurrentLanguage, ProductGroupId);
			if (pgi != null && pgi.ProductListTabId != -1)
				Response.Redirect(Globals.NavigateURL(pgi.ProductListTabId, "", "productgroup=" + ProductGroupId.ToString()));
			else
			{
				int dynamicTab = Convert.ToInt32(Settings["DynamicPage"] ?? TabId.ToString());
				Response.Redirect(Globals.NavigateURL(dynamicTab, "", "productgroup=" + ProductGroupId.ToString()));
			}
		}

		protected void lstProductGroupsBS3_SelectedIndexChanging(object sender, ListViewSelectEventArgs e)
		{
			ProductGroupId = (int)lstProductGroupsBS3.DataKeys[e.NewSelectedIndex].Value;

			SetFilter(ProductGroupId, IncludeChilds);

			ProductGroupInfo pgi = Controller.GetProductGroup(PortalId, CurrentLanguage, ProductGroupId);
			if (pgi != null && pgi.ProductListTabId != -1)
				Response.Redirect(Globals.NavigateURL(pgi.ProductListTabId, "", "productgroup=" + ProductGroupId.ToString()));
			else
			{
				int dynamicTab = Convert.ToInt32(Settings["DynamicPage"] ?? TabId.ToString());
				Response.Redirect(Globals.NavigateURL(dynamicTab, "", "productgroup=" + ProductGroupId.ToString()));
			}
		}
		protected void treeProductGroup_SelectedNodeChanged(object sender, EventArgs e)
		{
			ProductGroupId = Convert.ToInt32(treeProductGroup.SelectedNode.Value.Substring(1));

			SetFilter(ProductGroupId, IncludeChilds);
			
			ProductGroupInfo pgi = Controller.GetProductGroup(PortalId, CurrentLanguage, ProductGroupId);
			if (pgi != null && pgi.ProductListTabId != -1)
				Response.Redirect(Globals.NavigateURL(pgi.ProductListTabId, "", "productgroup=" + ProductGroupId.ToString()));
			else
			{
				int dynamicTab = Convert.ToInt32(Settings["DynamicPage"] ?? TabId.ToString());
				Response.Redirect(Globals.NavigateURL(dynamicTab, "", "productgroup=" + ProductGroupId.ToString()));
			}
		}
		protected void ddl_SelectedIndexChanged(object sender, EventArgs e)
		{
			DropDownList ddl = sender as DropDownList;
			ProductGroupId = Convert.ToInt32(ddl.SelectedValue);

			int dynamicTab = Convert.ToInt32(Settings["DynamicPage"] ?? TabId.ToString());

			if (ProductGroupId == -1)
			{
				Controller.DeleteProductFilter(PortalId, FilterSessionId, "ProductGroup");
				Response.Redirect(Globals.NavigateURL(dynamicTab));
			}

			SetFilter(ProductGroupId, IncludeChilds);

			ProductGroupInfo pgi = Controller.GetProductGroup(PortalId, CurrentLanguage, ProductGroupId);
			if (pgi != null && pgi.ProductListTabId != -1)
				Response.Redirect(Globals.NavigateURL(pgi.ProductListTabId, "", "productgroup=" + ProductGroupId.ToString()));
			else
			{
				Response.Redirect(Globals.NavigateURL(dynamicTab, "", "productgroup=" + ProductGroupId.ToString()));
			}
		}

		

		#endregion

		#region Helper Methods
		private void SetFilter(int productGroupId, bool includeChilds)
		{
			Controller.DeleteProductFilter(PortalId, FilterSessionId, "ProductGroup");
			ProductFilterInfo fi = new ProductFilterInfo();
			fi.FilterSessionId = FilterSessionId;
			fi.FilterSource = "ProductGroup";
			fi.FilterValue = productGroupId.ToString()+"|"+includeChilds.ToString();
			fi.PortalId = PortalId;
			Controller.NewProductFilter(fi);
		}

		private Control FindControlRecursive(Control rootControl, string controlID)
		{
			if (rootControl.ID == controlID)
				return rootControl;

			foreach (Control controlToSearch in rootControl.Controls)
			{
				Control controlToReturn = FindControlRecursive(controlToSearch, controlID);
				if (controlToReturn != null)
					return controlToReturn;
			}
			return null;
		}
		protected TreeNode FindSelectedNode(TreeNodeCollection nodes, int selectedNodeId)
		{
			if (nodes != null)
			{
				foreach (TreeNode node in nodes)
				{
					node.Expand();
					if (node.Value == "_" + selectedNodeId.ToString())
					{
						return node;
					}
					TreeNode childNode = FindSelectedNode(node.ChildNodes, selectedNodeId);
					if (childNode != null)
					{
						return childNode;
					}
				}
			}
			return null;
		}

		protected DropDownList ProductGroupCombo(int productGroupId, int Value, int Level)
		{
			if (ShowLevels > 0 && Level >= ShowLevels)
				return null;

			List<ProductGroupInfo> pgs = Controller.GetProductSubGroupsByNode(PortalId, CurrentLanguage, productGroupId, ShowProductCount, IncludeChilds, IncludeDisabled);
			if (pgs.Count > 0)
			{
				DropDownList ddl = new DropDownList();
				ddl.DataTextField = "Key";
				ddl.DataValueField = "Value";
				ddl.ID = "cboLevel" + Level.ToString();
				ddl.Style.Add("width", "100%");
				string selText = Localization.GetString(IncludeChilds ? "ShowAll.Text" : "Select.Text", this.LocalResourceFile);
				
				if (Level == 0)
					ddl.Items.Add(new ListItem(selText, "-1"));
				else
					ddl.Items.Add(new ListItem(selText, productGroupId.ToString()));
			
				foreach (ProductGroupInfo pg in pgs)
				{
					string itemText;
					if (ShowProductCount && pg.ProductCount > 0)
						itemText = String.Format("{0} ({1})", pg.ProductGroupName, pg.ProductCount);
					else
						itemText = pg.ProductGroupName;
					ListItem li = new ListItem(itemText, pg.ProductGroupId.ToString());
					ddl.Items.Add(li);
				}

				ddl.SelectedValue = Value.ToString();
				ddl.AutoPostBack = true;
				ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
				return ddl;
			}
			else
				return null;
		}

		protected string GetCols()
		{
			int gic = lstProductGroupsBS3.GroupItemCount;

			if (gic <= 1)
				return "12";
			else if (gic == 2)
				return "6";
			else if (gic == 3)
				return "4";
			else if (gic == 4)
				return "3";
			else if (gic >= 5 && gic <= 11)
				return "2";
			return "1";
		}
		#endregion


	}
}