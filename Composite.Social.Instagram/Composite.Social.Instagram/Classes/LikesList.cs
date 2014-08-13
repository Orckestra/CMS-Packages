using System;

namespace Composite.Social.Instagram.Classes
{
    [Serializable]
    public class LikesList : InstagramBaseObject
    {
        public int count;
        public User[] data;
    }
}