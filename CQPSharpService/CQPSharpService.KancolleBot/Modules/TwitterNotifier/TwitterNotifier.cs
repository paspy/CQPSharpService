using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CQPSharpService.KancolleBot.Utility;

using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Events;

namespace CQPSharpService.KancolleBot.Modules {
    internal sealed class TwitterNotifier {

        private string ConsumerKey => KCBManager.Instance.TwitterConsumerKey;
        private string ConsumerSecret => KCBManager.Instance.TwitterConsumerSecret;
        private string AccessToken => KCBManager.Instance.TwitterAccessToken;
        private string AccessTokenSecret => KCBManager.Instance.TwitterAccessTokenSecret;
        private List<IUser> MainUserFriends => User.GetAuthenticatedUser().GetFriends().ToList();

        private Dictionary<string, List<ITweet>> m_todayTweets;
        private List<Task> m_taskList;
        private Timer m_avatarCheckTimer;

        public TwitterNotifier() {
            try {
                Directory.CreateDirectory(MODULE_PATH);
                m_todayTweets = new Dictionary<string, List<ITweet>>();
                m_taskList = new List<Task>();
                Auth.SetUserCredentials(ConsumerKey, ConsumerSecret, AccessToken, AccessTokenSecret);
                TweetinviConfig.ApplicationSettings.TweetMode = TweetMode.Extended;
                TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
                var watchStream = Tweetinvi.Stream.CreateUserStream();
                watchStream.TweetCreatedByFriend += TweetCreatedByFriendNotifier;
                TweetinviEvents.QueryBeforeExecute += RateLimitHandler;
                GetUserTodayTweets("KanColle_STAFF");
                m_avatarCheckTimer = new Timer(CheckAvatar, null, 1000, Timeout.Infinite);
                //new Thread(() => { watchStream.StartStream(); }) { Name = "TwitterNotifierStream" }.Start();
                m_taskList.Add(Sync.ExecuteTaskAsync(() => { watchStream.StartStream(); }));
            } catch (Exception e) {
                var le = ExceptionHandler.GetLastException();
                KCBManager.Instance.AdminLog(e.Message + le?.TwitterDescription);
            }
        }

        public string ToCoolQMessage(string screenName, int tweetNo = 1) {
            if (m_todayTweets.ContainsKey(screenName)) {
                var tweetLst = m_todayTweets[screenName];
                tweetNo--;
                if (tweetNo >= 0 && tweetNo < tweetLst.Count) {
                    var tweet = tweetLst[tweetNo];
                    return ToCoolQMessage(tweet);
                }
            }
            return string.Empty;
        }

        public string ToCoolQMessage(long tweetId) {
            return ToCoolQMessage(Tweet.GetTweet(tweetId));
        }

        public string ToCoolQMessage(ITweet tweet) {
            var user = tweet.CreatedBy;
            var userPath = Path.Combine(MODULE_PATH, user.Id.ToString());
            var normal = string.Format(@"\avatar_{0}.png", Enum.GetName(typeof(ImageSize), (int)ImageSize.normal));
            var avatar = KCBManager.Instance.GetCoolQImageCode(userPath + normal);
            string mediaStr = "\n";
            if (tweet.Media.Count > 0) {
                foreach (var media in tweet.Media) {
                    mediaStr += KCBManager.Instance.GetCoolQImageCode(media.MediaURL) + "\n";
                }
            }
            string finalContent = string.Format("{0} ({1})\n{2}{3}{4} JST",
                tweet.CreatedBy.Name,
                tweet.CreatedBy.ScreenName,
                tweet.Text, mediaStr, tweet.CreatedAt.ToTokyoTime().ToString());
            return finalContent;
        }

        private void TweetCreatedByFriendNotifier(object sender, TweetReceivedEventArgs args) {
            var tweet = args.Tweet;
            if (tweet.CreatedBy.ScreenName == "KanColle_STAFF")
                KCBManager.Instance.BroadcastToGroup(ToCoolQMessage(tweet));

        }

