using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Dispatcher;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;

namespace Bitboxx.DNNModules.BBStore
{
	public class SecurityTokenInspector:IParameterInspector
	{
		public string Role;
		public SecurityTokenInspector(string value)
		{
			Role = value;
		}
		
		#region IParameterInspector Members

		public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
		{
			return;
		}

		public object BeforeCall(string operationName, object[] inputs)
		{
			// token will always be the last parameter
			int index = inputs.Length - 1;
			string TokenId = inputs[index].ToString();

			// first make sure token exists
			BBStoreSecurityToken token = (BBStoreSecurityToken)DataCache.GetCache("BBStoreSecurityToken_" + TokenId);
			if (token == null)
				throw new Exception("Security Token Expired. Please request a new Token");

			// if token exists, check user roles
			UserInfo user = UserController.GetUserById(token.PortalId, token.UserId);
			if (!user.IsInRole(Role))
				throw new Exception("Access Denied. Role Membership Requirements not met.");
			return null;
		}

		#endregion
	}
}