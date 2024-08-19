﻿using System.Diagnostics;
using System.Media;
using ToNSaveManager.Extensions;
using ToNSaveManager.Models;
using ToNSaveManager.Utils;
using ToNSaveManager.Windows;

using OnLineArgs = ToNSaveManager.Utils.LogWatcher.OnLineArgs;
using LogContext = ToNSaveManager.Utils.LogWatcher.LogContext;
using ToNSaveManager.Utils.Discord;
using ToNSaveManager.Localization;
using ToNSaveManager.Models.Index;
using System.Xml.Linq;

namespace ToNSaveManager
{
    public partial class MainWindow : Form {
        #region Initialization
        internal static readonly LogWatcher LogWatcher = new LogWatcher("wrld_a61cdabe-1218-4287-9ffc-2a4d1414e5bd");
        // internal static readonly AppSettings Settings = AppSettings.Import();
        internal static readonly SaveData SaveData = SaveData.Import();
        internal static MainWindow? Instance;
        internal static bool Started;

        public MainWindow() {
            InitializeComponent();
            listBoxKeys.FixItemHeight();
            listBoxEntries.FixItemHeight();
            Instance = this;
        }
        #endregion

        #region Form Events

        #region Main Window
        private string OriginalTitle = string.Empty;
        public void SetTitle(string? title) {
            this.Text = string.IsNullOrEmpty(title) ? OriginalTitle : OriginalTitle + " | " + title;
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e) {
            Debug.WriteLine("Main Window form is closing.");
            WinSettings.Get.LastWindowWidth = this.Width;
            WinSettings.Get.LastWindowHeight = this.Height;
            WinSettings.Get.LastWindowSplit = splitContainer1.SplitterDistance;
            WinSettings.Export();
        }

        private void MainWindow_Activated(object sender, EventArgs e) {
            if (EditWindow.IsActive) EditWindow.Instance.Focus();
        }

        private void mainWindow_Loaded(object sender, EventArgs e) {
            if (WinSettings.Get.LastWindowWidth > this.Width)
                this.Width = WinSettings.Get.LastWindowWidth;

            if (WinSettings.Get.LastWindowHeight > this.Height)
                this.Height = WinSettings.Get.LastWindowHeight;

            if (WinSettings.Get.LastWindowSplit > 0)
                splitContainer1.SplitterDistance = WinSettings.Get.LastWindowSplit;

            OriginalTitle = this.Text;
            this.Text = "Loading, please wait...";

            XSOverlay.SetPort(Settings.Get.XSOverlayPort);

            SetBackupButton(Settings.Get.DiscordWebhookEnabled && !string.IsNullOrWhiteSpace(Settings.Get.DiscordWebhookURL));
            TooltipUtil.Set(linkSupport, "Buy Me A Coffee ♥");
        }

        private void mainWindow_Shown(object sender, EventArgs e) {
            LocalizeContent();

            if (Started) return;

            FirstImport();

            LogWatcher.OnLine += LogWatcher_OnLine;
            LogWatcher.OnTick += LogWatcher_OnTick;
            LogWatcher.Start();

            Started = true;
            SetTitle(null);

            LilOSC.SendData(true);
            StatsWindow.UpdateChatboxContent();
        }

        internal void LocalizeContent() {
            LANG.C(btnSettings, "MAIN.SETTINGS");
            LANG.C(btnObjectives, "MAIN.OBJECTIVES");
            LANG.C(linkWiki, "MAIN.WIKI");
            LANG.C(linkSupport, "MAIN.SUPPORT");
            LANG.C(btnStats, "MAIN.STATS");

            LANG.C(importToolStripMenuItem, "MAIN.CTX_IMPORT"); // .TITLE
            LANG.C(renameToolStripMenuItem, "MAIN.CTX_RENAME"); // .TITLE
            LANG.C(deleteToolStripMenuItem, "MAIN.CTX_DELETE"); // .TITLE

            LANG.C(ctxMenuEntriesCopyTo, "MAIN.CTX_ADD_TO");
            LANG.C(ctxMenuEntriesNew, "MAIN.CTX_ADD_TO.NEW");
            LANG.C(ctxMenuEntriesNote, "MAIN.CTX_EDIT_NOTE");
            LANG.C(ctxMenuEntriesBackup, "MAIN.CTX_BACKUP");
            LANG.C(ctxMenuEntriesDelete, "MAIN.CTX_DELETE");

            Entry.LocalizeContent();
            DSWebHook.LocalizeContent();

            RightToLeft = LANG.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
        }
        #endregion

