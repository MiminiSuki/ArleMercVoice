using BaseVoiceoverLib;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;


namespace ArleMercVoice
{
    public class ArleMercVoiceComponents : BaseVoiceoverComponent
    {
        //here is where we'll actually play our sounds
        //at the end of the BaseVoiceoverComponent class you can see the actions that can trigger voice lines
        //youre not limited to just those tho, you can try making new ones if you actually know what youre doing
        #region Creating voice strings
        public static NetworkSoundEventDef nseArleMercPrimary;
        public static NetworkSoundEventDef nseArleMercSecondary;

        public static NetworkSoundEventDef nseArleMercUtility1;
        public static NetworkSoundEventDef nseArleMercUtility2;
        public static NetworkSoundEventDef nseArleMercUtility3;

        public static NetworkSoundEventDef nseArleMercSpecial1;
        public static NetworkSoundEventDef nseArleMercSpecial2;
        public static NetworkSoundEventDef nseArleMercSpecial3;

        public static NetworkSoundEventDef nseArleMercChest1;
        public static NetworkSoundEventDef nseArleMercChest2;
        public static NetworkSoundEventDef nseArleMercChest3;

        public static NetworkSoundEventDef nseArleMercPrimaryENG;
        public static NetworkSoundEventDef nseArleMercSecondaryENG;

        public static NetworkSoundEventDef nseArleMercUtility1ENG;
        public static NetworkSoundEventDef nseArleMercUtility2ENG;
        public static NetworkSoundEventDef nseArleMercUtility3ENG;

        public static NetworkSoundEventDef nseArleMercSpecial1ENG;
        public static NetworkSoundEventDef nseArleMercSpecial2ENG;
        public static NetworkSoundEventDef nseArleMercSpecial3ENG;

        public static NetworkSoundEventDef nseArleMercChest1ENG;
        public static NetworkSoundEventDef nseArleMercChest2ENG;
        public static NetworkSoundEventDef nseArleMercChest3ENG;

        public static NetworkSoundEventDef nseArleMercPrimaryJP;
        public static NetworkSoundEventDef nseArleMercSecondaryJP;

        public static NetworkSoundEventDef nseArleMercUtility1JP;
        public static NetworkSoundEventDef nseArleMercUtility2JP;
        public static NetworkSoundEventDef nseArleMercUtility3JP;

        public static NetworkSoundEventDef nseArleMercSpecial1JP;
        public static NetworkSoundEventDef nseArleMercSpecial2JP;
        public static NetworkSoundEventDef nseArleMercSpecial3JP;

        public static NetworkSoundEventDef nseArleMercChest1JP;
        public static NetworkSoundEventDef nseArleMercChest2JP;
        public static NetworkSoundEventDef nseArleMercChest3JP;

        public static string voArleMercFallen1;
        public static string voArleMercFallen2;
        public static string voArleMercFallen3;

        public static string voArleMercAscention1;
        public static string voArleMercAscention2;
        public static string voArleMercAscention3;

        public static string voArleMercHurt;

        public static string voArleMercJoin1;
        public static string voArleMercJoin2;

        public static string voArleMercWeather1;
        public static string voArleMercWeather2;

        public static string voArleMercLowHP1;
        public static string voArleMercLowHP2;
        public static string voArleMercLowHP3;
        #endregion

        //these cooldowns are made so we can apply cooldowns for specific actions or situations
        //the cooldown field in TryPlayNetworkSound and TryPlaySound are global cooldowns that all other sounds played with those will respect*
        //*except the ones marked with forcePlay, like the death ones
        private float lowHealthCooldown = 0f;
        private float primaryCooldown = 0f;
        private float secondaryCooldown = 0f;
        private float itemGetCooldown = 0f;

