using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.UserControls;

namespace Bitboxx.DNNModules.BBStore
{
    public partial class LanguageEditorControl : UserControl
    {
        #region private fields

        private Type _internalType;
        private LanguageEditorAttribute _fixedDisplay;

        #endregion

        #region public properties and methods

        public string InternalType
        {
            set { _internalType = Type.GetType(value); }
        }

        public string FixedDisplay
        {
            set
            {
                string[] props = value.Split(',');
                _fixedDisplay = new LanguageEditorAttribute("");
                foreach (string prop in props)
                {
                    string[] vals = prop.Split('=');
                    string key = vals[0].Trim();
                    string val = vals[1].Trim();
                    switch (key.ToUpper())
                    {
                        case "CONTROL":
                            _fixedDisplay.Control = val;
                            break;
                        case "HEIGHT":
                            _fixedDisplay.Height = val;
                            break;
                        case "WIDTH":
                            _fixedDisplay.Width = val;
                            break;
                        case "MAXLENGTH":
                            _fixedDisplay.MaxLength = Convert.ToInt32(val);
                            break;
                        case "ROWS":
                            _fixedDisplay.Rows = Convert.ToInt32(val);
                            break;
                        case "LABEL":
                            _fixedDisplay.Label = val.Trim();
                            break;
                    }

                }
            }
        }
        public List<ILanguageEditorInfo> Langs
        {
            get
            {
                if (ViewState["Langs"] != null)
                    return (List<ILanguageEditorInfo>) ViewState["Langs"];
                else
                    return new List<ILanguageEditorInfo>();
            }
            set
            {
                List<ILanguageEditorInfo> pgLangs = new List<ILanguageEditorInfo>();
                LocaleController lc = new LocaleController();
                Dictionary<string, Locale> loc = lc.GetLocales(PortalSettings.Current.PortalId);

                // Lets build the Languages Collection
                foreach (KeyValuePair<string, Locale> item in loc)
                {
                    ILanguageEditorInfo fg = (from l in value where l.Language == item.Key select l).FirstOrDefault();
                    if (fg != null)
                        pgLangs.Add(fg);
                    else
                    {
                        var li = (ILanguageEditorInfo) Activator.CreateInstance(_internalType);
                        li.Language = item.Key;
                        pgLangs.Add(li);
                    }
                }
                ViewState["Langs"] = pgLangs;
                short defLangId = 0;
                foreach (ILanguageEditorInfo li in Langs)
                {
                    if (li.Language == System.Threading.Thread.CurrentThread.CurrentCulture.Name)
                    {
                        formViewLang.PageIndex = defLangId;
                        break;
                    }
                    defLangId++;
                }
            }
        }

