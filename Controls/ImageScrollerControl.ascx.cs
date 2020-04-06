using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using DotNetNuke.Entities.Modules;
using Telerik.Web.UI;
using DotNetNuke.Services.GeneratedImage;


namespace Bitboxx.DNNModules.BBStore
{
	public partial class ImageScrollerControl : PortalModuleBase
	{
		private string _ImageDirectory = "";
		private int _ImageWidth = 100;
		private int _ImageCount = 3;
		private GeneratedImage _ImageControl = null;
		private string _ImageBackColor = "#F0F0F0";

		public string ImageDirectory
		{
			get { return _ImageDirectory; }
			set { _ImageDirectory = value; }
		}
		public int ImageWidth
		{
			get { return _ImageWidth; }
			set { _ImageWidth = value; }
		}
		public int ImageCount
		{
			get { return _ImageCount; }
			set { _ImageCount = value; }
		}		
		public GeneratedImage ImageControl
		{
			get { return _ImageControl; }
			set { _ImageControl = value; }
		}
		public string ImageBackColor
		{
			get { return _ImageBackColor; }
			set { _ImageBackColor = value; }
		}
		

		protected void Page_Load(object sender, EventArgs e)
		{
			// if (!IsPostBack)
			{
				string home = this.PortalSettings.HomeDirectoryMapPath;
				if (Directory.Exists(home + ImageDirectory))
				{
					string[] fi = Directory.GetFiles(home + ImageDirectory);
					if (fi.Length > 0)
					{
						imgDetail.ImageUrl = Page.ResolveUrl("~\\dnnImagehandler.ashx") + "?mode=file&w=" + ((ImageWidth + 10) * ImageCount - 10).ToString() + "&file=" + HttpUtility.UrlEncode(fi[0].Trim());
						if (fi.Length > 1)
						{
							for (int i = 0; i < fi.Length; i++)
							{
								fi[i] = Page.ResolveUrl("~\\dnnImagehandler.ashx") + "?mode=file&w=" + ImageWidth.ToString() + "&backcolor=" + HttpUtility.UrlEncode(ImageBackColor) + "&resizemode=fitsquare&file=" + HttpUtility.UrlEncode(fi[i].Trim());
							}
							RadRotator1.ItemWidth = new Unit(ImageWidth + 10);
							RadRotator1.Width = new Unit((ImageWidth + 10) * ImageCount - 10);
							RadRotator1.ItemHeight = new Unit(ImageWidth);
							RadRotator1.Height = RadRotator1.ItemHeight;
							RadRotator1.ItemClick += new RadRotatorEventHandler(RadRotator1_ItemClick);
							RadRotator1.DataSource = fi;
							RadRotator1.DataBind();
							RadRotator1.Style.Add("cursor", "hand");
						}
						else
							RadRotator1.Visible = false;
					}
					else
						imgDetail.Visible = false;
				}
				else
				{
					RadRotator1.Visible = false;
					imgDetail.Visible = false;
				}
			}
			if (_ImageControl != null)
				imgDetail.Visible = false;
		}

		void RadRotator1_ItemClick(object sender, RadRotatorEventArgs e)
		{
			string home = this.PortalSettings.HomeDirectoryMapPath;
			string[] fi = Directory.GetFiles(home + ImageDirectory);
			
			if (fi.Length > 0)
			{
				string imageFile = fi[e.Item.Index];
				if (_ImageControl != null)
				{
					foreach (ImageParameter para in _ImageControl.Parameters)
					{
						if (para.Name == "File")
						{
							_ImageControl.Parameters.Remove(para);
							_ImageControl.Parameters.Add(new ImageParameter() { Name = "file", Value = imageFile.Trim() });
							break;
						}
					}
				}
				else
					imgDetail.ImageUrl = Page.ResolveUrl("~\\dnnImagehandler.ashx") + "?mode=file&w=" + ((ImageWidth + 10) * ImageCount - 10).ToString() + "&file=" + HttpUtility.UrlEncode(imageFile.Trim());
			}

		}
	}
}