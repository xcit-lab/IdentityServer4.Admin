using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
	public class IdentityControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
	{
		public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
		{
			if (IdentityControllerGenericTypes.ControllerTypes.Length > 0)
			{
				TypeInfo[] otherIdentityControllers = feature.Controllers.Where(t => t.Name == "IdentityController").ToArray();
				foreach (TypeInfo typeInfo in otherIdentityControllers)
				{
					feature.Controllers.Remove(typeInfo);
				}

				TypeInfo controllerType = typeof(BaseIdentityController<,,,,,,,,,,,,,>).MakeGenericType(IdentityControllerGenericTypes.ControllerTypes).GetTypeInfo();
				feature.Controllers.Add(controllerType); 
			}
		}
	}
}
