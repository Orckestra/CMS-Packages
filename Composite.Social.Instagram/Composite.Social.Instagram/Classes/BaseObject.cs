
namespace Composite.Social.Instagram.Classes
{
    [System.Serializable]
    public class InstagramBaseObject {
        protected InstagramApiWrapper InstagramApi { get { return InstagramApiWrapper.GetInstance(); } }
    }
}
