using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.UI.Skins.Controls;

namespace Bitboxx.DNNModules.BBStore
{
	public partial class ViewCartLogin : PortalModuleBase
	{
		private string _userName = "";
		private BBStoreController _controller;
		private List<UserInfo> _allUsers; 
		
		public ViewCart MainControl { get; set; }
		public BBStoreController Controller
		{
			get
			{
				if (_controller == null)
					_controller = new BBStoreController();
				return _controller;
			}
		}

		protected List<UserInfo> AllUsers
		{
			get
			{
				if (_allUsers == null)
				{
					_allUsers = new List<UserInfo>();
					foreach (UserInfo user in UserController.GetUsers(false, false, -1))
					{
						_allUsers.Add(user);
					}
				}
				return _allUsers;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				divMessages.Attributes.Add("class", "dnnFormMessage dnnFormInfo");
				lblIntro.Text = Localization.GetString("Intro.Text", this.LocalResourceFile);
				pnlPassword.Visible = false;
				pnlCheckUser.Visible = true;
			}
		}

		protected void cmdCheckuser_Click(object sender, EventArgs e)
		{
			divMessages.Attributes.Add("class", "dnnFormMessage dnnFormInfo");
			lblIntro.Text = Localization.GetString("Intro.Text", this.LocalResourceFile);

			_userName = txtUserName.Text.Trim().ToLower();
			txtPassword.Text = "";
			if (String.IsNullOrEmpty(_userName))
			{
				pnlPassword.Visible = false;
				pnlCheckUser.Visible = true;
			}
			else
			{
				var thisUsers = from u in AllUsers where u.Username == _userName select u;
				UserInfo thisPortalUser = (from u in thisUsers where u.PortalID == PortalId select u).FirstOrDefault();

				// we have to handle different cases:
				// 1.) User does not exist in any portal (thisUsers.Any() == false) => Create with random password and login
				// 2.) User exists in this portal (thisPortalUser # null) => Ask for password and login
				// 3.) User exists only in another portal (thisUsers.Any() == true && thisPortalUser == null) => Ask for password, create and login

				if (thisUsers.Any() == false)
				{
					if (DotNetNuke.Services.Mail.Mail.IsValidEmailAddress(_userName, PortalId))
					{
						pnlUser.Visible = false;
						pnlCheckUser.Visible = false;
						pnlConfirmUser.Visible = true;
						divMessages.Attributes.Add("class", "dnnFormMessage dnnFormInfo");
						lblIntro.Text = Localization.GetString("Repeat.Text", this.LocalResourceFile);
						lblUserNameConfirm.Text = _userName;
					}
					else
					{
						divMessages.Attributes.Add("class", "dnnFormMessage dnnFormWarning");
						lblIntro.Text = Localization.GetString("WrongEmail.Text", this.LocalResourceFile);
						pnlPassword.Visible = false;
						pnlCheckUser.Visible = true;
					}
				}
				else 
				{
					divMessages.Attributes.Add("class", "dnnFormMessage dnnFormInfo");
					string loginUrl = Globals.NavigateURL(TabId, "", "ctl=Login");
					if (thisPortalUser == null)
						lblIntro.Text = Localization.GetString("OtherPortalKnownUser.Text", this.LocalResourceFile);
					else
						lblIntro.Text = String.Format(Localization.GetString("KnownUser.Text", this.LocalResourceFile),thisPortalUser.Username,loginUrl);
					pnlPassword.Visible = true;
					pnlCheckUser.Visible = false;
				}
			}
		}

