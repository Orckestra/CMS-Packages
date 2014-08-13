using System;

namespace Composite.Social.Instagram.Classes
{
    [Serializable]
    public class CommentList : InstagramBaseObject
    {
        public int count;
        public Comment[] data;
    }
}