using System;

namespace Composite.Social.Instagram.Classes
{
    [Serializable]
    public class ImageLink : InstagramBaseObject
    {
        public string url;
        public int width;
        public int height;
    }
}