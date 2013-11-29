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
        protected string _values;
        protected ViewMode _displayMode = ViewMode.View;
        protected decimal _cost;
        protected decimal _costPercent;
        protected string _title;
        protected int _paymentProviderId;
        protected decimal _taxPercent = 19.0m;
		protected bool _showNetPrice = false;
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
            get { return _paymentProviderId; }
            set { _paymentProviderId = value; }
        }
        public virtual string Values
        {
            get { return _values; }
            set { _values = value; }
        }
		public virtual bool IsValid
		{
			get { return true; }
		}
        public ViewMode DisplayMode
        {
            get { return _displayMode; }
            set { _displayMode = value; }
        }
        public virtual decimal Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }
        public virtual decimal CostPercent
        {
            get { return _costPercent; }
            set { _costPercent = value; }
        }
        public virtual decimal TaxPercent
        {
            get { return _taxPercent; }
            set { _taxPercent = value; }
        }
        public virtual string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public virtual bool ShowNetprice
		{
			get { return _showNetPrice; }
			set { _showNetPrice = value; }
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

