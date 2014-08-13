using System;

namespace Composite.Social.Instagram.Classes
{
    [Serializable]
    public class ImagesList : InstagramBaseObject
    {
        public ImageLink low_resolution;
        public ImageLink thumbnail;
        public ImageLink standard_resolution;
    }
}