        public void UpdateLangs()
        {
            if (Langs.Count > formViewLang.PageIndex)
            {
                foreach (PropertyInfo pi in _internalType.GetProperties())
                {
                    LanguageEditorAttribute att = GetAttribute(pi);
                    if (att != null)
                    {
                        string text = "";

                        switch (att.Control.ToLower())
                        {
                            case "textbox":
                                TextBox txt = formViewLang.FindControl("txt" + pi.Name) as TextBox;
                                if (txt != null)
                                    text = txt.Text.Trim();
                                break;
                            case "texteditor":
                                TextEditor edt = formViewLang.FindControl("edt" + pi.Name) as TextEditor;
                                if (edt != null)
                                    text = edt.Text;
                                break;
                        }
                        for (int i = 0; i < Langs.Count; i++)
                        {
                            ILanguageEditorInfo fg = Langs[i];
                            PropertyInfo propertyInfo = fg.GetType().GetProperty(pi.Name);

                            if ((string) propertyInfo.GetValue(fg, null) == String.Empty || i == formViewLang.PageIndex)
                            {
                                if (i == formViewLang.PageIndex)
                                    propertyInfo.SetValue(fg, text, null);
                                else
                                {
                                    string newValue = "###" + fg.Language + " " + text;
                                    int maxLength = GetAttribute(propertyInfo).MaxLength;
                                    if (newValue.Length > maxLength)
                                        newValue = newValue.Substring(0, maxLength);
                                    propertyInfo.SetValue(fg,newValue, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Eventhandlers

        protected void Page_Init(object sender, EventArgs e)
        {
            formViewLang.EditItemTemplate = new LanguageEditorTemplate(_internalType, _fixedDisplay, this);
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            formViewLang.DataSource = Langs;
            formViewLang.DataBind();
        }

        protected void formViewLang_ItemCreated(object sender, EventArgs e)
        {
            LocaleController lc = new LocaleController();
            Dictionary<string, Locale> loc = lc.GetLocales(PortalSettings.Current.PortalId);

            FormViewRow row = formViewLang.HeaderRow;
            if (row != null)
            {
                PlaceHolder phLanguage = row.FindControl("phLanguage") as PlaceHolder;
                int i = 0;
                foreach (KeyValuePair<string, Locale> item in loc)
                {
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.Style.Add("display", "table-cell");

                    ImageButton imgBtn = new ImageButton();
                    imgBtn.ID = "imgLanguage" + i.ToString();
                    imgBtn.Click += Language_Selected;
                    imgBtn.ImageUrl = "~\\images\\Flags\\" + item.Key + ".gif";
                    imgBtn.Style.Add("padding", "3px 10px 5px 10px");

                    imgBtn.Style.Add("border-width", "1px");
                    imgBtn.Style.Add("border-color", "#000000");

                    if (i == formViewLang.PageIndex)
                    {
                        imgBtn.Style.Add("border-style", "solid solid hidden solid");
                        imgBtn.Style.Add("margin", "0 0 1px 0;");
                        imgBtn.Style.Add("background-color", "White");
                    }
                    else
                    {
                        imgBtn.Style.Add("border-style", "solid solid solid solid");
                        imgBtn.Style.Add("margin", "0");
                        imgBtn.Style.Add("background-color", "LightGrey");
                    }
                    div.Controls.Add(imgBtn);
                    phLanguage.Controls.Add(div);
                    i++;
                }
            }
        }

        protected void Language_Selected(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            foreach (PropertyInfo pi in _internalType.GetProperties())
            {
                LanguageEditorAttribute att = GetAttribute(pi);
                if (att != null)
                {
                    string text = "";
                    switch (att.Control.ToLower())
                    {
                        case "textbox":
                            TextBox txt = formViewLang.FindControl("txt" + pi.Name) as TextBox;
                            if (txt != null)
                                text = txt.Text.Trim();
                            break;
                        case "texteditor":
                            TextEditor edt = formViewLang.FindControl("edt" + pi.Name) as TextEditor;
                            if (edt != null)
                                text = edt.Text;
                            break;
                    }
                    ILanguageEditorInfo fg = Langs[formViewLang.PageIndex];
                    PropertyInfo propertyInfo = fg.GetType().GetProperty(pi.Name);
                    propertyInfo.SetValue(fg, text, null);
                }
            }

            ImageButton btn = sender as ImageButton;
            string url = btn.ImageUrl;
            string language = url.Substring(url.LastIndexOf('\\') + 1);
            language = language.Replace(".gif", "");
            int i = 0;
            foreach (ILanguageEditorInfo lang in Langs)
            {
                if (lang.Language == language)
                {
                    formViewLang.PageIndex = i;
                    break;
                }
                i++;
            }
        }

        #endregion

        #region "Helper methods"

        protected LanguageEditorAttribute GetAttribute(PropertyInfo pi)
        {
            object[] attrs = pi.GetCustomAttributes(true);
            foreach (object attr in attrs)
            {
                LanguageEditorAttribute languageEditorAttribute = attr as LanguageEditorAttribute;
                if (languageEditorAttribute != null)
                {
                    return languageEditorAttribute;
                }
            }
            return null;
        }

        #endregion
    }
}