        private void RateLimitHandler(object sender, QueryBeforeExecuteEventArgs args) {
            var queryRateLimits = RateLimit.GetQueryRateLimit(args.QueryURL);
            if (queryRateLimits != null) {
                if (queryRateLimits.Remaining <= 0) {
                    var errorMsg =
                        string.Format("Waiting for RateLimits until : {0}", queryRateLimits.ResetDateTime.ToLongTimeString());
                    KCBManager.Instance.AdminLog(errorMsg, true);
                    Thread.Sleep((int)queryRateLimits.ResetDateTimeInMilliseconds);
                }
            }
        }

        private void CheckAvatar(object obj) {
            GetUserProfileImage("KanColle_STAFF");
            m_avatarCheckTimer.Change(MAX_CHECK_TIME, Timeout.Infinite);
        }

        private void GetUserProfileImage(string screenName) {
            var user = User.GetUserFromScreenName(screenName);
            var userPath = Path.Combine(MODULE_PATH, user.Id.ToString());
            var miniFilename = string.Format(@"\avatar_{0}.png", Enum.GetName(typeof(ImageSize), (int)ImageSize.mini));
            Directory.CreateDirectory(userPath);
            var latestChecksumImage = user.GetProfileImageStream(ImageSize.mini);
            if (File.Exists(userPath + miniFilename)) {
                var existChecksumImage = File.ReadAllBytes(userPath + miniFilename);
                using (Image latestImage = Image.FromStream(latestChecksumImage))
                using (MemoryStream latestPngMs = new MemoryStream()) {
                    latestImage.Save(latestPngMs, ImageFormat.Png);
                    var l = latestPngMs.ToArray();
                    if (!existChecksumImage.HashEquals(l)) {
                        for (ImageSize size = ImageSize.normal; size <= ImageSize.original; size++) {
                            var profileImage = user.GetProfileImageStream(size);
                            var filename = string.Format(@"\avatar_{0}.png", Enum.GetName(typeof(ImageSize), (int)size));
                            using (Image img = Image.FromStream(profileImage))
                                img.Save(userPath + filename, ImageFormat.Png);
                        }
                        var original = string.Format(@"\avatar_{0}.png", Enum.GetName(typeof(ImageSize), (int)ImageSize.original));
                        var avatar = KCBManager.Instance.GetCoolQImageCode(userPath + original);
                        string msgContent = string.Format("{0}(@{1}) 更新头像了哟~ {2}\n", user.Name, user.ScreenName, avatar);
                        KCBManager.Instance.BroadcastToGroup(msgContent);
                        KCBManager.Instance.AdminLog(msgContent, true);
                    }
                }
            } else {
                for (ImageSize size = ImageSize.normal; size <= ImageSize.original; size++) {
                    var profileImage = user.GetProfileImageStream(size);
                    var filename = string.Format(@"\avatar_{0}.png", Enum.GetName(typeof(ImageSize), (int)size));
                    using (Image img = Image.FromStream(profileImage))
                        img.Save(userPath + filename, ImageFormat.Png);
                }
            }

        }

        private void GetUserTodayTweets(string screenName) {
            TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
            m_todayTweets[screenName] = new List<ITweet>();
            var user = User.GetUserFromScreenName(screenName);
            var timeline = user.GetUserTimeline(100);
            foreach (var tweet in timeline) {
                var a = tweet.CreatedAt.ToTokyoTime().Day;
                var b = DateTime.Now.ToTokyoTime().Day;
                if (a == b) {
                    m_todayTweets[screenName].Add(tweet);
                }
            }
            m_todayTweets[screenName] = m_todayTweets[screenName].OrderByDescending(x => x.CreatedAt).ToList();
        }

        private const int MAX_CHECK_TIME = 150000;
        private string MODULE_PATH => KCBManager.Instance.AppDataPath + @"\TwitterNotifier";

    }
}
