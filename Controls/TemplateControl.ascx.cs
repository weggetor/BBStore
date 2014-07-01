#region copyright

// bitboxx - http://www.bitboxx.net
// Copyright (c) 2012 
// by bitboxx solutions Torsten Weggen
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

#endregion


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;

namespace Bitboxx.DNNModules.BBStore
{
	public partial class TemplateControl : PortalModuleBase
	{
	    protected DotNetNuke.UI.UserControls.TextEditor edtTemplate;
        
        private string _cssClass = "Normal";
		private int _width = 150;
		private string _key;
		private bool _isSuperUser;
        private EditorControlEnum _editorControl = EditorControlEnum.TextBox;

        public enum TemplateEnum
        {
            Default,
            Neutral,
            Language,
            Portal,
            PortalLanguage,
        }

        public enum ViewModeEnum
        {
            View,
            New,
            Edit,
        }

        public enum EditorControlEnum
        {
            TextBox,
            TextEditor,
        }

        public Func<string,string> CreateImageCallback { get; set; }
        public string Value
		{
			get
			{
				if (ViewState["Value"] != null && ddlTemplate.Items.FindByText((string)ViewState["Value"]) != null )
				{
                    return (string)ViewState["Value"];
				}
				return "Default";
			}
			set
			{
			    if (ddlTemplate.Items.FindByText(value) == null)
			    {
                    ViewState["Value"] = "Default";
			    }
			    else
			    {
                    ViewState["Value"] = value;
                    ddlTemplate.SelectedValue = value;    
			    }
			    lblTemplateName.Text = (string)ViewState["Value"];
                ShowThumb();
			}
		}
		public string CssClass
		{
			get { return _cssClass; }
			set
			{
				_cssClass = value;
				pnlView.CssClass = value;
			}
		}
		public int Width
		{
			get { return _width; }
			set 
			{ 
				_width = value;
				pnlView.Width = value;
				imgThumb.Width = new System.Web.UI.WebControls.Unit(_width);
                ddlTemplate.Width = new System.Web.UI.WebControls.Unit(_width);
			}
		}
		public ViewModeEnum ViewMode
		{
			get
			{
				if (ViewState["ViewMode"] != null)
					return (ViewModeEnum) Enum.Parse(typeof(ViewModeEnum),(string)ViewState["ViewMode"]);
				return ViewModeEnum.View;
			}
			set
			{
				ViewState["ViewMode"] = value.ToString();
				pnlEdit.Visible = (value != ViewModeEnum.View);
                pnlHelp.Visible = (value != ViewModeEnum.View);
				pnlView.Visible = (value == ViewModeEnum.View);
				pnlNewTemplate.Visible = (value == ViewModeEnum.New);
				pnlEditTemplate.Visible = (value == ViewModeEnum.Edit);
			}
		}
		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}
        public string CaptionText { get; set;}
        public string CaptionHelp { get; set; } 
		
	    public EditorControlEnum EditorControl
	    {
            get { return _editorControl; }
            set
	        {
	            _editorControl = value;
	            switch (value)
	            {
	                case EditorControlEnum.TextBox:
	                    txtTemplate.Visible = true;
	                    edtTemplate.Visible = false;
	                    break;
	                case EditorControlEnum.TextEditor:
	                    txtTemplate.Visible = false;
	                    edtTemplate.Visible = true;
	                    break;
	            }
	        }
	    }

		private string TemplatePath
		{
			get
			{
				string controlPath = MapPath(AppRelativeVirtualPath);
				FileInfo fi = new FileInfo(controlPath);
				string templatePath = fi.DirectoryName + @"\..\Templates\" + Key +"\\";
				fi = new FileInfo(templatePath);
				return fi.DirectoryName + "\\";
			}
		}

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ReadTemplateList();

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
            ltrHelp.Text = GetHelpText();

            string fallbackLanguage = PortalSettings.DefaultLanguage;

