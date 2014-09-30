using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Composite.Social.Instagram.Classes;

namespace Composite.Social.Instagram
{
    public class InstagramApiWrapper : ApiBase
    {

        private static InstagramApiWrapper _sharedInstance = null;
        private static Configuration _sharedConfiguration = null;

        public Configuration Configuration
        {
            get { return _sharedConfiguration; }
            set { _sharedConfiguration = value; }
        }
        private InstagramApiWrapper()
        {
        }

        public static InstagramApiWrapper GetInstance(Configuration configuration, ICache cache)
        {
            lock (Threadlock)
            {
                if (_sharedInstance == null)
                {
                    _sharedInstance = new InstagramApiWrapper();
                    Cache = cache;
                    _sharedInstance.Configuration = configuration;
                }
            }

            return _sharedInstance;
        }
        public static InstagramApiWrapper GetInstance(Configuration configuration)
        {
            lock (Threadlock)
            {
                if (_sharedInstance == null)
                {
                    _sharedInstance = new InstagramApiWrapper { Configuration = configuration };
                }
            }

            return _sharedInstance;
        }
        public static InstagramApiWrapper GetInstance()
        {
            if (_sharedInstance == null)
            {
                if (_sharedConfiguration == null)
                    throw new ApplicationException("API Uninitialized");
                else
                    _sharedInstance = new InstagramApiWrapper();
            }
            return _sharedInstance;
        }

        #region auth
        public string AuthGetUrl(string scope)
        {
            if (string.IsNullOrEmpty(scope))
                scope = "basic";
            return Configuration.AuthUrl + "?client_id=" + Configuration.ClientId + "&redirect_uri=" + Configuration.ReturnUrl + "&response_type=code&scope=" + scope;
        }
        public AccessToken AuthGetAccessToken(string code)
        {
            string json = RequestPostToUrl(Configuration.TokenRetrievalUrl, new NameValueCollection
                                                               {
                                                                       {"client_id" , Configuration.ClientId},
                                                                       {"client_secret" , Configuration.ClientSecret},
                                                                       {"grant_type" , "authorization_code"},
                                                                       {"redirect_uri" , Configuration.ReturnUrl},
                                                                       {"code" , code}
                                                               }, Configuration.Proxy);

            if (!string.IsNullOrEmpty(json))
            {
                var tk = AccessToken.Deserialize(json);
                return tk;
            }

            return null;
        }
        #endregion

        #region user
        public InstagramResponse<User> UserDetails(string userid, string accessToken)
        {

            if (userid == "self")
                return CurrentUserDetails(accessToken);

            string url = Configuration.ApiBaseUrl + "users/" + userid + "?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "users/" + userid + "?client_id=" + Configuration.ClientId;

            if (Cache != null)
                if (Cache.Exists(url))
                    return Cache.Get<InstagramResponse<User>>("users/" + userid);

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<User>>(json);

            if (!string.IsNullOrEmpty(accessToken))
            {
                //CurrentUserIsFollowing(userid, accessToken);

                res.data.isFollowed = CurrentUserIsFollowing(res.data.id, accessToken);
            }

            if (Cache != null)
                Cache.Add("users/" + userid, res, 600);

            return res;
        }
        public InstagramResponse<User[]> UsersSearch(string query, string count, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "users/search?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "users/search?client_id=" + Configuration.ClientId;

            if (Cache != null)
                if (Cache.Exists(url))
                    return Cache.Get<InstagramResponse<User[]>>(url);

            if (!string.IsNullOrEmpty(query)) url = url + "&q=" + query;
            if (!string.IsNullOrEmpty(count)) url = url + "&count=" + count;
            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<User[]>>(json);

            if (Cache != null)
                Cache.Add(url, res, 300);

            return res;
        }
        public User[] UsersPopular(string accessToken)
        {
            var media = MediaPopular(accessToken, true).data;
            var users = UsersInMediaList(media);
            return users;
        }
        public InstagramResponse<InstagramMedia[]> UserRecentMedia(string userid, string min_id, string max_id, string count, string min_timestamp, string max_timestamp, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "users/" + userid + "/media/recent?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "users/" + userid + "/media/recent?client_id=" + Configuration.ClientId;

            if (!string.IsNullOrEmpty(min_id)) url = url + "&min_id=" + min_id;
            if (!string.IsNullOrEmpty(max_id)) url = url + "&max_id=" + max_id;
            if (!string.IsNullOrEmpty(count)) url = url + "&count=" + count;
            if (!string.IsNullOrEmpty(min_timestamp)) url = url + "&min_timestamp=" + min_timestamp;
            if (!string.IsNullOrEmpty(max_timestamp)) url = url + "&max_timestamp=" + max_timestamp;

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<InstagramMedia[]>>(json);


            return res;
        }


