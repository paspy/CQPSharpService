using System;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using CQPSharpService.KancolleBot.Utility;

namespace CQPSharpService.KancolleBot.Modules {
    internal sealed class KCUpdateNotifier {

        private Timer m_tMonthlyRankingTimer;
        private Timer m_tSlotItemTimer;
        private bool m_bIsRankingBoardRuning;
        private bool m_bIsSlotItemRuning;
        private int m_iMaxSlotItem = 300;

        public KCUpdateNotifier() {
            Directory.CreateDirectory(RANKING_BOARD_PATH);
            Directory.CreateDirectory(SLOT_ITEM_PATH);
            m_bIsRankingBoardRuning = false;
            m_bIsSlotItemRuning = false;
            StartCheck();
        }

        public void StartCheck() {
            m_bIsRankingBoardRuning = true;
            m_bIsSlotItemRuning = true;
            m_tMonthlyRankingTimer = new Timer(CheckRankingBoards, null, 5000, Timeout.Infinite);
            m_tSlotItemTimer = new Timer(CheckSlotItems, null, 5000, Timeout.Infinite);
        }

        public void StopCheck() {
            m_bIsRankingBoardRuning = false;
            m_bIsSlotItemRuning = false;
        }

        private void CheckRankingBoards(object obj = null) {
            var prvMoJST = DateTime.UtcNow.AddMonths(-1).ToTokyoTime();
            using (HttpClient client = new HttpClient()) {
                for (int serverId = 1; serverId <= 20; serverId++) {
                    string dateId = prvMoJST.ToString("yyMM") + serverId.ToString("D2");
                    var url = string.Format(RANKING_BOARD_URL, dateId);
                    var filename = Path.Combine(RANKING_BOARD_PATH, "rank" + dateId + ".jpg");
                    if (!File.Exists(filename)) {
                        try {
                            var respone = client.GetAsync(url).Result;
                            if (respone.IsSuccessStatusCode) {
                                var result = respone.Content.ReadAsByteArrayAsync().Result;
                                File.WriteAllBytes(filename, result);
                                string updateMsg = string.Format("{0}月战果人事榜 - {1} 已更新。", prvMoJST.Month, KCBManager.SERVER_NAME[serverId - 1]);
                                KCBManager.Instance.AdminLog(updateMsg, true);
                            }
                        } catch {
                        }
                    }
                }
            }
            if (m_bIsRankingBoardRuning)
                m_tMonthlyRankingTimer.Change(240000, Timeout.Infinite);
        }

        private void CheckSlotItems(object obj = null) {
            List<Task> tLst = new List<Task>();
            for (int slotItemId = 1; slotItemId <= m_iMaxSlotItem; slotItemId++) {
                tLst.Add(CheckSlotItemAsync(slotItemId));
            }
            Task.WaitAll(tLst.ToArray());
            if (m_bIsSlotItemRuning)
                m_tSlotItemTimer.Change(15000, Timeout.Infinite);
        }