			if (!IsPostBack)
			{
				UserInfo objUser = UserController.GetCurrentUserInfo();
				_isSuperUser = (objUser != null && objUser.IsSuperUser);
				rblMode.SelectedValue = (_isSuperUser ? "0" : "1");

				Localization.LoadCultureDropDownList(ddlLanguage, CultureDropDownTypes.NativeName, ((PageBase)Page).PageCulture.Name);
				ddlLanguage.Visible = (ddlLanguage.Items.Count > 1);
				ddlLanguage.SelectedValue = fallbackLanguage;

				LoadTemplateFile();
                cmdEdit.Visible = (ddlTemplate.SelectedIndex > 0);
			}
		}

        protected void Page_Prerender(object sender, EventArgs e)
        {
            Label lblLabel = lblCaption.FindControl("lblLabel") as Label;
            if (lblLabel != null)
            {
                if (!String.IsNullOrEmpty(CaptionText))
                    lblLabel.Text = CaptionText;
                else
                    lblLabel.Text = LocalizeString("lblTemplate.Text");
            }

            Label lblHelp = lblCaption.FindControl("lblHelp") as Label;
            if (lblHelp != null)
            {
                if (!String.IsNullOrEmpty(CaptionHelp))
                    lblHelp.Text = CaptionHelp;
                else
                    lblHelp.Text = LocalizeString("lblTemplate.Help");
            }

        }

		protected void ddlTemplate_SelectedIndexChanged(object sender, EventArgs e)
		{
			Value = ddlTemplate.SelectedValue;
		    cmdEdit.Visible = ddlTemplate.SelectedIndex > 0;
		}
		protected void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadTemplateFile();
		}
		protected void rblMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadTemplateFile();
		}
		protected void cmdNew_Click(object sender, EventArgs e)
		{
			ViewMode = ViewModeEnum.New;
		    if (EditorControl == EditorControlEnum.TextBox)
		        txtTemplate.Text = GetTemplate("Default");
		    else
		        edtTemplate.Text = GetTemplate("Default");

			txtName.Text = "";
			lblFile.Text = "";
		}

		protected void cmdEdit_Click(object sender, EventArgs e)
		{
			LoadTemplateFile();
			ViewMode = ViewModeEnum.Edit;
		}

		protected void cmdSave_Click(object sender, EventArgs e)
		{
			SaveTemplateFile();
			ReadTemplateList();
			if (ViewMode == ViewModeEnum.New)
			{
				ddlTemplate.SelectedValue = txtName.Text.Trim();
				ddlTemplate_SelectedIndexChanged(ddlTemplate, new EventArgs());
			}
			else
			{
				if (ddlTemplate.Items.FindByValue(Value)!= null)
				{
					ddlTemplate.SelectedValue = Value;
					ddlTemplate_SelectedIndexChanged(ddlTemplate, new EventArgs());
				}
			}
			ViewMode = ViewModeEnum.View;
		}

		protected void cmdCancelEdit_Click(object sender, EventArgs e)
		{
			ViewMode = ViewModeEnum.View;
		}

		private void ReadTemplateList()
		{
			string[] files = Directory.GetFiles(TemplatePath, "*.htm");

			// We assemble all the filenames without language codes, portal infos and file extensions
			// and filter out all duplicates

            ddlTemplate.Items.Add(new ListItem("(Select template)", "default"));
            List<string> lst = new List<string>();
			foreach (string file in files)
			{
				FileInfo fi = new FileInfo(file);
				string normFile = fi.Name.Substring(0, fi.Name.IndexOf('.'));
			    if (!lst.Contains(normFile) && normFile.ToLower() != "default")
			    {
			        ddlTemplate.Items.Add(new ListItem(normFile,normFile));
                    lst.Add(normFile);
			    }
			}

			if (ddlTemplate.Items.FindByValue(Value) != null)
			{
			    ddlTemplate.SelectedValue = Value;
			}
			else
			{
			    ddlTemplate.SelectedIndex = 0;
			}
            //if (lst.Count > 0)
            //{
            //    if (!lst.Contains(Value) && Value != "default")
            //    {
            //        Value = lst[0];
            //    }
            //    ddlTemplate.DataSource = lst;
            //    ddlTemplate.DataBind();
            //}
			
		    ddlTemplate.Visible = (lst.Count > 0);
		    lblTemplateName.Visible = (lst.Count <= 0);
			cmdEdit.Visible = (lst.Count > 0 && ddlTemplate.SelectedIndex > 0);
		}

		private void ShowThumb()
		{
			string imageFile = TemplatePath + Value + ".jpg";
			if (File.Exists(imageFile))
				imgThumb.ImageUrl = AppRelativeTemplateSourceDirectory + @"../Templates/" + Key + "/" + Value + ".jpg" + "?id=" + Guid.NewGuid().ToString(); // No caching !
			else
				imgThumb.ImageUrl = "";
		}
		private void LoadTemplateFile()
		{
			if (ViewMode == ViewModeEnum.New)
				return;
			string currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
			string fallbackLanguage = PortalSettings.DefaultLanguage;
			string templateFile = ddlTemplate.SelectedValue;
			string defaultFile = Path.Combine(TemplatePath, templateFile + ".htm");
			string defaultLangFile = Path.Combine(TemplatePath, templateFile + "." + ddlLanguage.SelectedValue + ".htm");

			if (ddlLanguage.SelectedValue != fallbackLanguage)
				templateFile += "." + ddlLanguage.SelectedValue;
			if (rblMode.SelectedValue == "1")
				templateFile += ".Portal-" + PortalSettings.PortalId;
			templateFile += ".htm";
			lblFile.Text = templateFile;
			templateFile = Path.Combine(TemplatePath, templateFile);

			if (File.Exists(templateFile))
				txtTemplate.Text = File.ReadAllText(templateFile);
			else if (File.Exists(defaultLangFile))
				txtTemplate.Text = File.ReadAllText(defaultLangFile);
			else if (File.Exists(defaultFile))
				txtTemplate.Text = File.ReadAllText(defaultFile);

		    edtTemplate.Text = txtTemplate.Text;
		}

		private void SaveTemplateFile()
		{
			string currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
			string fallbackLanguage = PortalSettings.DefaultLanguage;
			string templateFile;

			if (ViewMode == ViewModeEnum.New)
				templateFile = txtName.Text.Trim();
			else
				templateFile = ddlTemplate.SelectedValue;

			string thumbFile = Path.Combine(TemplatePath, templateFile + ".jpg");

			if (ddlLanguage.SelectedValue != fallbackLanguage)
				templateFile += "." + ddlLanguage.SelectedValue;

			if (rblMode.SelectedValue == "1")
				templateFile += ".Portal-" + PortalSettings.PortalId;

			templateFile += ".htm";
			lblFile.Text = templateFile;
			templateFile = Path.Combine(TemplatePath, templateFile);

			File.WriteAllText(templateFile, (_editorControl == EditorControlEnum.TextBox ? txtTemplate.Text : edtTemplate.Text));
		    CreateThumb(thumbFile);

			DotNetNuke.Common.Utilities.DataCache.ClearPortalCache(PortalSettings.PortalId, false);
		}

		private void CreateThumb(string thumbFile)
		{
            string html = "";
            if (CreateImageCallback != null)
		        html = CreateImageCallback(txtTemplate.Text);

		    if (html != string.Empty)
		    {
		        string baseUrl = "http://" + Request["HTTP_POST"] + "/";
                StringBuilder sb = new StringBuilder();
		        sb.AppendLine("<html><head>");
		        sb.AppendLine("<style type=\"text/css\">");
		        string cssFile = Server.MapPath("~/Portals/_default/default.css");
                if (File.Exists(cssFile))
                    sb.Append(File.ReadAllText(cssFile));
		        cssFile = Server.MapPath("~/DesktopModules/BBStore/module.css");
                if (File.Exists(cssFile))
                    sb.Append(File.ReadAllText(cssFile));
		        cssFile = Server.MapPath(PortalSettings.ActiveTab.SkinPath + "skin.css");
                if (File.Exists(cssFile))
                    sb.Append(File.ReadAllText(cssFile));
		        cssFile = Server.MapPath(PortalSettings.HomeDirectory + "portal.css");
                if (File.Exists(cssFile))
                    sb.Append(File.ReadAllText(cssFile));
                sb.AppendLine("</style>");
		        sb.AppendLine("</head>");
		        sb.AppendLine("<body>");
		        sb.AppendLine("<div style=\"min-width:600px;width=600px;\">");
		        sb.AppendLine(html);
		        sb.AppendLine("</div>");
                sb.AppendLine("</body></html>");

		        AutoResetEvent resultEvent = new AutoResetEvent(false);
		        IEBrowser browser = new IEBrowser(sb.ToString(), thumbFile, resultEvent);
		        EventWaitHandle.WaitAll(new AutoResetEvent[] {resultEvent});
		    }
		}

		public string GetTemplate(string name)
		{
			string currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

			string portalLangFile = Path.Combine(TemplatePath,name + "." + currentLanguage + ".Portal-" + PortalSettings.PortalId.ToString() + ".htm");
			string langFile = Path.Combine(TemplatePath, name + "." + currentLanguage + ".htm");
			string portalFile = Path.Combine(TemplatePath, name + ".Portal-" + PortalSettings.PortalId.ToString() + ".htm");
			string neutralFile = Path.Combine(TemplatePath, name + ".htm");
			string defaultFile = Path.Combine(TemplatePath, "default.htm");

			if (File.Exists(portalLangFile))
				return File.ReadAllText(portalLangFile);
			else if (File.Exists(langFile))
				return File.ReadAllText(langFile);
			else if (File.Exists(portalFile))
				return File.ReadAllText(portalFile);
			else if (File.Exists(neutralFile))
				return File.ReadAllText(neutralFile);
			else if (File.Exists(defaultFile))
				return File.ReadAllText(defaultFile);
			return "";
		}

		public void SaveTemplate(string template, string name, TemplateEnum templateEnum )
		{
            string currentLanguage = Thread.CurrentThread.CurrentUICulture.Name;

		    string file = "";
            switch (templateEnum)
		    {
		        case TemplateEnum.Default:
                    file = Path.Combine(TemplatePath, "default.htm");
		            break;
                case TemplateEnum.Neutral:
                    file = Path.Combine(TemplatePath, name + ".htm");
		            break;
                case TemplateEnum.Language:
                    file = Path.Combine(TemplatePath, name + "." + currentLanguage + ".htm");
		            break;
                case TemplateEnum.Portal:
                    file = Path.Combine(TemplatePath, name + ".Portal-" + PortalSettings.PortalId.ToString() + ".htm");
		            break;
                case TemplateEnum.PortalLanguage:
                    file = Path.Combine(TemplatePath, name + "." + currentLanguage + ".Portal-" + PortalSettings.PortalId.ToString() + ".htm");
		            break;
		    }
			FileInfo fi = new FileInfo(file);
            if (fi.Directory != null && !fi.Directory.Exists)
                fi.Directory.Create();
		    
		    File.WriteAllText(file, template);
		    string thumbFile = file.Replace(".htm", ".jpg");
		    CreateThumb(thumbFile);
		}

		private string GetHelpText()
		{
			string currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

			string langFile = Path.Combine(TemplatePath, "tokens." + currentLanguage + ".htp");
			string neutralFile = Path.Combine(TemplatePath, "tokens.htp");

			if (File.Exists(langFile))
				return File.ReadAllText(langFile);
			else if (File.Exists(neutralFile))
				return File.ReadAllText(neutralFile);
			return "";

		}
	}
}