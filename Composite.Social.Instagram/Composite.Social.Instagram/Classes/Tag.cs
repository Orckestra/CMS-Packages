using System;

namespace Composite.Social.Instagram.Classes 
{
    [Serializable]
    public class Tag : InstagramBaseObject
    {
        public string name;
        public int media_count;
    }
}