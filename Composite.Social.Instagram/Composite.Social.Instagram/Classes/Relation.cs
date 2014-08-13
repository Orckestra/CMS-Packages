using System;

namespace Composite.Social.Instagram.Classes
{
    [Serializable]
    public class Relation : InstagramBaseObject
    {
        public string outgoing_status;
        public string incoming_status;
    }
}