        public InstagramResponse<User> CurrentUserDetails(string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "users/self?access_token=" + accessToken;

            if (Cache != null)
                if (Cache.Exists("users/self/" + accessToken))
                    return Cache.Get<InstagramResponse<User>>("users/self/" + accessToken);

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<User>>(json);
            res.data.isSelf = true;

            if (Cache != null)
                Cache.Add("users/self/" + accessToken, res, 600);

            return res;
        }
        public InstagramResponse<InstagramMedia[]> CurrentUserRecentMedia(string min_id, string max_id, string count, string min_timestamp, string max_timestamp, string accessToken)
        {
            return UserRecentMedia("self", min_id, max_id, count, min_timestamp, max_timestamp, accessToken);
        }
        public InstagramResponse<InstagramMedia[]> CurrentUserFeed(string min_id, string max_id, string count, string accessToken)
        {
            var url = Configuration.ApiBaseUrl + "users/self/feed?access_token=" + accessToken;

            if (!string.IsNullOrEmpty(min_id)) url = url + "&min_id=" + min_id;
            if (!string.IsNullOrEmpty(max_id)) url = url + "&max_id=" + max_id;
            if (!string.IsNullOrEmpty(count)) url = url + "&count=" + count;

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<InstagramMedia[]>>(json);

            return res;
        }
        public InstagramResponse<InstagramMedia[]> CurrentUserLikedMedia(string max_like_id, string count, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "users/self/media/liked?access_token=" + accessToken;
            if (!string.IsNullOrEmpty(max_like_id)) url = url + "&max_like_id=" + max_like_id;
            if (!string.IsNullOrEmpty(count)) url = url + "&count=" + count;

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<InstagramMedia[]>>(json);

            return res;
        }
        public User[] UsersInMediaList(InstagramMedia[] media)
        {
            var users = new List<User>();
            foreach (var instagramMedia in media)
            {
                if (!users.Contains(instagramMedia.user))
                    users.Add(instagramMedia.user);
            }

            return users.ToArray();
        }
        #endregion

        #region relationships
        public InstagramResponse<User[]> UserFollows(string userid, string accessToken, string max_user_id)
        {
            var url = Configuration.ApiBaseUrl + "users/" + userid + "/follows?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "users/" + userid + "/follows?client_id=" + Configuration.ClientId;

            if (!string.IsNullOrEmpty(max_user_id)) url = url + "&cursor=" + max_user_id;

            //if(_cache!=null) 
            //    if (_cache.Exists(userid + "/follows"))
            //        return _cache.Get<InstagramResponse<User[]>>(userid + "/follows");

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<User[]>>(json);
            //https://api.instagram.com/v1/users/530914/follows?access_token=530914.0c0b99a.56e7a173b9af43eba8a60759904f6fc4&cursor=32754039"
            //if (_cache != null)
            //    _cache.Add(userid + "/follows", res);

            return res;
        }
        public InstagramResponse<User[]> UserFollowedBy(string userid, string accessToken, string max_user_id)
        {
            string url = Configuration.ApiBaseUrl + "users/" + userid + "/followed-by?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "users/" + userid + "/followed-by?client_id=" + Configuration.ClientId;

            if (!string.IsNullOrEmpty(max_user_id)) url = url + "&cursor=" + max_user_id;

            //if (_cache != null)
            //    if (_cache.Exists(userid + "/followed-by"))
            //        return _cache.Get<InstagramResponse<User[]>>(userid + "/followed-by");

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<User[]>>(json);

            //if (_cache != null)
            //    _cache.Add(userid + "/followed-by", res);

            return res;
        }

        public InstagramResponse<User[]> CurrentUserFollows(string accessToken, string maxUserId)
        {
            string url = Configuration.ApiBaseUrl + "users/self/follows?access_token=" + accessToken;

            if (!string.IsNullOrEmpty(maxUserId)) url = url + "&cursor=" + maxUserId;

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<User[]>>(json);



            return res;
        }
        public InstagramResponse<User[]> CurrentUserFollowedBy(string accessToken, string maxUserId)
        {
            string url = Configuration.ApiBaseUrl + "users/self/followed-by?access_token=" + accessToken;

            if (!string.IsNullOrEmpty(maxUserId)) url = url + "&cursor=" + maxUserId;

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<User[]>>(json);


            return res;
        }
        public InstagramResponse<User[]> CurrentUserRequestedBy(string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "users/self/requested-by?access_token=" + accessToken;



            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<User[]>>(json);



            return res;
        }

