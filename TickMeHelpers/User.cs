using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace TickMeHelpers
{
    [Serializable]
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty(PropertyName = "authid")]
        public string AuthId { get; set; } = default(string);

        [JsonProperty(PropertyName = "authprovider")]
        public string AuthProvider { get; set; } = "none";

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = default(string);

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static User FromUser(ClaimsPrincipal user)
        {
            var claimsPrefix = @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/";
            var claims = new Dictionary<string,string>();
            foreach(var c in user.Claims)
            {
                claims.Add(c.Type, c.Value);
            }
            var provider = "azuread";

            return new User()
            {
                AuthId = claims[$"{claimsPrefix}nameidentifier"],
                AuthProvider = provider,
                Name = claims["name"]
            };
        }
    }
}
