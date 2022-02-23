using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LordAshes
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(LordAshes.FileAccessPlugin.Guid)]
    public partial class AudioPlugin : BaseUnityPlugin
    {
        // Plugin info
        public const string Name = "Audio Plug-In";              
        public const string Guid = "org.lordashes.plugins.audio";
        public const string Version = "1.0.0.0";                    

        // Configuration
        private ConfigEntry<KeyboardShortcut> triggerKey { get; set; }
        private static bool audioMenuOpen = false;
        private static Dictionary<string, SouceInfo> audioSource = new Dictionary<string, SouceInfo>();

        public class SouceInfo
        {
            public string url { get; set; }
            public bool loop { get; set; } = false;
            public float volume { get; set; } = 1.0f;
        }

        public class MenuConfiguration
        {
            private string _menuButtonTextureName;
            private int _menuFontSize;
            private UnityEngine.Color _menuFontColor;

            public string menuButtonTextureName { get { return _menuButtonTextureName; } set { _menuButtonTextureName = value; buttonTexture = FileAccessPlugin.Image.LoadTexture(value); } }
            public float menuButtonGap { get; set; }
            public int menuFontSize { get { return _menuFontSize; } set { _menuFontSize = value; labelStyle.fontSize = value; } }
            public UnityEngine.Color menuFontColor { get { return _menuFontColor; } set { _menuFontColor = value; labelStyle.normal.textColor = value; } }
            public Texture2D buttonTexture { get; private set; } = null;
            public GUIStyle labelStyle { get; private set; } = new GUIStyle() { alignment = TextAnchor.MiddleCenter};
        }

        public static MenuConfiguration menuConfig = new MenuConfiguration();

        void Awake()
        {
            UnityEngine.Debug.Log("Audio Plugin: Active.");

            triggerKey = Config.Bind("Hotkeys", "Open Audio Menu", new KeyboardShortcut(KeyCode.A, KeyCode.RightControl));

            menuConfig.menuButtonTextureName = Config.Bind("Menu", "Button Texture Name", AudioPlugin.Guid+".button.png").Value;
            menuConfig.menuButtonGap = Config.Bind("Menu", "Gap Between Buttons", 5f).Value;
            menuConfig.menuFontSize = Config.Bind("Menu", "Font Size", 32).Value;
            menuConfig.menuFontColor = Config.Bind("Menu", "Font Color", UnityEngine.Color.black).Value;

            string json = FileAccessPlugin.File.ReadAllText("AudioPlugin.json");
            audioSource = JsonConvert.DeserializeObject<Dictionary<string, SouceInfo>>(json);

            Utility.PostOnMainPage(this.GetType());
        }

        void Update()
        {
            if (Utility.StrictKeyCheck(triggerKey.Value))
            {
                audioMenuOpen = !audioMenuOpen;
            }
        }

        void OnGUI()
        {
            if(audioMenuOpen)
            {
                float offsetY = 60;
                foreach(KeyValuePair<string, SouceInfo> source in audioSource)
                {
                    if(GUI.Button(new Rect((Screen.width - menuConfig.buttonTexture.width) /2,offsetY, menuConfig.buttonTexture.width, menuConfig.buttonTexture.height), menuConfig.buttonTexture, GUIStyle.none))
                    {
                        audioMenuOpen = false;
                        StartCoroutine("PlayAudio", new object[] { source.Value });
                    }
                    GUI.Label(new Rect((Screen.width - menuConfig.buttonTexture.width) / 2, offsetY, menuConfig.buttonTexture.width, menuConfig.buttonTexture.height), source.Key, menuConfig.labelStyle);
                    offsetY += menuConfig.buttonTexture.height + menuConfig.menuButtonGap;
                }
                if (GUI.Button(new Rect((Screen.width - menuConfig.buttonTexture.width) / 2, offsetY, menuConfig.buttonTexture.width, menuConfig.buttonTexture.height), menuConfig.buttonTexture, GUIStyle.none))
                {
                    audioMenuOpen = false;
                    StartCoroutine("PlayAudio", new object[] { new SouceInfo() { url = "", loop = false, volume = 0.0f } });
                }
                GUI.Label(new Rect((Screen.width - menuConfig.buttonTexture.width) / 2, offsetY, menuConfig.buttonTexture.width, menuConfig.buttonTexture.height), "Stop Audio", menuConfig.labelStyle);
            }
        }

        private IEnumerator PlayAudio(object[] inputs)
        {
            GameObject speaker = GameObject.Find("AudioPluginSpeaker");
            if(speaker==null)
            {
                speaker = new GameObject();
                speaker.name = "AudioPluginSpeaker";
                speaker.AddComponent<AudioSource>();
            }
            AudioSource source = speaker.GetComponent<AudioSource>();
            source.playOnAwake = false;
            source.Stop();
            SouceInfo audioInfo = (SouceInfo)inputs[0];
            if (audioInfo.url.Trim() != "")
            {
                Debug.Log("Audio Plugin: Loading '" + audioInfo.url + "'... (Loop=" + audioInfo.loop + ")");
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioInfo.url, AudioType.UNKNOWN))
                {
                    yield return www.SendWebRequest();

                    if (www.result == UnityWebRequest.Result.ConnectionError)
                    {
                        Debug.Log("Audio Plugin: Failure To Load '" + audioInfo.url + "'...");
                        Debug.Log(www.error);
                    }
                    else
                    {
                        source.clip = DownloadHandlerAudioClip.GetContent(www);
                        source.loop = audioInfo.loop;
                        source.volume = audioInfo.volume;
                        Debug.Log("Audio Plugin: Playing '" + audioInfo.url + "'...");
                        source.Play();
                        if (source.loop != true)
                        {
                            yield return new WaitForSeconds(source.clip.length);
                            Debug.Log("Audio Plugin: Stopping '" + audioInfo.url + "'...");
                            source.Stop();
                        }
                    }
                }
            }
        }
    }
}
