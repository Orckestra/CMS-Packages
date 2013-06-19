using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Xml.Linq;
using Composite.Community.OpenID.Validators;
using Composite.Data;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace Composite.Community.OpenID
{
    public class OpenIDFacade
    {
        internal static string SignIn(Guid userDetailsPageId)
        {
            string errorMessage = string.Empty;

            using (var openid = new OpenIdRelyingParty())
            {
                //fix wrong URI, add PathInfo
                HttpContext.Current.RewritePath(HttpContext.Current.Request.RawUrl);
                IAuthenticationResponse response = openid.GetResponse();
                if (response == null)
                {
                    //request
                    string openIDIdentifier = HttpContext.Current.Request.Form["openid_identifier"];

                    if (!string.IsNullOrEmpty(openIDIdentifier))
                    {
                        IAuthenticationRequest request = openid.CreateRequest(openIDIdentifier);
                        var claims = new ClaimsRequest();
                        claims.Email = DemandLevel.Require;
                        claims.Nickname = DemandLevel.Require;
                        claims.FullName = DemandLevel.Require;
                        claims.Country = DemandLevel.Require;
                        request.AddExtension(claims);

                        //small fix for request.RedirectToProvider();
                        string location = request.RedirectingResponse.Headers["Location"];
                        HttpContext.Current.Response.Redirect(location, false);
                    }
                }
                else
                {
                    //response
                    switch (response.Status)
                    {
                        case AuthenticationStatus.Authenticated:
                            HandleSuccessfulSignIn(response, userDetailsPageId);
                            break;
                        case AuthenticationStatus.Canceled:
                            errorMessage = "Login was cancelled at the provider.";
                            break;
                        case AuthenticationStatus.Failed:
                            errorMessage = "Login failed at the provider.";
                            break;
                        case AuthenticationStatus.SetupRequired:
                            errorMessage = "The provider requires setup.";
                            break;
                        default:
                            errorMessage = "Login failed.";
                            break;
                    }
                }
            }
            return errorMessage;
        }

        private static void HandleSuccessfulSignIn(IAuthenticationResponse response, Guid userDetailsPageId)
        {
            string displayName = string.Empty;
            string email = string.Empty;
            string fullName = string.Empty;
            string website = string.Empty;
            string country = string.Empty;
            string aboutMe = string.Empty;

            var claims = response.GetExtension<ClaimsResponse>();
            if (claims != null)
            {
                displayName = claims.Nickname;
                email = claims.Email;
                fullName = claims.FullName;
                country = claims.Country;
            }

            string claimedIdentifier = response.ClaimedIdentifier.ToString();

            Guid userId = Guid.NewGuid();
            OpenIDs openIDs = DataFacade.GetData<OpenIDs>().Where(o => o.Url == claimedIdentifier).FirstOrDefault();

            if (openIDs == null)
            {
                //new user
                var user = DataFacade.BuildNew<Users>();
                user.Id = userId;
                user.DisplayName = string.IsNullOrEmpty(displayName)
                                       ? "user_" + DateTime.Now.ToString("ddMMyyyyhhmmss")
                                       : displayName;
                user.Email = RegExp.Validate(RegExpLib.Email, email, false) ? email : string.Empty;
                user.RealName = fullName ?? string.Empty;
                user.Website = website ?? string.Empty;
                user.Location = country ?? string.Empty;
                user.AboutMe = aboutMe ?? string.Empty;
                DataFacade.AddNew(user);

                //attach openid
                var openId = DataFacade.BuildNew<OpenIDs>();
                openId.Id = Guid.NewGuid();
                openId.UserId = userId;
                openId.Url = claimedIdentifier;
                DataFacade.AddNew(openId);
            }
            else
            {
                userId = openIDs.UserId;
                //TODO: do we need dynamically update it?
            }

            FormsAuthentication.SetAuthCookie(userId.ToString(), false);

            string returnUrl = HttpContext.Current.Request.QueryString["returnUrl"];
            if (openIDs == null && Utils.Redirect(userDetailsPageId, returnUrl))
            {
                return;
            }
            else if (Utils.RedirectToReturnUrl())
            {
                return;
            }
            else
            {
                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.AbsolutePath, false);
                return;
            }
        }

        internal static void SignOut()
        {
            FormsAuthentication.SignOut();

            Utils.RedirectToReturnUrl();
        }

        internal static string GetCurrentUserDisplayName()
        {
            Guid userId = GetCurrentUserGuid();

            if (userId != Guid.Empty)
            {
                return GetUser(userId).DisplayName;
            }

            return string.Empty;
        }

        public static Guid GetCurrentUserGuid()
        {
            Guid userId;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (Guid.TryParse(HttpContext.Current.User.Identity.Name, out userId))
                {
                    if (DataFacade.GetData<Users>(d => d.Id == userId).Any())
                        return userId;
                }
            }

            return Guid.Empty;
        }

        public static Users GetCurrentUser()
        {
            Guid userId = GetCurrentUserGuid();
            return userId == Guid.Empty ? null : GetUser(userId);
        }

        internal static XElement GetCurrentUserDetails()
        {
            if (IsAuthenticated())
            {
                Guid userId = GetCurrentUserGuid();
                return GetUserDetails(userId);
            }
            else
            {
                return new XElement("User");
            }
        }

        internal static bool IsAuthenticated()
        {
            return GetCurrentUserGuid() == Guid.Empty ? false : true;
        }

        private static Users GetUser(Guid userId)
        {
            return DataFacade.GetData<Users>().Where(u => u.Id == userId).FirstOrDefault();
        }

        public static XElement GetUserDetails(Guid userId)
        {
            Users user = GetUser(userId);

            return new XElement("User",
                                new XElement("Data",
                                             new XAttribute("DisplayName", user.DisplayName ?? string.Empty),
                                             new XAttribute("Email", user.Email),
                                             new XAttribute("RealName", user.RealName ?? string.Empty),
                                             new XAttribute("Website", user.Website ?? string.Empty),
                                             new XAttribute("Location", user.Location ?? string.Empty),
                                             new XAttribute("AboutMe", user.AboutMe ?? string.Empty)
                                    )
                );
        }

        public static XElement SaveCurrentUserDetails()
        {
            string displayName = HttpContext.Current.Request.Form["displayName"];
            string email = HttpContext.Current.Request.Form["email"];
            string realName = HttpContext.Current.Request.Form["realName"] ?? string.Empty;
            string website = HttpContext.Current.Request.Form["website"] ?? string.Empty;
            string location = HttpContext.Current.Request.Form["location"] ?? string.Empty;
            string aboutMe = HttpContext.Current.Request.Form["aboutMe"] ?? string.Empty;

            var errorText = new Hashtable();
            var errors = new List<XElement>();

            if (string.IsNullOrEmpty(displayName))
            {
                errorText.Add("DisplayName", Utils.GetLocalized("UserDetailsForm", "DisplayName"));
                displayName = string.Empty;
            }

            if (!RegExp.Validate(RegExpLib.Email, email, false))
            {
                errorText.Add("Email", Utils.GetLocalized("UserDetailsForm", "Email"));
            }

            if (errorText.Count > 0)
            {
                errors.AddRange(from string key in errorText.Keys
                                select new XElement("Error", new XAttribute(key, errorText[key].ToString())));
            }
            else
            {
                var userId = new Guid(HttpContext.Current.User.Identity.Name);
                Users user = DataFacade.GetData<Users>().Where(u => u.Id == userId).FirstOrDefault();
                user.DisplayName = displayName;
                user.Email = email;
                user.RealName = realName;
                user.Website = website;
                user.Location = location;
                user.AboutMe = aboutMe;

                DataFacade.Update(user);
            }

            Utils.RedirectToReturnUrl();

            return new XElement("User",
                                new XElement("Data",
                                             new XAttribute("DisplayName", displayName),
                                             new XAttribute("Email", email),
                                             new XAttribute("RealName", realName),
                                             new XAttribute("Website", website),
                                             new XAttribute("Location", location),
                                             new XAttribute("AboutMe", aboutMe)
                                    ), errors
                );
        }
    }
}