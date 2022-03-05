using BepInEx;
using Bounce.BlobAssets;
using Bounce.TaleSpire.AssetManagement;
using Bounce.Unmanaged;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TaleSpire.Atmosphere;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HolloFox
{
    public partial class AudioPlugin : BaseUnityPlugin
    {
        [HarmonyPatch(typeof(AssetDb), "OnSetupInternals")]
        public class OnSetupInternalsPatches
        {
            static void Postfix()
            {
                Debug.Log("Audio Plugin: Registering Audio From Collected Audio Library");
                foreach (MusicData.MusicKind kind in Enum.GetValues(typeof(MusicData.MusicKind)))
                {
                    foreach (var file in AudioPlugin.Audio[kind.ToString()])
                    {
                        if (AssetDb.Music.TryGetValue(file.Key, out _)) continue;
                        var builder = new BlobBuilder(Allocator.Persistent);
                        ref var root = ref builder.ConstructRoot<MusicData>();
                        MusicData.Construct(builder, ref root, file.Key, file.Key, file.Value.name, file.Value.name, new string[] { }, "", file.Value.name, kind);
                        var x = builder.CreateBlobAssetReference<MusicData>(Allocator.Persistent);
                        AssetDb.Music.TryAdd(file.Key, x.TakeView());
                    }
                }
            }
        }

        [HarmonyPatch(typeof(AtmosphereManager.LoadedAudioClip), "Load")]
        public class LoadedAudioClipLoadPatch
        {
            static bool Prefix(System.Action<AtmosphereManager.LoadedAudioClip, AudioClip> ClipLoaded,
                ref AudioClip ____clip,
                ref AtmosphereManager.LoadedAudioClip __instance
            )
            {
                bool found = false;
                foreach (Dictionary<NGuid, AudioPlugin.AudioData> audioSource in AudioPlugin.Audio.Values)
                {
                    if (audioSource.ContainsKey(__instance.GUID)) { found = true; break; }
                }
                if ((!found) || (____clip != null))
                {
                    Debug.Log("Audio Plugin: Core Audio Requested");
                    return true;
                }
                Debug.Log("Audio Plugin: Custom Audio Requested");
                AudioPlugin.LoadAudioCallback(new object[] { __instance, ____clip, ClipLoaded });
                return false;
            }
        }
    }
}
