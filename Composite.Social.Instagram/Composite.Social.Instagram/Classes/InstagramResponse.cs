using System;

namespace Composite.Social.Instagram.Classes
{
    [Serializable]
    public class InstagramResponse<T> {
        public Pagination pagination;
        public Metadata meta;
        public T data;
    }
}
