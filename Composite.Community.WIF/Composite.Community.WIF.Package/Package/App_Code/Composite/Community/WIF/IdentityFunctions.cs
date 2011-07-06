using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Web;
using Microsoft.IdentityModel.Web.Controls;

namespace Composite.Community.WIF
{
	public class IdentityFunctions
	{
		private static ClaimsIdentity CurrentIdentity
		{
			get { return Thread.CurrentPrincipal.Identity as ClaimsIdentity; }
		}

		public static bool IsLoggedIn()
		{
			return CurrentIdentity != null && CurrentIdentity.Claims.Count > 0 /* !string.IsNullOrEmpty(CurrentIdentity.Name)*/;
		}

		public static void RedirectToLogin()
		{
			(HttpContext.Current.Handler as Page).PreRender += (o, args) => RedirectToLoginInt();
		}

		public static void RedirectToLoginInt()
		{
			var btn = new FederatedPassiveSignIn();
			btn.UseFederationPropertiesFromConfiguration = true;
			btn.Page = (HttpContext.Current.Handler as Page);

			BindingFlags instanceNotPublic = BindingFlags.Instance | BindingFlags.NonPublic;

			typeof(SignInControl)
				.GetMethod("CreateChildControls", instanceNotPublic)
				.Invoke(btn, new object[0]);

			typeof(FederatedPassiveSignIn)
				.GetMethod("CreateWSFederationAuthenticationModule", instanceNotPublic)
				.Invoke(btn, new object[0]);

			typeof(FederatedPassiveSignIn)
				.GetMethod("OnClick", instanceNotPublic)
				.Invoke(btn, new object[] { null, null });
		}


		public static void LogOut()
		{
			(HttpContext.Current.Handler as Page).PreRender += (o, args) => LogOutInt();
		}

		public static void LogOutInt()
		{
			FormsAuthentication.SignOut();
			FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();

			var context = HttpContext.Current;
			context.Response.Redirect(context.Request.RawUrl);
		}

		public static string GetName()
		{
			var identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
			if (identity == null) return string.Empty;

			if (identity.Name != null) return identity.Name;

			var emailClaim = identity.Claims.FirstOrDefault(c => c.ClaimType == ClaimTypes.Email);
			if (emailClaim != null) return emailClaim.Value;

			var nameIdentifierClaim = identity.Claims.FirstOrDefault(c => c.ClaimType == ClaimTypes.NameIdentifier);
			if (nameIdentifierClaim != null) return "ID: " + nameIdentifierClaim.Value;

			return string.Empty;
		}

		public static void DisableAspNetCache()
		{
			var context = HttpContext.Current;
			if (context == null) return;

			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}
	}
}