        //if you look at the code, a bunch of times its used Util.CheckRoll
        //i separated most voicelines in sound events by themselves and im rolling to have them play randomly
        //this seems pointless because you can make sound events in wwise have random chances, so why do it in code?
        //this is because of the global cooldown of TryPlayNetworkSound and TryPlaySound
        //This way we can set the cooldown duration to the duration of that sound, so we dont have overlaps
        //in places where i dont use CheckRoll, i made random sound events in wwise, because i dont care if those overlap
        // /\ with the exception of the m1, i just made that random sound event have a random chance of playing at all
        //if youre going to put voicelines in the m1, i recommend you do something similar
        //not doing it leads to a lot of moaning, and not everyone likes that
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (lowHealthCooldown > 0f)
            {
                lowHealthCooldown -= Time.fixedDeltaTime;
            }
            if (primaryCooldown > 0f)
            {
                primaryCooldown -= Time.fixedDeltaTime;
            }
            if (secondaryCooldown > 0f)
            {
                secondaryCooldown -= Time.fixedDeltaTime;
            }
            if (itemGetCooldown > 0f)
            {
                itemGetCooldown -= Time.fixedDeltaTime;
            }
        }
        protected override void Start()
        {
            base.Start();
        }
        protected override void Awake()
        {
            base.Awake();
        }
        public override void PlayPrimaryAuthority(GenericSkill skill)
        {
            if (Util.CheckRoll(60f) && !(primaryCooldown > 0f))
            {
                TryPlayNetworkSound(nseArleMercPrimary, 0f, false);
                primaryCooldown = 0.8f;
            }
        }
        public override void PlaySecondaryAuthority(GenericSkill skill)
        {
            if (!(secondaryCooldown > 0f))
            {
                TryPlayNetworkSound(nseArleMercSecondary, 0f, false);
                secondaryCooldown = 1.7f;
            }
        }
        public override void PlayUtilityAuthority(GenericSkill skill)
        {
            if (Util.CheckRoll(33.333332f))
            {
                TryPlayNetworkSound(nseArleMercUtility1, 0.9f, false);
            }
            else if (Util.CheckRoll(50f))
            {
                TryPlayNetworkSound(nseArleMercUtility2, 0.8f, false);
            }
            else
            {
                TryPlayNetworkSound(nseArleMercUtility3, 0.8f, false);
            }
        }
        public override void PlaySpecialAuthority(GenericSkill skill)
        {
            if (Util.CheckRoll(33.333332f))
            {
                TryPlayNetworkSound(nseArleMercSpecial1, 2.1f, false);
            }
            else if (Util.CheckRoll(50f))
            {
                TryPlayNetworkSound(nseArleMercSpecial2, 2f, false);
            }
            else
            {
                TryPlayNetworkSound(nseArleMercSpecial3, 1.7f, false);
            }
        }
        public override void PlayDeath()
        {
            if (Util.CheckRoll(33.333332f))
            {
                TryPlaySound(voArleMercFallen1, 4f, true);
            }
            else if (Util.CheckRoll(50f))
            {
                TryPlaySound(voArleMercFallen2, 3.2f, true);
            }
            else
            {
                TryPlaySound(voArleMercFallen3, 4f, true);
            }
        }
        public override void PlayLevelUp()
        {
            if (Util.CheckRoll(33.333332f))
            {
                TryPlaySound(voArleMercAscention1, 4.9f, false);
            }
            else if (Util.CheckRoll(50f))
            {
                TryPlaySound(voArleMercAscention2, 8.2f, false);
            }
            else
            {
                TryPlaySound(voArleMercAscention3, 7.1f, false);
            }
        }
        public override void PlayHurt(float percentHPLost)
        {
            if (percentHPLost >= 0.1f)
            {
                TryPlaySound(voArleMercHurt, 0f, false);
            }
        }
        public override void PlayTeleporterStart()
        {
            if (Util.CheckRoll(50f))
            {
                TryPlaySound(voArleMercJoin1, 3f, false);
            }
            else
            {
                TryPlaySound(voArleMercJoin2, 2.9f, false);
            }
        }
        public override void PlayTeleporterFinish()
        {
            if (Util.CheckRoll(50f))
            {
                TryPlaySound(voArleMercWeather1, 4.2f, false);
            }
            else
            {
                TryPlaySound(voArleMercWeather2, 5f, false);
            }
        }
        public override void PlayVictory()
        {
            if (Util.CheckRoll(50f))
            {
                TryPlaySound(voArleMercWeather1, 4.2f, true);
            }
            else
            {
                TryPlaySound(voArleMercWeather2, 5f, true);
            }
        }
        public override void PlayLowHealth()
        {
            if (!(lowHealthCooldown > 0f))
            {
                if (Util.CheckRoll(33.333332f))
                {
                    TryPlaySound(voArleMercLowHP1, 2f, false);
                }
                else if (Util.CheckRoll(50f))
                {
                    TryPlaySound(voArleMercLowHP2, 2.2f, false);
                }
                else
                {
                    TryPlaySound(voArleMercLowHP3, 2.7f, false);
                }
                lowHealthCooldown = 40f;
            }
        }
        protected override void Inventory_onItemAddedClient(ItemIndex itemIndex)
        {
            base.Inventory_onItemAddedClient(itemIndex);
            PlayItemGet();
        }
        protected void PlayItemGet()
        {
            if (!(itemGetCooldown > 0f))
            {
                if (Util.CheckRoll(33.333332f))
                {
                    TryPlayNetworkSound(nseArleMercChest1, 3f, false);
                }
                else if (Util.CheckRoll(50f))
                {
                    TryPlayNetworkSound(nseArleMercChest2, 1.5f, false);
                }
                else
                {
                    TryPlayNetworkSound(nseArleMercChest3, 3.6f, false);
                }
                itemGetCooldown = 40f;
            }
        }
        public override bool ComponentEnableVoicelines()
        {
            return ArleMercVoicePlugin.enableVoicelines.Value;
        }
    }
}
