using System;

namespace Composite.Social.Instagram.Classes
{
    [Serializable]
    public class Pagination : InstagramBaseObject {
        public string next_url;
        public string next_max_id;
        public string next_max_like_id;
        public string next_max_tag_id;
        public string min_tag_id ;
    }
}