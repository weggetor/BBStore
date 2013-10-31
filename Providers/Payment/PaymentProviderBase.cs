using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBStore.Providers.Payment
{
    public enum ViewMode
    {
        View,
        Edit,
        Summary
    }

    public class PaymentProviderBase: PortalModuleBase
    {
  		#region Protected Declarations
		protected PortalModuleBase parentControl;
        protected string _properties = "";
        protected string values;
        protected ViewMode displayMode = ViewMode.View;
        protected decimal cost;
        protected string title;
        protected int paymentProviderId;
        protected decimal taxPercent = 19.0m;
		protected bool showNetPrice = false;
		#endregion

		#region Public Properties
		public PortalModuleBase ParentControl
		{
			get {return parentControl;}
			set {parentControl = value;}
		}
        public virtual string Properties
        {
            get {return _properties;}
            set {_properties = value;}
        }
        public int PaymentProviderId
        {
            get { return paymentProviderId; }
            set { paymentProviderId = value; }
        }
        public virtual string Values
        {
            get { return values; }
            set { values = value; }
        }
		public virtual bool IsValid
		{
			get { return true; }
		}
        public ViewMode DisplayMode
        {
            get { return displayMode; }
            set { displayMode = value; }
        }
        public virtual decimal Cost
        {
            get { return cost; }
            set { cost = value; }
        }
        public virtual decimal TaxPercent
        {
            get { return taxPercent; }
            set { taxPercent = value; }
        }
        public virtual string Title
        {
            get { return title; }
            set { title = value; }
        }
        public virtual bool ShowNetprice
		{
			get { return showNetPrice; }
			set { showNetPrice = value; }
		}
 		#endregion

		#region Constructors
		public PaymentProviderBase()
		{
		}
		#endregion

		#region Events
        public event EventHandler EditComplete;
		protected void OnEditComplete()
		{
			if (EditComplete != null)
			{
				EditComplete(this, null);
			}
		}
		#endregion
		
		#region PortalModuleBase Overrides
		protected override void OnLoad(EventArgs e)
		{
            this.LocalResourceFile = Localization.GetResourceFile(this, this.GetType().BaseType.Name + ".ascx");
			base.OnLoad(e);
		}
		#endregion
	}
}

