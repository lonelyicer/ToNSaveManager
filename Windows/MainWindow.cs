using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Media;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using ToNSaveManager.Models;
using ToNSaveManager.Utils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ToNSaveManager
{
    public partial class MainWindow : Form
    {
        #region Initialization
        internal static readonly LogWatcher LogWatcher = new LogWatcher();
        internal static readonly AppSettings Settings = AppSettings.Import();
        internal static readonly SaveData SaveData = SaveData.Import();
        private bool Started;

        public MainWindow() =>
            InitializeComponent();
        #endregion

        #region Form Events

        #region Main Window
        private string OriginalTitle = string.Empty;
        public void SetTitle(string? title)
        {
            this.Text = string.IsNullOrEmpty(title) ? OriginalTitle : OriginalTitle + " | " + title;
        }

        private void mainWindow_Loaded(object sender, EventArgs e)
        {
            OriginalTitle = this.Text;
            this.Text = "Loading, please wait...";

            InitializeOptions();
            // TooltipUtil.Set(linkLabel1, "Source Code and Documentation for this tool can be found in my GitHub.\n" + SourceLink + "\n\nYou can also find me in discord as Kittenji.");
        }

        private void mainWindow_Shown(object sender, EventArgs e)
        {
            if (Started) return;

            FirstImport();

            LogWatcher.OnLine += LogWatcher_OnLine;
            LogWatcher.OnTick += LogWatcher_OnTick;
            LogWatcher.Start();

            Started = true;
            SetTitle(null);
        }
        #endregion

        #region ListBox Keys
        private void listBoxKeys_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                int index = listBoxKeys.IndexFromPoint(e.Location);
                if (index < 0 || index >= SaveData.Count) return;
                listBoxKeys.SelectedIndex = index;
            }
        }

        private void listBoxKeys_MouseUp(object sender, MouseEventArgs e)
        {
            bool isRight = e.Button == MouseButtons.Right;
            if (e.Button == MouseButtons.Left || isRight)
            {
                int index = listBoxKeys.SelectedIndex;
                if (index < 0)
                    return;

                if (isRight && index == listBoxKeys.IndexFromPoint(e.Location))
                    ctxMenuKeys.Show((ListBox)sender, e.Location);

                UpdateEntries();
            }
        }

        #region Context Menu | Keys
        private void ctxMenuKeysImport_Click(object sender, EventArgs e)
        {
            if (listBoxKeys.SelectedItem == null) return;
            History h = (History)listBoxKeys.SelectedItem;
            if (!h.IsCustom) return;

            EditResult edit = EditWindow.Show(string.Empty, "Import Code", this);
            if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text))
            {
                string content = edit.Text.Trim();
                AddCustomEntry(new Entry(content, DateTime.Now) { Note = "Imported" }, h);
                Export(true);
            }
        }

        private void ctxMenuKeysRename_Click(object sender, EventArgs e)
        {
            if (listBoxKeys.SelectedItem == null) return;
            History h = (History)listBoxKeys.SelectedItem;

            EditResult edit = EditWindow.Show(h.Name, "Set Collection Name", this);
            if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text))
            {
                string title = edit.Text.Trim();
                if (title == h.Name) return;

                h.Name = title;
                listBoxKeys.Refresh();
                SetTitle(title);
                Export(true);
            }
        }

        private void ctxMenuKeysDelete_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBoxKeys.SelectedIndex;
            if (selectedIndex != -1 && listBoxKeys.SelectedItem != null)
            {
                History h = (History)listBoxKeys.SelectedItem;
                DialogResult result = MessageBox.Show($"Are you SURE that you want to delete this entry?\n\nEvery code from '{h}' will be permanently deleted.\nThis operation is not reversible!", "Deleting Entry: " + h.ToString(), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    listBoxKeys.SelectedIndex = -1;
                    listBoxKeys.Items.Remove(h);
                    SaveData.Remove(h);
                    UpdateEntries();
                    SetTitle(null);
                    Export(true);
                }
            }
        }
        #endregion
        #endregion

        #region ListBox Entries
        private void listBoxEntries_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                int index = listBoxEntries.IndexFromPoint(e.Location);
                if (index < 0) return;
                listBoxEntries.SelectedIndex = index;
            }
        }

        private void listBoxEntries_MouseUp(object sender, MouseEventArgs e)
        {
            bool isRight = e.Button == MouseButtons.Right;
            if (e.Button == MouseButtons.Left || isRight)
            {
                int index = listBoxEntries.SelectedIndex;
                if (index < 0) return;

                if (isRight && index == listBoxEntries.IndexFromPoint(e.Location))
                {
                    ctxMenuEntries.Show((ListBox)sender, e.Location);
                    return;
                }

                if (listBoxEntries.SelectedItem != null)
                {
                    Entry entry = (Entry)listBoxEntries.SelectedItem;
                    entry.CopyToClipboard();
                    MessageBox.Show("Copied to clipboard!\n\nYou can now paste the code in game.", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                listBoxEntries.SelectedIndex = -1;
            }
        }

        private void listBoxEntries_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            e.DrawBackground();

            ListBox listBox = (ListBox)sender;
            string itemText = listBox.Items[e.Index].ToString() ?? string.Empty;

            int maxWidth = e.Bounds.Width;
            TextRenderer.DrawText(e.Graphics, GetTruncatedText(itemText, listBox.Font, maxWidth), listBox.Font, e.Bounds, e.ForeColor, TextFormatFlags.Left);

            e.DrawFocusRectangle();
        }

        private void listBoxEntries_Resize(object sender, EventArgs e)
        {
            listBoxEntries.Refresh();
        }

        // Tooltips
        int PreviousTooltipIndex = -1;
        private void listBoxEntries_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the index of the item under the mouse pointer
            int index = listBoxEntries.IndexFromPoint(e.Location);

            if (PreviousTooltipIndex != index)
            {
                PreviousTooltipIndex = index;

                if (index < 0)
                {
                    TooltipUtil.Set(listBoxEntries, null);
                    return;
                }

                Entry entry = (Entry)listBoxEntries.Items[index];
                TooltipUtil.Set(listBoxEntries, entry.GetTooltip(Settings.SaveNames));
            }
        }

        // Reset tooltip when mouse leaves the control.
        // This prevents accidental tooltip display when doing ALT+TAB.
        private void listBoxEntries_MouseLeave(object sender, EventArgs e)
        {
            if (PreviousTooltipIndex < 0) return;
            PreviousTooltipIndex = -1;
            TooltipUtil.Set(listBoxEntries, null);
        }

        #region Context Menu | Entries
        private Entry? ContextEntry;

        private void ctxMenuEntries_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            listBoxEntries.Enabled = true;
            if (e.CloseReason != ToolStripDropDownCloseReason.ItemClicked)
                listBoxEntries.SelectedIndex = -1;
        }

        private void ctxMenuEntries_Opened(object sender, EventArgs e)
        {
            listBoxEntries.Enabled = false;
            ctxMenuEntriesCopyTo.DropDownItems.Clear();

            // Might not be the most efficient way of doing this
            foreach (History h in SaveData.Collection)
            {
                if (!h.IsCustom) continue;

                ToolStripMenuItem item = new ToolStripMenuItem(h.Name);
                ctxMenuEntriesCopyTo.DropDownItems.Insert(0, item);
                item.Click += (o, e) =>
                {
                    if (ContextEntry != null)
                        AddCustomEntry(ContextEntry, h);
                };
            }

            ctxMenuEntriesCopyTo.DropDownItems.Add(ctxMenuEntriesNew);

            if (listBoxEntries.SelectedItem == null) ctxMenuEntries.Close();
            else ContextEntry = (Entry)listBoxEntries.SelectedItem;
        }

        private void ctxMenuEntriesNew_Click(object sender, EventArgs e)
        {
            if (ContextEntry != null)
                AddCustomEntry(ContextEntry, null);

            listBoxEntries.SelectedIndex = -1;
        }

        private void ctxMenuEntriesNote_Click(object sender, EventArgs e)
        {
            if (ContextEntry != null)
            {
                EditResult edit = EditWindow.Show(ContextEntry.Note, "Note Editor", this);
                if (edit.Accept && !edit.Text.Equals(ContextEntry.Note, StringComparison.Ordinal))
                {
                    ContextEntry.Note = edit.Text.Trim();
                    listBoxEntries.Refresh();
                    Export(true);
                }
            }

            listBoxEntries.SelectedIndex = -1;
        }

        private void ctxMenuEntriesDelete_Click(object sender, EventArgs e)
        {
            if (listBoxKeys.SelectedItem == null)
            {
                listBoxEntries.SelectedIndex = -1;
                return;
            }

            History h = (History)listBoxKeys.SelectedItem;
            if (ContextEntry != null)
            {
                DialogResult result = MessageBox.Show($"Are you SURE that you want to delete this entry?\n\nDate: {ContextEntry.Timestamp}\nNote: {ContextEntry.Note}\n\nThis operation is not reversible!", "Deleting Entry: " + h.ToString(), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    h.Entries.Remove(ContextEntry);
                    listBoxEntries.Items.Remove(ContextEntry);
                    Export(true);
                }
            }

            listBoxEntries.SelectedIndex = -1;
        }
        #endregion
        #endregion

        #region Options & Settings
        private void InitializeOptions()
        {
            ctxMenuSettingsAutoCopy.Checked = Settings.AutoCopy;
            ctxMenuSettingsNotifSounds.Checked = Settings.PlayAudio;
            ctxMenuSettingsCollectNames.Checked = Settings.SaveNames;
            ctxMenuSettingsSoundControl_Set();
        }

        private void optionsLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ctxMenuSettings.Show(Cursor.Position);
        }

        private void ctxMenuSettingsClose_Click(object sender, EventArgs e)
        {
            ctxMenuSettings.Close();
        }

        private void ctxMenuSettingsUpdate_Click(object sender, EventArgs e)
        {
            ctxMenuSettings.Close();
            Program.StartCheckForUpdate(true);
        }

        private void ctxMenuSettings_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked) e.Cancel = true;
        }

        private void ctxMenuSettingsAutoCopy_Click(object sender, EventArgs e)
        {
            Settings.AutoCopy = ToggleCheckState(Settings.AutoCopy, ctxMenuSettingsAutoCopy);
            Settings.Export();

            if (RecentData != null)
            {
                RecentData.Fresh = true;
                CopyRecent();
            }
        }

        private void ctxMenuSettingsSoundControl_Set()
        {
            ctxMenuSettingsClearSound.Enabled = !string.IsNullOrEmpty(Settings.AudioLocation);
            ctxMenuSettingsClearSound.ToolTipText = ctxMenuSettingsClearSound.Enabled ? "Custom Audio Path: " + Settings.AudioLocation : null;
        }

        private void ctxMenuSettingsNotifSounds_Click(object sender, EventArgs e)
        {
            Settings.PlayAudio = ToggleCheckState(Settings.PlayAudio, ctxMenuSettingsNotifSounds);
            Settings.Export();
            PlayNotification();
            if (!Settings.PlayAudio) ResetNotification();
        }

        private void ctxMenuSettingsSelectSound_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.InitialDirectory = "./";
                fileDialog.Title = "Select Custom Sound";
                fileDialog.Filter = "Waveform Audio (*.wav)|*.wav";

                if (fileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fileDialog.FileName))
                {
                    Settings.AudioLocation = fileDialog.FileName;
                    Settings.Export();
                    ctxMenuSettingsSoundControl_Set();
                }
            }
        }

        private void ctxMenuSettingsClearSound_Click(object sender, EventArgs e)
        {
            Settings.AudioLocation = null;
            Settings.Export();
            ctxMenuSettingsSoundControl_Set();
        }

        private void ctxMenuSettingsCollectNames_Click(object sender, EventArgs e)
        {
            Settings.SaveNames = ToggleCheckState(Settings.SaveNames, ctxMenuSettingsCollectNames);
            Settings.Export();
        }

        private bool ToggleCheckState(bool value, ToolStripMenuItem item)
        {
            value = !value;
            item.Checked = value;
            return value;
        }
        #endregion

        #region Extras & Info
        const string SourceLink = "https://github.com/ChrisFeline/ToNSaveManager/";
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs ev)
        {
            OpenExternalLink(SourceLink);
        }

        private void objectivesLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ObjectivesWindow.Open(this);
        }

        internal static void OpenExternalLink(string url)
        {
            ProcessStartInfo psInfo = new ProcessStartInfo { FileName = url, UseShellExecute = true };
            using (Process.Start(psInfo))
            {
                Debug.WriteLine("Opening external link: " + url);
            }
        }
        #endregion

        #endregion

        #region Form Methods
        #region Notifications
        static readonly SoundPlayer CustomNotificationPlayer = new SoundPlayer();
        static readonly SoundPlayer DefaultNotificationPlayer = new SoundPlayer();
        static readonly Stream? DefaultAudioStream = // Get default notification in the embeded resources
            Program.GetEmbededResource("notification.wav");

        private void ResetNotification()
        {
            CustomNotificationPlayer.Stop();
            DefaultNotificationPlayer.Stop();
        }
        private void PlayNotification()
        {
            if (!Settings.PlayAudio || !Started) return;

            try
            {
                if (!string.IsNullOrEmpty(Settings.AudioLocation) && File.Exists(Settings.AudioLocation))
                {
                    CustomNotificationPlayer.SoundLocation = Settings.AudioLocation;
                    CustomNotificationPlayer.Play();
                    return;
                }

                DefaultNotificationPlayer.Stream = DefaultAudioStream;
                DefaultNotificationPlayer.Play();
            }
            catch { }
        }
        #endregion

        private void UpdateEntries()
        {
            listBoxEntries.Items.Clear();

            if (listBoxKeys.SelectedItem == null)
                return;

            History selected = (History)listBoxKeys.SelectedItem;
            SetTitle(selected.Name);

            foreach (Entry entry in selected.Entries)
                listBoxEntries.Items.Add(entry);
        }

        private static void InsertSafe(ListBox list, int i, object value) =>
            list.Items.Insert(Math.Min(Math.Max(i, 0), list.Items.Count), value);

        internal static string GetTruncatedText(string text, Font font, int maxWidth)
        {
            Size textSize = TextRenderer.MeasureText(text, font);
            if (textSize.Width <= maxWidth) return text;

            int ellipsisWidth = TextRenderer.MeasureText("...", font).Width;
            while (textSize.Width + ellipsisWidth > maxWidth && text.Length > 0)
            {
                text = text.Substring(0, text.Length - 1);
                textSize = TextRenderer.MeasureText(text, font);
            }

            return text + "...";
        }
        #endregion

        #region Log Handling
        const string WorldNameKeyword = "Terrors of Nowhere";
        const string SaveStartKeyword = "  [START]";
        const string SaveEndKeyword = "[END]";

        private void LogWatcher_OnLine(object? sender, LogWatcher.OnLineArgs e)
        {
            string line = e.Content;
            DateTime timestamp = e.Timestamp;

            LogWatcher.LogContext context = e.Context;
            if (string.IsNullOrEmpty(context.DisplayName) ||
                string.IsNullOrEmpty(context.RoomName) ||
                !context.RoomName.Contains(WorldNameKeyword))
            {
                return;
            }

            int index = line.IndexOf(SaveStartKeyword, 32);
            if (index < 0) return;

            index += SaveStartKeyword.Length;

            int end = line.IndexOf(SaveEndKeyword, index);
            if (end < 0) return;
            end -= index;

            string save = line.Substring(index, end);
            string logName = context.FileName.Substring(11, 19);

            AddLogEntry(logName, save, timestamp, context);
        }

        private void LogWatcher_OnTick(object? sender, EventArgs e)
        {
            CopyRecent();
            Export();
        }
        #endregion

        #region Data
        private Entry? RecentData;

        private void Export(bool force = false) =>
            SaveData.Export(force);

        private void FirstImport()
        {
            for (int i = 0; i < SaveData.Count; i++)
            {
                AddKey(SaveData[i], i);
                if (SaveData[i].IsCustom) continue;

                // First should always be the most recent, hopefully
                Entry? first = SaveData[i].Entries.FirstOrDefault();
                if (first != null) SetRecent(first);
            }

            CopyRecent();
        }

        private void AddCustomEntry(Entry entry, History? collection)
        {
            if (collection == null)
            {
                EditResult edit = EditWindow.Show(string.Empty, "Set Collection Name", this);
                if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text))
                {
                    string title = edit.Text.Trim();
                    collection = new History(title, DateTime.Now);
                    AddKey(collection);
                }
                else return;
            }
            else collection.Timestamp = DateTime.Now; // Update edited timestamp

            int ind = collection.Add(entry);
            if (listBoxKeys.SelectedItem == collection)
                InsertSafe(listBoxEntries, ind, entry);

            Export(true);
        }
        private void AddLogEntry(string dateKey, string content, DateTime timestamp, LogWatcher.LogContext context)
        {
            History? collection = SaveData[dateKey];
            if (collection == null)
            {
                collection = new History(dateKey);
                AddKey(collection);
            }

            int ind = collection.Add(content, timestamp, out Entry? entry);
            if (ind < 0) return; // Not added, duplicate

#pragma warning disable CS8604, CS8602 // Nullability is handled along with the return value of <History>.Add
            if (Settings.SaveNames) entry.Players = context.GetRoomString();

            if (listBoxKeys.SelectedItem == collection)
                InsertSafe(listBoxEntries, ind, entry);

            SetRecent(entry);
#pragma warning restore CS8604, CS8602
            SaveData.SetDirty();

            PlayNotification();
        }

        private void AddKey(History collection, int i = -1)
        {
            if (i == -1) i = SaveData.Add(collection);
            listBoxKeys.Items.Insert(i, collection);
        }

        private void SetRecent(Entry entry)
        {
            if (entry == null) return;
            if (RecentData == null || RecentData.Timestamp < entry.Timestamp)
            {
                entry.Fresh = true;
                RecentData = entry;
            }
        }

        private void CopyRecent()
        {
            if (!Settings.AutoCopy || RecentData == null || !RecentData.Fresh) return;

            RecentData.CopyToClipboard();
            RecentData.Fresh = false;
        }
        #endregion

    }
}