        #region ListBox Keys
        private void listBoxKeys_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) {
                int index = listBoxKeys.IndexFromPoint(e.Location);
                if (index < 0 || index >= SaveData.Count) return;
                listBoxKeys.SelectedIndex = index;
            }
        }

        private void listBoxKeys_MouseUp(object sender, MouseEventArgs e) {
            bool isRight = e.Button == MouseButtons.Right;
            if (e.Button == MouseButtons.Left || isRight) {
                int index = listBoxKeys.SelectedIndex;
                if (index < 0)
                    return;

                if (isRight && index == listBoxKeys.IndexFromPoint(e.Location)) {
                    ctxMenuKeys_Show((ListBox)sender, e.Location);
                }

                UpdateEntries();
            }
        }

        private void listBoxKeys_KeyDown(object sender, KeyEventArgs e) {
            int count = listBoxKeys.Items.Count, index;

            if (count > 0) {
                switch (e.KeyCode) {
                    case Keys.Enter:
                        if (listBoxKeys.SelectedItem != null) {
                            if (e.Shift) ctxMenuKeys_Show((ListBox)sender, listBoxKeys.GetItemRectangle(listBoxKeys.SelectedIndex).Location);
                            else UpdateEntries();
                        }
                        break;

                    case Keys.Down:
                        index = listBoxKeys.SelectedIndex + 1;
                        if (index >= listBoxKeys.Items.Count) index = 0;
                        listBoxKeys.SelectedIndex = Math.Max(index, 0);
                        break;

                    case Keys.Up:
                        index = listBoxKeys.SelectedIndex - 1;
                        if (index < 0) index = listBoxKeys.Items.Count - 1;
                        listBoxKeys.SelectedIndex = Math.Min(index, listBoxKeys.Items.Count - 1);
                        break;

                    default:
                        break;
                }
            }

            e.Handled = true;
        }

        #region Context Menu | Keys
        private void ctxMenuKeys_Show(Control control, Point position) {
            importToolStripMenuItem.Enabled = renameToolStripMenuItem.Enabled =
                listBoxKeys.SelectedItem != null && ((History)listBoxKeys.SelectedItem).IsCustom;

            ctxMenuKeys.Show(control, position);
        }

        private void ctxMenuKeysImport_Click(object sender, EventArgs e) {
            if (listBoxKeys.SelectedItem == null) return;
            History h = (History)listBoxKeys.SelectedItem;
            if (!h.IsCustom) return;

            EditResult edit = EditWindow.Show(string.Empty, LANG.S("MAIN.CTX_IMPORT.TITLE") ?? "Import Title", this);
            if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text)) {
                string content = edit.Text.Trim();
                AddCustomEntry(new Entry(content, DateTime.Now) { Note = "Imported" }, h);
                Export();
            }
        }

        private void ctxMenuKeysRename_Click(object sender, EventArgs e) {
            if (listBoxKeys.SelectedItem == null) return;
            History h = (History)listBoxKeys.SelectedItem;
            if (!h.IsCustom) return;

            EditResult edit = EditWindow.Show(h.Name, LANG.S("MAIN.CTX_RENAME.TITLE") ?? "Set Collection Name", this);
            if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text)) {
                string title = edit.Text.Trim();
                if (title == h.Name) return;

                h.Name = title;
                listBoxKeys.Refresh();
                SetTitle(title);

                Export(h);
            }
        }

        private void ctxMenuKeysDelete_Click(object sender, EventArgs e) {
            int selectedIndex = listBoxKeys.SelectedIndex;
            if (selectedIndex != -1 && listBoxKeys.SelectedItem != null) {
                History h = (History)listBoxKeys.SelectedItem;
                DialogResult result = MessageBox.Show(
                    LANG.S("MAIN.CTX_DELETE_ALL.SUBTITLE", h.ToString()) ?? $"Are you SURE that you want to delete this entry?\n\nEvery code from '{h}' will be permanently deleted.\nThis operation is not reversible!",
                    LANG.S("MAIN.CTX_DELETE_ALL.TITLE", h.ToString()) ?? "Deleting Entry: " + h.ToString(),
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.OK) {
                    listBoxKeys.SelectedIndex = -1;
                    listBoxKeys.Items.Remove(h);
                    SaveData.Remove(h);
                    UpdateEntries();
                    SetTitle(null);
                    Export(null, true);
                }
            }
        }
        #endregion
        #endregion

        #region ListBox Entries
        private void listBoxEntries_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) {
                int index = listBoxEntries.IndexFromPoint(e.Location);
                if (index < 0) return;
                listBoxEntries.SelectedIndex = index;
            }
        }

        private void listBoxEntries_MouseUp(object sender, MouseEventArgs e) {
            bool isRight = e.Button == MouseButtons.Right;
            if (e.Button == MouseButtons.Left || isRight) {
                int index = listBoxEntries.SelectedIndex;
                if (index < 0) return;

                if (isRight && index == listBoxEntries.IndexFromPoint(e.Location)) {
                    ctxMenuEntries.Show((ListBox)sender, e.Location);
                    return;
                }

                if (listBoxEntries.SelectedItem != null) {
                    Entry entry = (Entry)listBoxEntries.SelectedItem;
                    entry.CopyToClipboard();
                    MessageBox.Show(LANG.S("MESSAGE.COPY_TO_CLIPBOARD") ?? "Copied to clipboard!\n\nYou can now paste the code in game.", LANG.S("MESSAGE.COPY_TO_CLIPBOARD.TITLE") ?? "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                listBoxEntries.SelectedIndex = -1;
            }
        }

        private void listBoxEntries_DrawItem(object sender, DrawItemEventArgs e) {
            if (e.Index < 0)
                return;

            e.DrawBackground();

            ListBox listBox = (ListBox)sender;
            string itemText = listBox.Items[e.Index].ToString() ?? string.Empty;

            int maxWidth = e.Bounds.Width;
            TextRenderer.DrawText(e.Graphics, GetTruncatedText(itemText, listBox.Font, maxWidth), listBox.Font, e.Bounds, e.ForeColor, TextFormatFlags.VerticalCenter);

            e.DrawFocusRectangle();
        }

        private void listBoxEntries_Resize(object sender, EventArgs e) {
            listBoxEntries.Refresh();
        }

        // Tooltips
        int PreviousTooltipIndex = -1;
        private void listBoxEntries_MouseMove(object sender, MouseEventArgs e) {
            // Get the index of the item under the mouse pointer
            int index = listBoxEntries.IndexFromPoint(e.Location);

            if (PreviousTooltipIndex != index) {
                PreviousTooltipIndex = index;

                if (index < 0) {
                    TooltipUtil.Set(listBoxEntries, null);
                    return;
                }

                Entry entry = (Entry)listBoxEntries.Items[index];
                TooltipUtil.Set(listBoxEntries, entry.GetTooltip(Settings.Get.SaveNames, Settings.Get.SaveRoundInfo));
            }
        }

        // Reset tooltip when mouse leaves the control.
        // This prevents accidental tooltip display when doing ALT+TAB.
        private void listBoxEntries_MouseLeave(object sender, EventArgs e) {
            if (PreviousTooltipIndex < 0) return;
            PreviousTooltipIndex = -1;
            TooltipUtil.Set(listBoxEntries, null);
        }

        #region Context Menu | Entries
        private Entry? ContextEntry;

        private void ctxMenuEntries_Closed(object sender, ToolStripDropDownClosedEventArgs e) {
            listBoxEntries.Enabled = true;
            if (e.CloseReason != ToolStripDropDownCloseReason.ItemClicked)
                listBoxEntries.SelectedIndex = -1;
        }

        private void ctxMenuEntries_Opened(object sender, EventArgs e) {
            listBoxEntries.Enabled = false;
            ctxMenuEntriesCopyTo.DropDownItems.Clear();

            // Might not be the most efficient way of doing this
            foreach (History h in SaveData.Collection) {
                if (!h.IsCustom) continue;

                ToolStripMenuItem item = new ToolStripMenuItem(h.Name);
                ctxMenuEntriesCopyTo.DropDownItems.Insert(0, item);
                item.Click += (o, e) => {
                    if (ContextEntry != null)
                        AddCustomEntry(ContextEntry, h);
                };
            }

            ctxMenuEntriesCopyTo.DropDownItems.Add(ctxMenuEntriesNew);

            if (listBoxEntries.SelectedItem == null) ctxMenuEntries.Close();
            else {
                ContextEntry = (Entry)listBoxEntries.SelectedItem;
                if (ContextEntry.Parent == null)
                    ContextEntry.Parent = (History?)listBoxKeys.SelectedItem;
            }
        }

        private void ctxMenuEntriesNew_Click(object sender, EventArgs e) {
            if (ContextEntry != null)
                AddCustomEntry(ContextEntry, null);

            listBoxEntries.SelectedIndex = -1;
        }

        private void ctxMenuEntriesNote_Click(object sender, EventArgs e) {
            if (ContextEntry != null) {
                EditResult edit = EditWindow.Show(ContextEntry.Note, LANG.S("MAIN.CTX_EDIT_NOTE.TITLE") ?? "Note Editor", this);
                if (edit.Accept && !edit.Text.Equals(ContextEntry.Note, StringComparison.Ordinal)) {
                    ContextEntry.Note = edit.Text.Trim();
                    listBoxEntries.Refresh();
                    Export(ContextEntry.Parent);
                }
            }

            listBoxEntries.SelectedIndex = -1;
        }

        private void ctxMenuEntriesBackup_Click(object sender, EventArgs e) {
            if (ContextEntry != null)
                DSWebHook.Send(ContextEntry, true);

            listBoxEntries.SelectedIndex = -1;
        }

        private void ctxMenuEntriesDelete_Click(object sender, EventArgs e) {
            if (listBoxKeys.SelectedItem == null) {
                listBoxEntries.SelectedIndex = -1;
                return;
            }

            History h = (History)listBoxKeys.SelectedItem;
            if (ContextEntry != null) {
                DialogResult result = MessageBox.Show(
                    LANG.S("MAIN.CTX_DELETE_ENTRY.SUBTITLE", ContextEntry.ToString()) ?? $"Are you SURE that you want to delete this entry?\n\nDate: {ContextEntry.Timestamp}\nNote: {ContextEntry.Note}\n\nThis operation is not reversible!",
                    LANG.S("MAIN.CTX_DELETE_ENTRY.TITLE", ContextEntry.ToString()) ?? "Deleting Entry: " + ContextEntry.ToString(),
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.OK) {
                    h.Database.Remove(ContextEntry);
                    listBoxEntries.Items.Remove(ContextEntry);
                    Export(ContextEntry.Parent);
                }
            }

            listBoxEntries.SelectedIndex = -1;
        }
        #endregion
        #endregion

        #region Settings & Info
        private void btnSettings_Click(object? sender, EventArgs e) {
            SettingsWindow.Open(this);
        }

        private void linkSource_Clicked(object sender, EventArgs ev) {
            const string link = "https://github.com/ChrisFeline/ToNSaveManager/";
            OpenExternalLink(link);
        }

        private void linkWiki_Clicked(object sender, EventArgs e) {
            const string wiki = "https://terror.moe/";
            const string wikiJP = "https://www.ton-jp.com/";
            OpenExternalLink(LANG.SelectedKey == "ja-JP" ? wikiJP : wiki);
        }

        private void linkSupport_Click(object sender, EventArgs e) {
            const string support = "https://ko-fi.com/kittenji";
            OpenExternalLink(support);
        }

        private void btnObjectives_Click(object sender, EventArgs e) {
            ObjectivesWindow.Open(this);
        }

        private void btnStats_Click(object sender, EventArgs e) {
            StatsWindow.Open(this);
        }

        internal static void OpenExternalLink(string url) {
            if (string.IsNullOrEmpty(url)) return;

            ProcessStartInfo psInfo = new ProcessStartInfo { FileName = url, UseShellExecute = true };
            using (Process.Start(psInfo)) {
                Debug.WriteLine("Opening external link: " + url);
            }
        }
        #endregion

        #region Split Container
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e) {
            if (splitContainer1.CanFocus)
                splitContainer1.ActiveControl = listBoxEntries;

            listBoxKeys.Refresh();
            listBoxEntries.Refresh();
        }
        #endregion
        #endregion

        #region Form Methods
        #region Notifications
        static readonly XSOverlay XSOverlay = new XSOverlay();
        static readonly SoundPlayer CustomNotificationPlayer = new SoundPlayer();
        static readonly SoundPlayer DefaultNotificationPlayer = new SoundPlayer();
        static readonly Stream? DefaultAudioStream = // Get default notification in the embeded resources
            Program.GetEmbededResource("notification.wav");

        internal static void ResetNotification() {
            CustomNotificationPlayer.Stop();
            DefaultNotificationPlayer.Stop();
        }
        internal static void PlayNotification(bool forceDefault = false) {
            if ((!Started || !Settings.Get.PlayAudio) && !forceDefault) return;

            try {
                if (!forceDefault && !string.IsNullOrEmpty(Settings.Get.AudioLocation) && File.Exists(Settings.Get.AudioLocation)) {
                    CustomNotificationPlayer.SoundLocation = Settings.Get.AudioLocation;
                    CustomNotificationPlayer.Play();
                    return;
                }

                DefaultNotificationPlayer.Stream = DefaultAudioStream;
                DefaultNotificationPlayer.Play();
            } catch { }
        }
        internal static void SendXSNotification(bool test = false) {
            if (!Started || !Settings.Get.XSOverlay) return;
            const string message = "<color=#ff9999><b>ToN</b></color><color=grey>:</color> <color=#adff2f>Save Data Stored</color>";
            const string msgtest = "<color=#ff9999><b>ToN</b></color><color=grey>:</color> <color=#adff2f>Notifications Enabled</color>";

            if (test) XSOverlay.Send(LANG.S("SETTINGS.XSOVERLAY.TOGGLE") ?? msgtest, 1);
            else XSOverlay.Send(LANG.S("SETTINGS.XSOVERLAY.MESSAGE") ?? message);
        }
        #endregion

        internal static void RefreshLists() {
            Instance?.listBoxKeys.Refresh();
            Instance?.listBoxEntries.Refresh();
        }

        private void UpdateEntries() {
            listBoxEntries.Items.Clear();

            if (listBoxKeys.SelectedItem == null)
                return;

            History selected = (History)listBoxKeys.SelectedItem;
            SetTitle(selected.Name);

            foreach (Entry entry in selected.Database)
                listBoxEntries.Items.Add(entry);
        }

        private static void InsertSafe(ListBox list, int i, object value) =>
            list.Items.Insert(Math.Min(Math.Max(i, 0), list.Items.Count), value);

        internal static string GetTruncatedText(string text, Font font, int maxWidth) {
            Size textSize = TextRenderer.MeasureText(text, font);
            if (textSize.Width <= maxWidth) return text;

            int ellipsisWidth = TextRenderer.MeasureText("...", font).Width;
            while (textSize.Width + ellipsisWidth > maxWidth && text.Length > 0) {
                text = text.Substring(0, text.Length - 1);
                textSize = TextRenderer.MeasureText(text, font);
            }

            return text + "...";
        }

        internal void SetBackupButton(bool enabled) {
            ctxMenuEntriesBackup.Enabled = enabled;
        }
        #endregion

        #region Log Handling
        const string SaveInitKey = "saveInit";
        const string SaveStartKeyword = "[START]";
        const string SaveEndKeyword = "[END]";
        const string SaveInitKeyword = "[TERRORS SAVE CODE CREATED";

        const string ROUND_PARTICIPATION_KEY = "optedIn";
        const string ROUND_OPTIN_KEYWORD = "opted in";
        const string ROUND_OPTOUT_KEYWORD = "Player respawned";

        const string ROUND_RESULT_KEY = "rResult";
        const string ROUND_KILLERS_KEY = "rKillers";
        const string ROUND_WON_KEYWORD = "Player Won";
        const string ROUND_LOST_KEYWORD = "Player lost,";

        const string ROUND_IS_SABO_KEY = "rSabo";
        const string ROUND_SABO_END = "Clearing Items // Ran Item Removal";
        const string ROUND_IS_SABO = "You are the sussy baka of cringe naenae legend";

        const string KILLER_MATRIX_KEYWORD = "Killers have been set - ";
        const string KILLER_MATRIX_UNKNOWN = "Killers is unknown - ";
        const string KILLER_MATRIX_REVEAL = "Killers have been revealed - ";
        const string KILLER_ROUND_TYPE_KEYWORD = " // Round type is ";

        const string ROUND_MAP_KEY = "rMap";
        const string ROUND_MAP_LOCATION = "This round is taking place at ";
        const string ROUND_MAP_SWAPPED = "Solstice has swapped the map to ";

        // Encounters
        static ToNIndex.Terror[] EncounterList = ToNIndex.Instance.Encounters.Values.ToArray();

        private void LogWatcher_OnLine(object? sender, OnLineArgs e) {
            DateTime timestamp = e.Timestamp;
            LogContext context = e.Context;
            string line = e.Content.Substring(34);

            if (HandleSaveCode(line, timestamp, context) ||
                HandleTerrorIndex(line, timestamp, context) ||
                HandleStatCollection(line, timestamp, context)) { }
        }

        private void LogWatcher_OnTick(object? sender, EventArgs e) {
            CopyRecent();
            Export();
            StatsWindow.Export();
            LilOSC.SendData();
        }

        private bool HandleSaveCode(string line, DateTime timestamp, LogContext context) {
            int index = line.IndexOf(SaveInitKeyword);
            if (index > -1) {
                context.Set(SaveInitKey, true);
                return true;
            }

            if (!context.Get<bool>(SaveInitKey)) return false;

            index = line.IndexOf(SaveStartKeyword);
            if (index < 0) return false;

            index += SaveStartKeyword.Length;

            int end = line.IndexOf(SaveEndKeyword, index);
            if (end < 0) return false;
            end -= index;

            string save = line.Substring(index, end);
            if (string.IsNullOrEmpty(save)) {
                context.Set(SaveInitKey, false);
                return false;
            }

            AddLogEntry(context.DateKey, save, timestamp, context);
            context.Set(SaveInitKey, false);
            return true;
        }

        private bool HandleTerrorIndex(string line, DateTime timestamp, LogContext context) {
            // Handle participation
            bool isOptedIn = line.StartsWith(ROUND_OPTIN_KEYWORD);
            if (isOptedIn || line.StartsWith(ROUND_OPTOUT_KEYWORD)) {
                context.Set(ROUND_PARTICIPATION_KEY, isOptedIn);
                if (!isOptedIn) {
                    context.Rem(ROUND_KILLERS_KEY);
                    context.Rem(ROUND_RESULT_KEY);
                    context.Rem(ROUND_IS_SABO_KEY);
                    context.Rem(ROUND_MAP_KEY);
                }

                if (context.IsRecent) {
                    Debug.WriteLine("RECENT");

                    LilOSC.SetTerrorMatrix(TerrorMatrix.Empty);
                    LilOSC.SetMap();
                    LilOSC.SetOptInStatus(isOptedIn);
                    StatsWindow.SetRoundActive(false);
                }

                Debug.WriteLine("Opted In: " + isOptedIn);
                return true;
            } else {
                isOptedIn = context.Get<bool>(ROUND_PARTICIPATION_KEY);
            }

            if (!isOptedIn) return false;

            // Handle map location
            if (line.StartsWith(ROUND_MAP_LOCATION)) {
                int index = line.LastIndexOf('(') + 1;
                if (index <= 0) return true;

                int indexEnd = line.IndexOf(')', index);
                if (indexEnd < 0 || index >= indexEnd) return true;

                int length = (line.IndexOf(')', index) - index);

                string id_str = line.Substring(index, length);
                string name = line.Substring(ROUND_MAP_LOCATION.Length, index - ROUND_MAP_LOCATION.Length - 1).Trim();

                ToNIndex.Map map = ToNIndex.Instance.GetMap(name);
                if (map.IsEmpty && int.TryParse(id_str, out int mapIndex))
                    map = ToNIndex.Instance.GetMap(mapIndex);

                context.Set(ROUND_MAP_KEY, map);
                if (context.IsRecent) LilOSC.SetMap(map);

                if (map.Id == 68) { // RUN | The Meatball Man
                    TerrorMatrix terrorMatrix = new TerrorMatrix("RUN", 0, 0, 0);
                    context.Set(ROUND_KILLERS_KEY, terrorMatrix);
                    if (context.IsRecent) LilOSC.SetTerrorMatrix(terrorMatrix);
                }
                return true;
            }
            // Handle map swap
            if (line.StartsWith(ROUND_MAP_SWAPPED)) {
                string id_str = line.Substring(ROUND_MAP_SWAPPED.Length).Trim();
                if (int.TryParse(id_str, out int mapIndex)) {
                    ToNIndex.Map map = ToNIndex.Instance.GetMap(mapIndex);
                    Debug.WriteLine($"Map swapped to: {map} ({mapIndex})");
                    if (context.IsRecent) LilOSC.SetMap(map);
                }

                return true;
            }

            if (line.StartsWith(ROUND_IS_SABO)) {
                context.Set(ROUND_IS_SABO_KEY, true);
                if (context.IsRecent) LilOSC.SetTerrorMatrix(new TerrorMatrix() { IsSaboteur = true });
                return true;
            }

            // Track round participation results
            isOptedIn = line.StartsWith(ROUND_WON_KEYWORD);
            if (isOptedIn || line.StartsWith(ROUND_LOST_KEYWORD)) {
                context.Set(ROUND_RESULT_KEY, isOptedIn ? ToNRoundResult.W : ToNRoundResult.D);
                context.Rem(ROUND_IS_SABO_KEY);

                if (context.IsRecent) {
                    LilOSC.SetTerrorMatrix(TerrorMatrix.Empty);
                    LilOSC.SetMap();
                    StatsWindow.AddRound(isOptedIn);
                    StatsWindow.SetRoundActive(false);
                }
                return true;
            }

            bool isUnknown = line.StartsWith(KILLER_MATRIX_UNKNOWN); // Killers is unknown - 
            bool isRevealed = line.StartsWith(KILLER_MATRIX_REVEAL); // Killers have been revealed - 
            if (isUnknown || isRevealed || line.StartsWith(KILLER_MATRIX_KEYWORD)) { // Killers have been set - 
                int index  = isRevealed ? KILLER_MATRIX_REVEAL.Length : KILLER_MATRIX_KEYWORD.Length;
                int rndInd = line.IndexOf(KILLER_ROUND_TYPE_KEYWORD, index);
                if (rndInd < 0) return true;

                string roundType = line.Substring(rndInd + KILLER_ROUND_TYPE_KEYWORD.Length).Trim();
                int[] killerMatrix = new int[3];

                if (isUnknown) {
                    killerMatrix[0] = killerMatrix[1] = killerMatrix[2] = byte.MaxValue;
                } else {
                    string[] kMatrixRaw = line.Substring(index, rndInd - index).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    for (int i = 0; i < kMatrixRaw.Length; i++) {
                        killerMatrix[i] = int.TryParse(kMatrixRaw[i], out index) ? index : -1;
                    }
                }

                TerrorMatrix terrorMatrix = new TerrorMatrix(roundType, killerMatrix);
                terrorMatrix.IsSaboteur = context.Get<bool>(ROUND_IS_SABO_KEY);

                context.Set(ROUND_KILLERS_KEY, terrorMatrix);
                context.Rem(ROUND_IS_SABO_KEY);

                if (context.IsRecent) {
                    LilOSC.SetTerrorMatrix(terrorMatrix);
                    StatsWindow.SetRoundActive(true);
                }
                return true;
            }

            if (context.Get<bool>(ROUND_IS_SABO_KEY) && line.StartsWith("Clearing Items // Ran Item Removal")) {
                context.Rem(ROUND_IS_SABO_KEY);
                if (context.IsRecent) {
                    LilOSC.SetTerrorMatrix(TerrorMatrix.Empty);
                    StatsWindow.SetRoundActive(false);
                }
                return true;
            }

            if (context.HasKey(ROUND_KILLERS_KEY)) {
                for (int i = 0; i < EncounterList.Length; i++) {
                    ToNIndex.Terror encounter = EncounterList[i];
                    if (encounter.Keyword == null || !line.StartsWith(encounter.Keyword)) continue;

                    TerrorMatrix matrix = context.Get<TerrorMatrix>(ROUND_KILLERS_KEY);

                    if (encounter.Id == 4) { // GIGABYTES
                        Debug.WriteLine("Correcting Round Type to GIGABYTE.");
                        matrix.RoundType = ToNRoundType.GIGABYTE;
                        matrix.Terrors = [new(1, ToNIndex.TerrorGroup.Events)];
                        matrix.TerrorCount = 1;
                    } else {
                        matrix.AddEncounter(encounter.Id);
                    }

                    context.Set(ROUND_KILLERS_KEY, matrix);
                    if (context.IsRecent) LilOSC.SetTerrorMatrix(matrix);
                    return true;
                }
            }

            return false;
        }

        const string STAT_STUN_LANDED = " landed a stun!";
        const string STAT_STUN_TARGET = " was stunned.";
        const string STAT_HIT = "Hit - ";
        private bool HandleStatCollection(string line, DateTime timestamp, LogContext context) {
            if (!context.IsRecent) return false;

            if (line.Contains(LogWatcher.LocationKeyword)) {
                StatsWindow.ClearLobby();
                return true;
            }

            if (line.Contains(STAT_STUN_LANDED)) {
                StatsWindow.AddStun(true);
                return true;
            }

            if (line.Contains(STAT_STUN_TARGET)) {
                StatsWindow.AddStun(false);
                return true;
            }

            if (line.StartsWith(STAT_HIT)) {
                string ammount = line.Substring(STAT_HIT.Length).Trim();
                if (int.TryParse(ammount, out int result)) StatsWindow.AddDamage(result);
                return true;
            }

            return false;
        }
        #endregion

        #region Data
        private Entry? RecentData;

        private void Export(History? h = null, bool force = false) {
            if (h != null) h.SetDirty();
            SaveData.Export(force);
        }

        private void FirstImport() {
            History? temp = null;

            for (int i = 0; i < SaveData.Count; i++) {
                History h = SaveData[i];
                AddKey(h, i);
                if (h.IsCustom) continue;

                if (temp == null || temp.Timestamp < h.Timestamp) temp = h;
            }

            if (Settings.Get.AutoCopy) {
                Entry? first = temp?.Database.FirstOrDefault();
                if (first != null) SetRecent(first);
            }

            // CopyRecent();
        }

        private void AddCustomEntry(Entry entry, History? collection) {
            if (collection == null) {
                EditResult edit = EditWindow.Show(string.Empty, LANG.S("MAIN.CTX_RENAME.TITLE") ?? "Set Collection Name", this);
                if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text)) {
                    string title = edit.Text.Trim();
                    collection = new History(title, DateTime.Now);
                    AddKey(collection);
                } else return;
            } else collection.Timestamp = DateTime.Now; // Update edited timestamp

            int ind = collection.Add(entry);
            if (listBoxKeys.SelectedItem == collection)
                InsertSafe(listBoxEntries, ind, entry);

            Export(collection, true);
        }
        private void AddLogEntry(string dateKey, string content, DateTime timestamp, LogContext context) {
            History? collection = SaveData[dateKey];
            if (collection == null) {
                collection = new History(dateKey);
                collection.SetLogContext(context);
                AddKey(collection);
            }

            if (string.IsNullOrEmpty(context.DisplayName) && !string.IsNullOrEmpty(collection.DisplayName)) {
                context.DisplayName = collection.DisplayName;
            }

            int ind = collection.Add(content, timestamp, out Entry? entry);
            if (ind < 0) return; // Not added, duplicate

#pragma warning disable CS8604, CS8602 // Nullability is handled along with the return value of <History>.Add
            entry.PlayerCount = context.Players.Count;
            entry.Parent = collection;

            if (Settings.Get.SaveNames) entry.Players = context.GetRoomString();
            if (Settings.Get.SaveRoundInfo) {
                if (context.HasKey(ROUND_RESULT_KEY)) {
                    ToNRoundResult result = context.Get<ToNRoundResult>(ROUND_RESULT_KEY);
                    entry.RResult = result;
                    context.Rem(ROUND_RESULT_KEY);
                }

                if (context.HasKey(ROUND_KILLERS_KEY)) {
                    TerrorMatrix killers = context.Get<TerrorMatrix>(ROUND_KILLERS_KEY);
                    entry.RT = killers.RoundType;
                    entry.TD = killers.Terrors;

                    if (killers.Terrors.Length > 0) {
                        if (Settings.Get.SaveRoundNote)
                            entry.Note = string.Join(", ", killers.Terrors.Select(ToNIndex.Instance.GetTerror));
                    }

                    context.Rem(ROUND_KILLERS_KEY);
                }

                if (context.HasKey(ROUND_MAP_KEY)) {
                    var map = context.Get<ToNIndex.Map>(ROUND_MAP_KEY);
                    entry.MapID = map.IsEmpty ? -1 : map.Id;
                    context.Rem(ROUND_MAP_KEY);
                }
            }

            if (!context.IsHomeWorld) entry.Note = "(BETA) " + entry.Note;

            if (listBoxKeys.SelectedItem == collection)
                InsertSafe(listBoxEntries, ind, entry);

            SetRecent(entry);
#pragma warning restore CS8604, CS8602
            SaveData.SetDirty();

            if (context.Initialized) {
                PlayNotification();
                SendXSNotification();
                DSWebHook.Send(entry);
            }
        }

        private void AddKey(History collection, int i = -1) {
            if (i == -1) i = SaveData.Add(collection);
            listBoxKeys.Items.Insert(i, collection);
        }

        private void SetRecent(Entry entry) {
            if (entry == null) return;
            if (RecentData == null || RecentData.Timestamp < entry.Timestamp) {
                entry.Fresh = true;
                RecentData = entry;
            }
        }

        private void CopyRecent() {
            if (!Settings.Get.AutoCopy || RecentData == null || !RecentData.Fresh) return;

            RecentData.CopyToClipboard();
            RecentData.Fresh = false;
        }

        #endregion

    }
}