using System;
using System.Collections.Generic;
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
                    //System.Console.WriteLine("ArleMerc: voicelines OFF");
                    DisableSound();
                }
                else
                {
                    //System.Console.WriteLine("ArleMerc: voicelines ON");
                    EnableSound();
                }
                if (jpVoicelines.Value)
                {
                    //System.Console.WriteLine("ArleMerc: voicelines are JP");
                    DefineJPVO();
                }
                else
                {
                    //System.Console.WriteLine("ArleMerc: voicelines are ENG");
                    DefineVO();
                }
            }
        }

        public static ConfigEntry<bool> enableVoicelines;
        public static ConfigEntry<bool> jpVoicelines;
        public static SurvivorDef survivorDef = Addressables.LoadAssetAsync<SurvivorDef>("RoR2/Base/Merc/Merc.asset").WaitForCompletion();
        public static List<NSEInfo> nseList = new List<NSEInfo>();

        private static string voArleMercLobby = null;
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
            enableVoicelines.SettingChanged += Voicelines_SettingChanged;
            jpVoicelines = ((BaseUnityPlugin)this).Config.Bind<bool>(new ConfigDefinition("Settings", "Use JP Voicelines"), false, new ConfigDescription("Switches to the Japonese voicelines when using the Arlecchino Mercenary Skin.", (AcceptableValueBase)null, Array.Empty<object>()));
            jpVoicelines.SettingChanged += Voicelines_SettingChanged;
            if (Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                RiskOfOptionsCompat();
            }
        }
        private void Voicelines_SettingChanged(object sender, EventArgs e)
        {
            RefreshNSE();
        }
        private void RiskOfOptionsCompat()
        {
            //you can add an assetbundle to this to have and icon for the mod in riskofoptions menu, but im lazy and didnt do that
            ModSettingsManager.AddOption((BaseOption)new CheckBoxOption(enableVoicelines));
            ModSettingsManager.AddOption((BaseOption)new CheckBoxOption(jpVoicelines));
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
                Util.PlaySound(voArleMercLobby, mannequinObject);
            }
        }
        private void InitNSE()
        {
            #region Register ENG NSEs
            ArleMercVoiceComponents.nseArleMercPrimaryENG = RegisterNSE("Play_ArleMerc_Primary");
            ArleMercVoiceComponents.nseArleMercSecondaryENG = RegisterNSE("Play_ArleMerc_Secondary");

            ArleMercVoiceComponents.nseArleMercUtility1ENG = RegisterNSE("Play_ArleMerc_Skill1");
            ArleMercVoiceComponents.nseArleMercUtility2ENG = RegisterNSE("Play_ArleMerc_Skill2");
            ArleMercVoiceComponents.nseArleMercUtility3ENG = RegisterNSE("Play_ArleMerc_Skill3");

            ArleMercVoiceComponents.nseArleMercSpecial1ENG = RegisterNSE("Play_ArleMerc_Burst1");
            ArleMercVoiceComponents.nseArleMercSpecial2ENG = RegisterNSE("Play_ArleMerc_Burst2");
            ArleMercVoiceComponents.nseArleMercSpecial3ENG = RegisterNSE("Play_ArleMerc_Burst3");

            ArleMercVoiceComponents.nseArleMercChest1ENG = RegisterNSE("Play_ArleMerc_Chest1");
            ArleMercVoiceComponents.nseArleMercChest2ENG = RegisterNSE("Play_ArleMerc_Chest2");
            ArleMercVoiceComponents.nseArleMercChest3ENG = RegisterNSE("Play_ArleMerc_Chest3");
            #endregion

            #region Register JP NSEs
            ArleMercVoiceComponents.nseArleMercPrimaryJP = RegisterNSE("Play_ArleMerc_Primary_JP");
            ArleMercVoiceComponents.nseArleMercSecondaryJP = RegisterNSE("Play_ArleMerc_Secondary_JP");

            ArleMercVoiceComponents.nseArleMercUtility1JP = RegisterNSE("Play_ArleMerc_Skill1_JP");
            ArleMercVoiceComponents.nseArleMercUtility2JP = RegisterNSE("Play_ArleMerc_Skill2_JP");
            ArleMercVoiceComponents.nseArleMercUtility3JP = RegisterNSE("Play_ArleMerc_Skill3_JP");

            ArleMercVoiceComponents.nseArleMercSpecial1JP = RegisterNSE("Play_ArleMerc_Burst1_JP");
            ArleMercVoiceComponents.nseArleMercSpecial2JP = RegisterNSE("Play_ArleMerc_Burst2_JP");
            ArleMercVoiceComponents.nseArleMercSpecial3JP = RegisterNSE("Play_ArleMerc_Burst3_JP");

            ArleMercVoiceComponents.nseArleMercChest1JP = RegisterNSE("Play_ArleMerc_Chest1_JP");
            ArleMercVoiceComponents.nseArleMercChest2JP = RegisterNSE("Play_ArleMerc_Chest2_JP");
            ArleMercVoiceComponents.nseArleMercChest3JP = RegisterNSE("Play_ArleMerc_Chest3_JP");
            #endregion
        }
        public void RefreshNSE()
        {
            foreach (NSEInfo nse in nseList)
            {
                //System.Console.WriteLine("ArleMerc: Validating Params...");
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
        private static void DefineVO()
        {
            ArleMercVoiceComponents.nseArleMercPrimary = ArleMercVoiceComponents.nseArleMercPrimaryENG;
            ArleMercVoiceComponents.nseArleMercSecondary = ArleMercVoiceComponents.nseArleMercSecondaryENG;

            ArleMercVoiceComponents.nseArleMercUtility1 = ArleMercVoiceComponents.nseArleMercUtility1ENG;
            ArleMercVoiceComponents.nseArleMercUtility2 = ArleMercVoiceComponents.nseArleMercUtility2ENG;
            ArleMercVoiceComponents.nseArleMercUtility3 = ArleMercVoiceComponents.nseArleMercUtility3ENG;

            ArleMercVoiceComponents.nseArleMercSpecial1 = ArleMercVoiceComponents.nseArleMercSpecial1ENG;
            ArleMercVoiceComponents.nseArleMercSpecial2 = ArleMercVoiceComponents.nseArleMercSpecial2ENG;
            ArleMercVoiceComponents.nseArleMercSpecial3 = ArleMercVoiceComponents.nseArleMercSpecial3ENG;

            ArleMercVoiceComponents.nseArleMercChest1 = ArleMercVoiceComponents.nseArleMercChest1ENG;
            ArleMercVoiceComponents.nseArleMercChest2 = ArleMercVoiceComponents.nseArleMercChest2ENG;
            ArleMercVoiceComponents.nseArleMercChest3 = ArleMercVoiceComponents.nseArleMercChest3ENG;


            ArleMercVoiceComponents.voArleMercFallen1 = "Play_ArleMerc_Fallen1";
            ArleMercVoiceComponents.voArleMercFallen2 = "Play_ArleMerc_Fallen2";
            ArleMercVoiceComponents.voArleMercFallen3 = "Play_ArleMerc_Fallen3";

            ArleMercVoiceComponents.voArleMercAscention1 = "Play_ArleMerc_Ascention1";
            ArleMercVoiceComponents.voArleMercAscention2 = "Play_ArleMerc_Ascention2";
            ArleMercVoiceComponents.voArleMercAscention3 = "Play_ArleMerc_Ascention3";

            ArleMercVoiceComponents.voArleMercHurt = "Play_ArleMerc_Hurt";
            voArleMercLobby = "Play_ArleMerc_Lobby";

            ArleMercVoiceComponents.voArleMercJoin1 = "Play_ArleMerc_Join1";
            ArleMercVoiceComponents.voArleMercJoin2 = "Play_ArleMerc_Join2";

            ArleMercVoiceComponents.voArleMercWeather1 = "Play_ArleMerc_Sun";
            ArleMercVoiceComponents.voArleMercWeather2 = "Play_ArleMerc_Thunder";

            ArleMercVoiceComponents.voArleMercLowHP1 = "Play_ArleMerc_LowHP1";
            ArleMercVoiceComponents.voArleMercLowHP2 = "Play_ArleMerc_LowHP2";
            ArleMercVoiceComponents.voArleMercLowHP3 = "Play_ArleMerc_LowHP3";
        }
        private static void DefineJPVO()
        {
            ArleMercVoiceComponents.nseArleMercPrimary = ArleMercVoiceComponents.nseArleMercPrimaryJP;
            ArleMercVoiceComponents.nseArleMercSecondary = ArleMercVoiceComponents.nseArleMercSecondaryJP;

            ArleMercVoiceComponents.nseArleMercUtility1 = ArleMercVoiceComponents.nseArleMercUtility1JP;
            ArleMercVoiceComponents.nseArleMercUtility2 = ArleMercVoiceComponents.nseArleMercUtility2JP;
            ArleMercVoiceComponents.nseArleMercUtility3 = ArleMercVoiceComponents.nseArleMercUtility3JP;

            ArleMercVoiceComponents.nseArleMercSpecial1 = ArleMercVoiceComponents.nseArleMercSpecial1JP;
            ArleMercVoiceComponents.nseArleMercSpecial2 = ArleMercVoiceComponents.nseArleMercSpecial2JP;
            ArleMercVoiceComponents.nseArleMercSpecial3 = ArleMercVoiceComponents.nseArleMercSpecial3JP;

            ArleMercVoiceComponents.nseArleMercChest1 = ArleMercVoiceComponents.nseArleMercChest1JP;
            ArleMercVoiceComponents.nseArleMercChest2 = ArleMercVoiceComponents.nseArleMercChest2JP;
            ArleMercVoiceComponents.nseArleMercChest3 = ArleMercVoiceComponents.nseArleMercChest3JP;


            ArleMercVoiceComponents.voArleMercFallen1 = "Play_ArleMerc_Fallen1_JP";
            ArleMercVoiceComponents.voArleMercFallen2 = "Play_ArleMerc_Fallen2_JP";
            ArleMercVoiceComponents.voArleMercFallen3 = "Play_ArleMerc_Fallen3_JP";

            ArleMercVoiceComponents.voArleMercAscention1 = "Play_ArleMerc_Ascention1_JP";
            ArleMercVoiceComponents.voArleMercAscention2 = "Play_ArleMerc_Ascention2_JP";
            ArleMercVoiceComponents.voArleMercAscention3 = "Play_ArleMerc_Ascention3_JP";

            ArleMercVoiceComponents.voArleMercHurt = "Play_ArleMerc_Hurt_JP";
            voArleMercLobby = "Play_ArleMerc_Lobby_JP";

            ArleMercVoiceComponents.voArleMercJoin1 = "Play_ArleMerc_Join1_JP";
            ArleMercVoiceComponents.voArleMercJoin2 = "Play_ArleMerc_Join2_JP";

            ArleMercVoiceComponents.voArleMercWeather1 = "Play_ArleMerc_Sun_JP";
            ArleMercVoiceComponents.voArleMercWeather2 = "Play_ArleMerc_Thunder_JP";

            ArleMercVoiceComponents.voArleMercLowHP1 = "Play_ArleMerc_LowHP1_JP";
            ArleMercVoiceComponents.voArleMercLowHP2 = "Play_ArleMerc_LowHP2_JP";
            ArleMercVoiceComponents.voArleMercLowHP3 = "Play_ArleMerc_LowHP3_JP";
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