        public InstagramResponse<Relation> CurrentUserRelationshipWith(string recipientUserid, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "users/" + recipientUserid + "/relationship?access_token=" + accessToken;
            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<Relation>>(json);
            return res;
        }
        public void CurrentUserFollow(string userid, string[] recipientUserids, string accessToken)
        {
            foreach (var recipientUserid in recipientUserids)
            {
                CurrentUserFollow(userid, recipientUserid, accessToken);
            }
        }
        public bool CurrentUserFollow(string userid, string recipientUserid, string accessToken)
        {
            if (Cache != null)
            {
                Cache.Remove(userid + "/follows");
                Cache.Remove("users/self/" + accessToken);
            }

            return CurrentUserSetRelationship(userid, recipientUserid, "follow", accessToken).meta.code == "200";
        }
        public bool CurrentUserFollowToggle(string userid, string recipientUserid, string accessToken)
        {
            if (Cache != null)
            {
                Cache.Remove("self" + "/follows");
                Cache.Remove(userid + "/follows");
                Cache.Remove("users/" + recipientUserid);
                Cache.Remove("users/self/" + accessToken);
            }

            if (CurrentUserIsFollowing(recipientUserid, accessToken))
                return CurrentUserSetRelationship(userid, recipientUserid, "unfollow", accessToken).meta.code == "200";
            else
                return CurrentUserSetRelationship(userid, recipientUserid, "follow", accessToken).meta.code == "200";
        }
        public void CurrentUserUnfollow(string userid, string[] recipientUserids, string accessToken)
        {
            foreach (var recipientUserid in recipientUserids)
            {
                CurrentUserUnfollow(userid, recipientUserid, accessToken);
            }

            if (Cache != null)
                Cache.Remove(userid + "/follows");
        }
        public bool CurrentUserUnfollow(string userid, string recipientUserid, string accessToken)
        {
            if (Cache != null)
            {
                Cache.Remove(userid + "/follows");
                Cache.Remove("users/self/" + accessToken);
            }

            return CurrentUserSetRelationship(userid, recipientUserid, "unfollow", accessToken).meta.code == "200";
        }
        public bool CurrentUserBlock(string userid, string recipientUserid, string accessToken)
        {
            return CurrentUserSetRelationship(userid, recipientUserid, "block", accessToken).meta.code == "200";
        }
        public void CurrentUserApprove(string userid, string[] recipientUserids, string accessToken)
        {
            foreach (var recipientUserid in recipientUserids)
            {
                CurrentUserApprove(userid, recipientUserid, accessToken);
            }
        }
        public bool CurrentUserApprove(string userid, string recipientUserid, string accessToken)
        {
            return CurrentUserSetRelationship(userid, recipientUserid, "approve", accessToken).meta.code == "200";
        }
        public bool CurrentUserDeny(string userid, string recipientUserid, string accessToken)
        {
            return CurrentUserSetRelationship(userid, recipientUserid, "deny", accessToken).meta.code == "200";
        }
        public bool CurrentUserUnblock(string userid, string recipientUserid, string accessToken)
        {
            return CurrentUserSetRelationship(userid, recipientUserid, "unblock", accessToken).meta.code == "200";
        }
        public InstagramResponse<Relation> CurrentUserSetRelationship(string userid, string recipientUserid, string relationshipKey, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "users/" + recipientUserid + "/relationship?access_token=" + accessToken;
            url = url + "&action=" + relationshipKey;
            string json = RequestPostToUrl(url, new NameValueCollection { { "action", relationshipKey } }, Configuration.Proxy);
            if (string.IsNullOrEmpty(json)) //error
                return new InstagramResponse<Relation> { meta = new Metadata { code = "400" } };

            var res = DeserializeObject<InstagramResponse<Relation>>(json);

            if (Cache != null)
            {
                Cache.Remove("users/self/" + accessToken);
                Cache.Remove(userid + "/follows");
                Cache.Remove("users/" + recipientUserid);
            }

            return res;
        }

