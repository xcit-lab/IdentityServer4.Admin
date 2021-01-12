﻿using Microsoft.AspNetCore.HttpsPolicy;
using System;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.UI.Configuration
{
	public class SecurityConfiguration
	{
		/// <summary>
		/// The trusted domains from which content can be downloaded.
		/// </summary>
		public List<string> CspTrustedDomains { get; set; } = new List<string>();

		/// <summary>
		/// Use the developer exception page instead of the Identity error handler.
		/// </summary>
		public bool UseDeveloperExceptionPage { get; set; } = false;

		/// <summary>
		/// Enable HSTS in responses.
		/// </summary>
		public bool UseHsts { get; set; } = true;

		/// <summary>
		/// An action to configure the HSTS pipeline.
		/// </summary>
		public Action<HstsOptions> HstsAction { get; set; } = options => { };
	}
}
