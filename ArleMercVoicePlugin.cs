using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using BaseVoiceoverLib;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static BaseVoiceoverLib.VoiceoverInfo;
using HG;

namespace ArleMercVoice
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.Moffein.BaseVoiceoverLib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]

    [R2API.Utils.NetworkCompatibility(R2API.Utils.CompatibilityLevel.EveryoneMustHaveMod, R2API.Utils.VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin("com.miminisusuki.ArleMercVoice", "ArleMercVoice", "1.0.0")]
    public class ArleMercVoicePlugin : BaseUnityPlugin
    {
        //honestly most of this code is stolen from the HiyoriMerc and SaoriBandit skin mods by VanillaVitamins
        public class NSEInfo
        {
            public NetworkSoundEventDef nse;

            public uint akId = 0u;

            public string eventName = string.Empty;

            public NSEInfo(NetworkSoundEventDef source)
            {
                nse = source;
                akId = source.akId;
                eventName = source.eventName;
            }

            private void DisableSound()
            {
                nse.akId = 0u;
                nse.eventName = string.Empty;
            }

            private void EnableSound()
            {
                nse.akId = akId;
                nse.eventName = eventName;
            }

            public void ValidateParams()
            {
                if (akId == 0)
                {
                    akId = nse.akId;
                }
                if (eventName == string.Empty)
                {
                    eventName = nse.eventName;
                }
                if (!enableVoicelines.Value)
                {
                    DisableSound();
                }
                else
                {
                    EnableSound();
                }
            }
        }

        public static ConfigEntry<bool> enableVoicelines;
        public static SurvivorDef survivorDef = Addressables.LoadAssetAsync<SurvivorDef>("RoR2/Base/Merc/Merc.asset").WaitForCompletion();
        public static List<NSEInfo> nseList = new List<NSEInfo>();
        public void Awake()
        {
            Files.PluginInfo = ((BaseUnityPlugin)this).Info;
            RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(OnLoad));
            new Content().Initialize();
            SoundBanks.Init();
            InitNSE();

            //create settings and them create those settings in riskofoptions as well
            //you can add an assetbundle to this to have and icon for the mod in riskofoptions menu, but im lazy and didnt do that
            enableVoicelines = ((BaseUnityPlugin)this).Config.Bind<bool>(new ConfigDefinition("Settings", "Enable Voicelines"), true, new ConfigDescription("Enable voicelines when using the Arlecchino Mercenary Skin.", (AcceptableValueBase)null, Array.Empty<object>()));
            enableVoicelines.SettingChanged += EnableVoicelines_SettingChanged;
            if (Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                RiskOfOptionsCompat();
            }
        }
        private void EnableVoicelines_SettingChanged(object sender, EventArgs e)
        {
            RefreshNSE();
        }
        private void RiskOfOptionsCompat()
        {
            //you can add an assetbundle to this to have and icon for the mod in riskofoptions menu, but im lazy and didnt do that
            ModSettingsManager.AddOption((BaseOption)new CheckBoxOption(enableVoicelines));
        }
        private void OnLoad()
        {
            //this part sounds like a crime but it works so yknow
            SkinDef skinDef = null;
            SkinDef[] array = ArrayUtils.Clone(FindSkinsForBody(BodyCatalog.FindBodyIndex("MercBody")));
            //SkinDef[] array = FindSkinsForBody(BodyCatalog.FindBodyIndex("MercBody")));
            foreach (SkinDef skinDef2 in array)
            {
                if (skinDef2.name == "ArleMercSkinRed")
                {
                    skinDef = skinDef2;
                    break;
                }
            }
            if (!skinDef)
            {
                Debug.LogError("ArleMercVoice: Arlecchino Mercenary SkinDef not found. Voicelines will not work!");
            }
            else
            {
                //the voiceoverInfo is what attaches the components.cs to the characters, and in there we'll tell the game what sounds to play and when
                VoiceoverInfo val = new VoiceoverInfo(typeof(ArleMercVoiceComponents), skinDef, "MercBody");
                val.selectActions = (LobbySelectActions)Delegate.Combine((Delegate)(object)val.selectActions, (Delegate)new LobbySelectActions(ArleSelect));
            }
            RefreshNSE();
        }
        private void ArleSelect(GameObject mannequinObject)
        {
            if (!enableVoicelines.Value)
            {
                return;
            }
            else
            {
                Util.PlaySound("Play_ArleMerc_Lobby", mannequinObject);
            }
        }
        private void InitNSE()
        {
            //some sounds need to be networked, so we need to register them first before actually using them
            //do it here so it does it every time you boot up the game and change the config(because of EnableVoicelines_SettingChanged and RefreshNSE)
            ArleMercVoiceComponents.nseArleMercPrimary = RegisterNSE("Play_ArleMerc_Primary");
            ArleMercVoiceComponents.nseArleMercSecondary = RegisterNSE("Play_ArleMerc_Secondary");

            ArleMercVoiceComponents.nseUtility1 = RegisterNSE("Play_ArleMerc_Skill1");
            ArleMercVoiceComponents.nseUtility2 = RegisterNSE("Play_ArleMerc_Skill2");
            ArleMercVoiceComponents.nseUtility3 = RegisterNSE("Play_ArleMerc_Skill3");

            ArleMercVoiceComponents.nseSpecial1 = RegisterNSE("Play_ArleMerc_Burst1");
            ArleMercVoiceComponents.nseSpecial2 = RegisterNSE("Play_ArleMerc_Burst2");
            ArleMercVoiceComponents.nseSpecial3 = RegisterNSE("Play_ArleMerc_Burst3");

            ArleMercVoiceComponents.nseChest1 = RegisterNSE("Play_ArleMerc_Chest1");
            ArleMercVoiceComponents.nseChest2 = RegisterNSE("Play_ArleMerc_Chest2");
            ArleMercVoiceComponents.nseChest3 = RegisterNSE("Play_ArleMerc_Chest3");
        }
        public void RefreshNSE()
        {
            foreach (NSEInfo nse in nseList)
            {
                nse.ValidateParams();
            }
        }
        private NetworkSoundEventDef RegisterNSE(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.eventName = eventName;
            Content.networkSoundEventDefs.Add(networkSoundEventDef);
            nseList.Add(new NSEInfo(networkSoundEventDef));
            return networkSoundEventDef;
        }


        //like there has to be a better way to this
        private static SkinDef[] FindSkinsForBody(BodyIndex bodyIndex)
        {
            ModelLocator component = BodyCatalog.GetBodyPrefab(bodyIndex).GetComponent<ModelLocator>();
            if (!component || !component.modelTransform)
            {
                return Array.Empty<SkinDef>();
            }

            ModelSkinController component2 = component.modelTransform.GetComponent<ModelSkinController>();
            if (!component2)
            {
                return Array.Empty<SkinDef>();
            }

            return component2.skins;
        }
    }
}
