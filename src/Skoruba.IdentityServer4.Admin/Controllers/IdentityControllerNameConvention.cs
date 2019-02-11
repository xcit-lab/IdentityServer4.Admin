using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
	public class IdentityControllerNameConvention : IControllerModelConvention
	{
		public void Apply(ControllerModel controller)
		{
			if (controller.ControllerType.IsGenericType && controller.ControllerName.StartsWith("BaseIdentityController`"))
			{
				controller.ControllerName = "Identity";
			}
		}
	}
}
