using System;

namespace Composite.Social.Instagram.Classes
{
    [Serializable]
    public class AccessToken : InstagramBaseObject
    {
        public string access_token;
        public User User;

        public AccessToken()
        {
        }

        public AccessToken(string json) {
            var tk = Deserialize(json);
            access_token = tk.access_token;
            User = tk.User;
        }

        public string GetJson() {
            return Serialize(this);
        }

        public static string Serialize(AccessToken token) {
            var json =  ApiBase.SerializeObject(token);
            return json;
        }

        public static AccessToken Deserialize(string json) {
            var token = ApiBase.DeserializeObject<AccessToken>(json);
            return token;
        }
    }
}