		protected void cmdPassword_Click(object sender, EventArgs e)
		{
			_userName = txtUserName.Text.Trim().ToLower();
			var thisUsers = from u in AllUsers where u.Username == _userName select u;
			UserInfo thisPortalUser = (from u in thisUsers where u.PortalID == PortalId select u).FirstOrDefault();

			if (thisUsers.Any() == true && thisPortalUser == null)
			{
				// 3.) User exists only in another portal (thisUsers.Any() == true && thisPortalUser == null) => Ask for password, create and login
				UserInfo user = new UserInfo();
				user.Username = _userName;
				user.FirstName = "";
				user.LastName = "";
				user.PortalID = PortalId;
				user.Email = _userName;
				user.DisplayName = _userName;
				user.Membership.Password = txtPassword.Text.Trim();

				user.Profile.PreferredLocale = PortalSettings.DefaultLanguage;
				user.Profile.PreferredTimeZone = PortalSettings.TimeZone;
				user.Profile.FirstName = user.FirstName;
				user.Profile.LastName = user.LastName;

				UserCreateStatus status = MembershipProvider.Instance().CreateUser(ref user);

				if (status == UserCreateStatus.Success)
				{
					// Add User to Standard Roles
					RoleController roleController = new RoleController();
					RoleInfo role = new RoleInfo();

					ArrayList roles = roleController.GetPortalRoles(PortalId);
					for (int i = 0; i < roles.Count - 1; i++)
					{
						role = (RoleInfo)roles[i];
						if (role.AutoAssignment == true)
							roleController.AddUserRole(PortalId, user.UserID, role.RoleID, Null.NullDate, Null.NullDate);
					}
					// Log new user in and create a new customer +  add him to cart
					UserController.UserLogin(PortalId, user, PortalSettings.PortalName, Request.UserHostAddress, false);
					int customerId = Controller.NewCustomer(new CustomerInfo(user.UserID, PortalId, _userName));
					Controller.UpdateCartCustomerId(this.MainControl.CartId, customerId);
					Mail.SendMail(user, MessageType.UserRegistrationVerified, PortalSettings);
					Response.Redirect(Request.QueryString["returnUrl"]);
				}
                else
                {
                    divMessages.Attributes.Add("class", "dnnFormMessage dnnFormWarning");
                    string loginUrl = Globals.NavigateURL(TabId, "", "ctl=Login");
                    lblIntro.Text = String.Format(Localization.GetString("ErrorCreatingUser.Text", this.LocalResourceFile), status.ToString("G"));
                    txtUserName.Text = "";
                    pnlUser.Visible = true;
                    pnlCheckUser.Visible = true;
                    pnlPassword.Visible = false;
                    pnlConfirmUser.Visible = false;
                }
            }
			else
			{
				// 2.) User exists in this portal (thisPortalUser # null) => Ask for password and login
				UserLoginStatus loginStatus= UserLoginStatus.LOGIN_FAILURE;
				UserInfo user = UserController.ValidateUser(PortalId, txtUserName.Text, txtPassword.Text, "DNN", "",
				                                            PortalSettings.PortalName, Request.UserHostAddress, ref loginStatus);
			
				if (loginStatus == UserLoginStatus.LOGIN_SUCCESS || loginStatus == UserLoginStatus.LOGIN_SUPERUSER)
				{
					UserController.UserLogin(PortalId, user, PortalSettings.PortalName, Request.UserHostAddress, false);
					List<CustomerInfo> customers = Controller.GetCustomersByUserId(PortalId, user.UserID);
					int customerId = -1;
					if (customers.Count == 0)
						customerId = Controller.NewCustomer(new CustomerInfo(user.UserID, PortalId, _userName));
					else 
						customerId = customers[0].CustomerId;
				
					Controller.UpdateCartCustomerId(this.MainControl.CartId, customerId);
					Response.Redirect(Request.QueryString["returnUrl"]);
				}
				else
				{
					divMessages.Attributes.Add("class", "dnnFormMessage dnnFormWarning");
					string loginUrl = Globals.NavigateURL(TabId, "", "ctl=Login");
					lblIntro.Text = String.Format(Localization.GetString("LoginFailure.Text", this.LocalResourceFile),loginUrl);
				}
				
			}
		}

		protected void cmdNewUser_Click(object sender, EventArgs e)
		{
			if (lblUserNameConfirm.Text.ToLower() == txtUserName2Repeat.Text.ToLower())
			{
				_userName = txtUserName2Repeat.Text;
				UserInfo user = new UserInfo();
				user.Username = _userName;
				user.FirstName = "";
				user.LastName = "";
				user.PortalID = PortalId;
				user.Email = _userName;
				user.DisplayName = _userName;
				user.Membership.Password = UserController.GeneratePassword(10);
				user.IsSuperUser = false;

				user.Profile.PreferredLocale = PortalSettings.DefaultLanguage;
				user.Profile.PreferredTimeZone = PortalSettings.TimeZone;
				user.Profile.FirstName = user.FirstName;
				user.Profile.LastName = user.LastName;

				UserCreateStatus status = MembershipProvider.Instance().CreateUser(ref user);

			    if (status == UserCreateStatus.Success)
			    {
			        // Add User to Standard Roles
			        RoleController roleController = new RoleController();
			        RoleInfo role = new RoleInfo();

			        ArrayList roles = roleController.GetPortalRoles(PortalId);
			        for (int i = 0; i < roles.Count - 1; i++)
			        {
			            role = (RoleInfo) roles[i];
			            if (role.AutoAssignment == true)
			                roleController.AddUserRole(PortalId, user.UserID, role.RoleID, Null.NullDate, Null.NullDate);
			        }
			        // Log new user in and create a new customer +  add him to cart
			        UserController.UserLogin(PortalId, user, PortalSettings.PortalName, Request.UserHostAddress, false);
			        int customerId = Controller.NewCustomer(new CustomerInfo(user.UserID, PortalId, _userName));
			        Controller.UpdateCartCustomerId(this.MainControl.CartId, customerId);
			        Mail.SendMail(user, MessageType.UserRegistrationVerified, PortalSettings);
			        Response.Redirect(Request.QueryString["returnUrl"]);
			    }
			    else
			    {
                    divMessages.Attributes.Add("class", "dnnFormMessage dnnFormWarning");
                    string loginUrl = Globals.NavigateURL(TabId, "", "ctl=Login");
                    lblIntro.Text = String.Format(Localization.GetString("ErrorCreatingUser.Text", this.LocalResourceFile),status.ToString("G"));
                    txtUserName.Text = "";
                    pnlUser.Visible = true;
                    pnlCheckUser.Visible = true;
                    pnlPassword.Visible = false;
                    pnlConfirmUser.Visible = false;
                }
			}
			else
			{
				divMessages.Attributes.Add("class", "dnnFormMessage dnnFormWarning");
				string loginUrl = Globals.NavigateURL(TabId, "", "ctl=Login");
				lblIntro.Text = String.Format(Localization.GetString("DifferentUserNames.Text", this.LocalResourceFile));
				txtUserName.Text = "";
				pnlUser.Visible = true;
				pnlCheckUser.Visible = true;
				pnlPassword.Visible = false;
				pnlConfirmUser.Visible = false;
			}
		}
	}
}

						