        public bool CurrentUserIsFollowing(string recipientUserid, string accessToken)
        {
            //outgoing_status: Your relationship to the user: "follows", "requested", "none". 
            //incoming_status: A user's relationship to you : "followed_by", "requested_by", "blocked_by_you", "none".
            Relation r = CurrentUserRelationshipWith(recipientUserid, accessToken).data;
            if (r.outgoing_status == "follows")
                return true;
            return false;
        }
        public bool CurrentUserIsFollowedBy(string recipientUserid, string accessToken)
        {
            //outgoing_status: Your relationship to the user: "follows", "requested", "none". 
            //incoming_status: A user's relationship to you : "followed_by", "requested_by", "blocked_by_you", "none".
            Relation r = CurrentUserRelationshipWith(recipientUserid, accessToken).data;
            if (r.incoming_status == "followed_by")
                return true;
            return false;
        }
        #endregion

        #region media
        public InstagramResponse<InstagramMedia> MediaDetails(string mediaid, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "media/" + mediaid + "?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "media/" + mediaid + "?client_id=" + Configuration.ClientId;

            if (Cache != null)
                if (Cache.Exists("media/" + mediaid))
                    return Cache.Get<InstagramResponse<InstagramMedia>>("media/" + mediaid);

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<InstagramMedia>>(json);
            if (Cache != null)
                Cache.Add("media/" + mediaid, res, 60);

            return res;
        }
        public InstagramResponse<InstagramMedia[]> MediaSearch(string lat, string lng, string distance, string minTimestamp, string maxTimestamp, string accessToken)
        {
            if (!string.IsNullOrEmpty(lat) && string.IsNullOrEmpty(lng) || !string.IsNullOrEmpty(lng) && string.IsNullOrEmpty(lat))
                throw new ArgumentException("if lat or lng are specified, both are required.");

            string url = Configuration.ApiBaseUrl + "media/search?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "media/search?client_id=" + Configuration.ClientId;

            if (!string.IsNullOrEmpty(lat)) url = url + "&lat=" + lat;
            if (!string.IsNullOrEmpty(lng)) url = url + "&lng=" + lng;
            if (!string.IsNullOrEmpty(distance)) url = url + "&distance=" + distance;
            if (!string.IsNullOrEmpty(minTimestamp)) url = url + "&min_timestamp=" + minTimestamp;
            if (!string.IsNullOrEmpty(maxTimestamp)) url = url + "&max_timestamp=" + maxTimestamp;

            if (Cache != null)
                if (Cache.Exists(url))
                    return Cache.Get<InstagramResponse<InstagramMedia[]>>(url);

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<InstagramMedia[]>>(json);

            if (Cache != null)
                Cache.Add(url, res, 60);

            return res;
        }
        public InstagramResponse<InstagramMedia[]> MediaPopular(string accessToken, bool usecache)
        {
            string url = Configuration.ApiBaseUrl + "media/popular?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "media/popular?client_id=" + Configuration.ClientId;

            if (Cache != null && usecache)
                if (Cache.Exists("media/popular"))
                    return Cache.Get<InstagramResponse<InstagramMedia[]>>("media/popular");

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<InstagramMedia[]>>(json);
            if (Cache != null)
                Cache.Add("media/popular", res, 600);

            return res;
        }
        #endregion

