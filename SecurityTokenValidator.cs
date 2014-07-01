using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Bitboxx.DNNModules.BBStore
{
	[AttributeUsage(AttributeTargets.All)]
	public class SecurityTokenValidator: Attribute,IOperationBehavior
	{
		public string Role;

		public SecurityTokenValidator(string Value)
		{
			Role = Value;
		}
		
		#region IOperationBehavior Members

		public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
		{
			return;
		}

		public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
		{
			return;
		}

		public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
		{
			dispatchOperation.ParameterInspectors.Add(new SecurityTokenInspector(Role));
			return;
		}

		public void Validate(OperationDescription operationDescription)
		{
			return;
		}

		#endregion
	}
}