        private async Task CheckSlotItemAsync(int slotItemId) {
            var slotItemIdStr = slotItemId.ToString("D3");
            using (HttpClient client = new HttpClient()) {
                var rndSrvAddr = KCBManager.SERVER_ADDRESS[KCBManager.Rnd.Next(KCBManager.SERVER_ADDRESS.Count)];
                var slotItemFolder = Path.Combine(SLOT_ITEM_PATH, slotItemIdStr);
                var combinedName = string.Format("{0}_combined.png", slotItemIdStr);
                if (!Directory.Exists(slotItemFolder) || !File.Exists(Path.Combine(slotItemFolder, combinedName))) {
                    var cardUrl = string.Format(SLOT_ITEM_CARD_URL, rndSrvAddr, slotItemIdStr);
                    var respone = await client.GetAsync(cardUrl);
                    if (respone.IsSuccessStatusCode) {
                        var cardName = string.Format("{0}_card.png", slotItemIdStr);
                        Directory.CreateDirectory(slotItemFolder);
                        var charName = string.Format("{0}_char.png", slotItemIdStr);
                        var onName = string.Format("{0}_on.png", slotItemIdStr);
                        var upName = string.Format("{0}_up.png", slotItemIdStr);
                        var charUrl = string.Format(SLOT_ITEM_CHARATER_URL, rndSrvAddr, slotItemIdStr);
                        var onUrl = string.Format(SLOT_ITEM_ON_URL, rndSrvAddr, slotItemIdStr);
                        var upUrl = string.Format(SLOT_ITEM_UP_URL, rndSrvAddr, slotItemIdStr);
                        File.WriteAllBytes(Path.Combine(slotItemFolder, cardName), await respone.Content.ReadAsByteArrayAsync());
                        respone = await client.GetAsync(charUrl);
                        if (respone.IsSuccessStatusCode)
                            File.WriteAllBytes(Path.Combine(slotItemFolder, charName), await respone.Content.ReadAsByteArrayAsync());
                        respone = await client.GetAsync(onUrl);
                        if (respone.IsSuccessStatusCode)
                            File.WriteAllBytes(Path.Combine(slotItemFolder, onName), await respone.Content.ReadAsByteArrayAsync());
                        respone = await client.GetAsync(upUrl);
                        if (respone.IsSuccessStatusCode)
                            File.WriteAllBytes(Path.Combine(slotItemFolder, upName), await respone.Content.ReadAsByteArrayAsync());
                        var combinedPath = Path.Combine(slotItemFolder, combinedName);
                        CombineImages(slotItemFolder, combinedPath);
                        var updateMsg = string.Format("镇守府开发出了新装备哦~！\n编号：{0}\n{1}", slotItemIdStr, KCBManager.Instance.GetCoolQImageCode(combinedPath));
                        KCBManager.Instance.BroadcastToGroup(updateMsg);
                    }
                }
            }
        }

        private void CombineImages(string folderPath, string outName) {
            try {
                int nIndex = 0;
                int width = 0;
                List<int> imageHeights = new List<int>();
                var images = Directory.GetFiles(folderPath);
                foreach (var filename in images) {
                    using (Image img = Image.FromFile(filename)) {
                        imageHeights.Add(img.Height);
                        width += img.Width;
                    }
                }
                imageHeights.Sort();
                int height = imageHeights[imageHeights.Count - 1];
                using (Bitmap bitmapImg = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                using (Graphics g = Graphics.FromImage(bitmapImg)) {
                    g.Clear(Color.Transparent);
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    foreach (var filename in images) {
                        using (Image img = Image.FromFile(filename)) {
                            if (nIndex == 0) {
                                g.DrawImage(img, new Point(0, 0));
                                nIndex++;
                                width = img.Width;
                            } else {
                                g.DrawImage(img, new Point(width, 0));
                                width += img.Width;
                            }
                        }
                    }
                    bitmapImg.Save(outName, ImageFormat.Png);
                }
            } catch {

            }
        }

        private const int MAX_CHECK_TIME = 150000;
        private string MODULE_PATH => KCBManager.Instance.AppDataPath + @"\KCUpdateNotifier";
        private string RANKING_BOARD_PATH => MODULE_PATH + @"\rankingBoards";
        private string SLOT_ITEM_PATH => MODULE_PATH + @"\slotItems";

        private const string RANKING_BOARD_URL = "http://203.104.209.7/kcscontents/information/image/rank{0}.jpg";

        private const string SLOT_ITEM_CARD_URL = "http://{0}/kcs/resources/image/slotitem/card/{1}.png";
        private const string SLOT_ITEM_CHARATER_URL = "http://{0}/kcs/resources/image/slotitem/item_character/{1}.png";
        private const string SLOT_ITEM_ON_URL = "http://{0}/kcs/resources/image/slotitem/item_on/{1}.png";
        private const string SLOT_ITEM_UP_URL = "http://{0}/kcs/resources/image/slotitem/item_up/{1}.png";

    }
}
