﻿using Newtonsoft.Json;

namespace ToNSaveManager.Models
{
    internal class AppSettings
    {
        const string LegacyDestination = "settings.json";
        static string Destination = "Settings.json";

        private bool IsDirty;

        [JsonIgnore] private bool m_AutoCopy = false;
        [JsonIgnore] private bool m_SaveNames = false;
        [JsonIgnore] private bool m_PlayAudio = false;
        [JsonIgnore] private string? m_AudioLocation = null;
        [JsonIgnore] private string? m_IgnoreRelease = null;

        /// <summary>
        /// Automatically copy newly detected save codes as you play.
        /// </summary>
        public bool AutoCopy
        {
            get { return m_AutoCopy; }
            set
            {
                if (value == m_AutoCopy) return;
                m_AutoCopy = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Play notification audio when a new save is detected.
        /// </summary>
        public bool PlayAudio
        {
            get { return m_PlayAudio; }
            set
            {
                if (value == m_PlayAudio) return;
                m_PlayAudio = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Custom audio location, must be .wav
        /// </summary>
        public string? AudioLocation
        {
            get { return m_AudioLocation; }
            set
            {
                if (value == m_AudioLocation) return;
                m_AudioLocation = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Saves a list of players that were in the same room as you at the time of the save.
        /// </summary>
        public bool SaveNames
        {
            get { return m_SaveNames; }
            set
            {
                if (value == m_SaveNames) return;
                m_SaveNames = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Stores a github release tag if the player clicks no when asking for update.
        /// </summary>
        public string? IgnoreRelease
        {
            get { return m_IgnoreRelease; }
            set
            {
                if (value == m_IgnoreRelease) return;
                m_IgnoreRelease = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Import a settings instance from the local json file
        /// </summary>
        /// <returns>Deserialized Settings object, or else Default Settings object.</returns>
        public static AppSettings Import()
        {
            string destination = Path.Combine(Program.DataLocation, Destination);
            AppSettings? settings;

            try
            {
                if (File.Exists(LegacyDestination))
                    File.Move(LegacyDestination, Destination);

                if (File.Exists(Destination) && !File.Exists(destination))
                {
                    File.Copy(Destination, destination, true);
                    File.Move(Destination, Destination + ".old");
                }

                Destination = destination;
                if (!File.Exists(Destination)) return new AppSettings();

                string content = File.ReadAllText(Destination);
                settings = JsonConvert.DeserializeObject<AppSettings>(content);
            }
            catch
            {
                settings = null;
            }

            return settings ?? new AppSettings();
        }

        public void Export()
        {
            if (!IsDirty) return;

            try
            {
                string json = JsonConvert.SerializeObject(this);
                File.WriteAllText(Destination, json);
            }
            catch (Exception e)
            {
                MessageBox.Show("An error ocurred while trying to write your settings to a file.\n\nMake sure that the program contains permissions to write files in the current folder it's located at.\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            IsDirty = false;
        }
    }
}