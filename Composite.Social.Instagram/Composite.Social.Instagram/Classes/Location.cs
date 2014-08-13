using System;

namespace Composite.Social.Instagram.Classes
{
    [Serializable]
    public class Location : InstagramBaseObject
    {
        public string id;
        public double latitude;
        public double longitude;
        public string name;

    }
}