        #region comments
        public InstagramResponse<Comment[]> Comments(string mediaid, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "media/" + mediaid + "/comments?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "media/" + mediaid + "/comments?client_id=" + Configuration.ClientId;

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<Comment[]>>(json);
            return res;
        }
        public void CommentAdd(string[] mediaids, string text, string accessToken)
        {
            foreach (var mediaid in mediaids)
            {
                CommentAdd(mediaid, text, accessToken);
            }
        }
        public bool CommentAdd(string mediaid, string text, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "media/" + mediaid + "/comments?access_token=" + accessToken;
            var post = new NameValueCollection
                                       {
                                               {"text",text},
                                               {"access_token", accessToken}
                                       };
            string json = RequestPostToUrl(url, post, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return true;

            var res = DeserializeObject<InstagramResponse<Comment>>(json);

            if (Cache != null)
                Cache.Remove("media/" + mediaid);

            return res.meta.code == "200";
        }
        public bool CommentDelete(string mediaid, string commentid, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "media/" + mediaid + "/comments/" + commentid + "?access_token=" + accessToken;
            string json = RequestDeleteToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return false;

            var res = DeserializeObject<InstagramResponse<Comment>>(json);

            if (Cache != null)
                Cache.Remove("media/" + mediaid);

            return res.meta.code == "200";
        }
        #endregion

        #region likes
        public InstagramResponse<User[]> Likes(string mediaid, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "media/" + mediaid + "/likes?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "media/" + mediaid + "/likes?client_id=" + Configuration.ClientId;

            if (Cache != null)
                if (Cache.Exists("media/" + mediaid + "/likes"))
                    return Cache.Get<InstagramResponse<User[]>>("media/" + mediaid + "/likes");

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<User[]>>(json);

            if (Cache != null)
                Cache.Add("media/" + mediaid + "/likes", res, 60);

            return res;
        }
        public void LikeAdd(string[] mediaids, string accessToken)
        {
            foreach (var mediaid in mediaids)
            {
                LikeAdd(mediaid, null, accessToken);
            }
        }
        public bool LikeAdd(string mediaid, string userid, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "media/" + mediaid + "/likes?access_token=" + accessToken;
            var post = new NameValueCollection
                                       {
                                               {"access_token", accessToken}
                                       };
            string json = RequestPostToUrl(url, post, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return true;

            var res = DeserializeObject<InstagramResponse<User[]>>(json);

            if (Cache != null)
            {
                Cache.Remove("media/" + mediaid);
                Cache.Remove("media/popular");
                Cache.Remove("media/" + mediaid + "/likes");
                Cache.Remove("users/self/" + accessToken);
                Cache.Remove("users/" + userid);
                if (!string.IsNullOrEmpty(userid))
                {
                    Cache.Remove("users/" + userid + "/media/recent");
                    Cache.Remove("users/self/feed?access_token=" + accessToken);
                }
            }

            return res.meta.code == "200";
        }
        public void LikeDelete(string[] mediaids, string accessToken)
        {
            foreach (var mediaid in mediaids)
            {
                LikeDelete(mediaid, null, accessToken);
            }
        }
        public bool LikeDelete(string mediaid, string userid, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "media/" + mediaid + "/likes?access_token=" + accessToken;

            string json = RequestDeleteToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return true;

            var res = DeserializeObject<InstagramResponse<User[]>>(json);

            if (Cache != null)
            {
                Cache.Remove("media/popular");
                Cache.Remove("media/" + mediaid);
                Cache.Remove("media/" + mediaid + "/likes");
                Cache.Remove("users/self/" + accessToken);
                Cache.Remove("users/" + userid);
                if (!string.IsNullOrEmpty(userid))
                {
                    Cache.Remove("users/" + userid + "/media/recent");
                    Cache.Remove("users/self/feed?access_token=" + accessToken);
                }
            }

            return res.meta.code == "200";
        }
        public bool LikeToggle(string mediaid, string userid, string accessToken)
        {

            InstagramMedia media = MediaDetails(mediaid, accessToken).data;

            if (media.user_has_liked)
                return LikeDelete(mediaid, userid, accessToken);
            else
                return LikeAdd(mediaid, userid, accessToken);

        }
        public bool UserIsLiking(string mediaid, string userid, string accessToken)
        {

            var userlinking = Likes(mediaid, accessToken).data;
            foreach (User user in userlinking)
                if (user.id.Equals(userid))
                    return true;

            return false;
        }
        #endregion

        #region tags
        public InstagramResponse<Tag> TagDetails(string tagname, string accessToken)
        {
            if (tagname.Contains("#"))
                tagname = tagname.Replace("#", "");

            string url = Configuration.ApiBaseUrl + "tags/" + tagname + "?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "tags/" + tagname + "?client_id=" + Configuration.ClientId;

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<Tag>>(json);
            return res;
        }
        public InstagramResponse<Tag[]> TagSearch(string query, string accessToken)
        {
            if (query.Contains("#"))
                query = query.Replace("#", "");

            string url = Configuration.ApiBaseUrl + "tags/search?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "tags/search?client_id=" + Configuration.ClientId;

            if (!string.IsNullOrEmpty(query)) url = url + "&q=" + query;

            if (Cache != null)
                if (Cache.Exists(url))
                    return Cache.Get<InstagramResponse<Tag[]>>(url);

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<Tag[]>>(json);

            if (Cache != null)
                Cache.Add(url, res, 300);

            return res;
        }
        public Tag[] TagPopular(string accessToken)
        {
            InstagramMedia[] pop = MediaPopular(accessToken, true).data;
            return TagsInMediaList(pop);
        }
        public InstagramResponse<InstagramMedia[]> TagMedia(string tagname, string min_id, string max_id, string accessToken)
        {
            tagname = tagname.Replace(" ", string.Empty).Replace("#", string.Empty);

            string url = Configuration.ApiBaseUrl + "tags/" + HttpUtility.UrlEncode(tagname) + "/media/recent?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "tags/" + HttpUtility.UrlEncode(tagname) + "/media/recent?client_id=" + Configuration.ClientId;


            if (!string.IsNullOrEmpty(min_id)) url = url + "&min_id=" + min_id;
            if (!string.IsNullOrEmpty(max_id)) url = url + "&max_id=" + max_id;

            if (Cache != null)
                if (Cache.Exists(url))
                    return Cache.Get<InstagramResponse<InstagramMedia[]>>(url);


            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<InstagramMedia[]>>(json);

            if (Cache != null)
                Cache.Add(url, res, 300);

            return res;
        }
        public static Tag[] TagsInMediaList(InstagramMedia[] media)
        {
            var t = new List<string>();
            foreach (var instagramMedia in media)
            {
                foreach (string tag in instagramMedia.tags)
                {
                    if (!t.Contains(tag))
                        t.Add(tag);
                }
            }

            return TagsFromStrings(t.ToArray());
        }
        public static Tag[] TagsFromStrings(string[] tags)
        {
            var taglist = new List<Tag>(tags.Length);
            foreach (string s in tags)
            {
                var tag = new Tag
                {
                    media_count = 0,
                    name = s
                };
                taglist.Add(tag);
            }
            return taglist.ToArray();
        }
        #endregion

        #region locations
        public Location LocationDetails(string locationid, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "locations/" + locationid + "?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "locations/" + locationid + "?client_id=" + Configuration.ClientId;

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return null;

            var res = DeserializeObject<InstagramResponse<Location>>(json);
            return res.data;
        }
        public InstagramMedia[] LocationMedia(string locationid, string minId, string maxId, string minTimestamp, string maxTimestamp, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "locations/" + locationid + "/media/recent?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "locations/" + locationid + "/media/recent?client_id=" + Configuration.ClientId;

            if (!string.IsNullOrEmpty(minId)) url = url + "&min_id=" + minId;
            if (!string.IsNullOrEmpty(maxId)) url = url + "&max_id=" + maxId;
            if (!string.IsNullOrEmpty(minTimestamp)) url = url + "&min_timestamp=" + minTimestamp;
            if (!string.IsNullOrEmpty(maxTimestamp)) url = url + "&max_timestamp=" + maxTimestamp;
            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return new InstagramMedia[0];

            var res = DeserializeObject<InstagramResponse<InstagramMedia[]>>(json);
            return res.data;
        }
        public Location[] LocationSearch(string lat, string lng, string foursquareId, string foursquareV2Id, string distance, string accessToken)
        {
            if (!string.IsNullOrEmpty(lat) && string.IsNullOrEmpty(lng) || !string.IsNullOrEmpty(lng) && string.IsNullOrEmpty(lat))
                throw new ArgumentException("if lat or lng are specified, both are required.");

            string url = Configuration.ApiBaseUrl + "locations/search?access_token=" + accessToken;
            if (string.IsNullOrEmpty(accessToken))
                url = Configuration.ApiBaseUrl + "locations/search?client_id=" + Configuration.ClientId;

            if (!string.IsNullOrEmpty(lat)) url = url + "&lat=" + lat;
            if (!string.IsNullOrEmpty(lng)) url = url + "&lng=" + lng;
            if (!string.IsNullOrEmpty(foursquareId)) url = url + "&foursquare_id=" + foursquareId;
            if (!string.IsNullOrEmpty(foursquareV2Id)) url = url + "&foursquare_v2_id=" + foursquareV2Id;
            if (!string.IsNullOrEmpty(distance)) url = url + "&distance=" + distance;

            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return new Location[0];


            var res = DeserializeObject<InstagramResponse<Location[]>>(json);
            return res.data;
        }
        #endregion

        #region geography
        public InstagramMedia[] GeographyMedia(string geographyid, string accessToken)
        {
            string url = Configuration.ApiBaseUrl + "geographies/" + geographyid + "/media/recent?access_token=" + accessToken;
            string json = RequestGetToUrl(url, Configuration.Proxy);
            if (string.IsNullOrEmpty(json))
                return new InstagramMedia[0];

            var res = DeserializeObject<InstagramResponse<InstagramMedia[]>>(json);
            return res.data;
        }
        #endregion

    }
}
