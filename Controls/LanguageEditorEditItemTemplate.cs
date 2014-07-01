using System;
using System.Reflection;
using System.Web.UI;

namespace Bitboxx.DNNModules.BBStore
{
    public class LanguageEditorTemplate:ITemplate
    {
        private string _control = "";
        private UserControl _parentControl;
        private LanguageEditorAttribute _fixedDisplay;
        public LanguageEditorTemplate(Type T, LanguageEditorAttribute fixedDisplay, UserControl parentControl)
        {
            _parentControl = parentControl;
            _fixedDisplay = fixedDisplay;
            _control = "<%@ Register TagPrefix=\"dnn\" TagName=\"Label\" Src=\"~/controls/LabelControl.ascx\" %>\r\n";
            _control += "<%@ Register TagPrefix=\"dnn\" TagName=\"TextEditor\" Src=\"~/controls/TextEditor.ascx\" %>\r\n";
            foreach (PropertyInfo pi in T.GetProperties())
            {
                LanguageEditorAttribute att = GetAttribute(pi);
                if (att != null)
                {
                    string labelName = att.Label == String.Empty ? pi.Name : att.Label;
                    switch (att.Control.ToLower())
                    {
                        case "textbox":
                            _control += "<div class=\"dnnFormItem\">\r\n" +
                                        "   <dnn:Label ID=\"lbl" + labelName + "\" runat=\"server\" ControlName=\"txt" + pi.Name + "\" Suffix=\":\" ></dnn:Label>\r\n" +
                                        "   <asp:TextBox ID=\"txt" + pi.Name + "\" runat=\"server\" CssClass=\"bbstore-template\" Width=\"" + att.Width + "\" MaxLength=\"" + att.MaxLength.ToString() + "\" " + (att.Rows > 1 ? "TextMode=\"MultiLine\"" : "") + " Rows = \"" + att.Rows.ToString() + "\" Text='<%# Bind(\"" + pi.Name + "\") %>' />\r\n" +
                                        "</div>";
                            break;
                        case "texteditor":
                            _control += "<div class=\"dnnFormItem\">\r\n" +
                                        "   <dnn:Label ID=\"lbl" + labelName + "\" runat=\"server\" ControlName=\"txt" + pi.Name + "\" Suffix=\":\" ></dnn:Label>\r\n" +
                                        "   <div style=\"float:right; width:70%\">\r\n" +
                                        "       <dnn:TextEditor ID=\"edt" + pi.Name + "\" runat=\"server\" Width=\"" + att.Width + "\" Height=\"" + att.Height + "\"  TextRenderMode=\"Raw\" HtmlEncode=\"False\" defaultmode=\"Rich\"  choosemode=\"False\" Text='<%# Bind(\"" + pi.Name + "\") %>' />\r\n" +
                                        "   </div>\r\n"+
                                        "</div>\r\n";
                            break;
                        
                    }
                }
            }
        }

        #region ITemplate Members

        public void InstantiateIn(Control container)
        {
            Page pg = new Page();
            Control ctrl = _parentControl.ParseControl(_control);
            container.Controls.Add(ctrl);
        }

        #endregion

        protected LanguageEditorAttribute GetAttribute(PropertyInfo pi)
        {
            object[] attrs = pi.GetCustomAttributes(true);
            foreach (object attr in attrs)
            {
                LanguageEditorAttribute languageEditorAttribute = attr as LanguageEditorAttribute;
                if (languageEditorAttribute != null)
                {
                    if (_fixedDisplay != null)
                        return _fixedDisplay;
                    return languageEditorAttribute;
                }
            }
            return null;
        }
    }


}