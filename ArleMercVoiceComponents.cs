using BaseVoiceoverLib;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using AK.Wwise;
using System.Runtime.CompilerServices;


namespace ArleMercVoice
{
    public class ArleMercVoiceComponents : BaseVoiceoverComponent
    {
        //here is where we'll actually play our sounds
        //at the end of the BaseVoiceoverComponent class you can see the actions that can trigger voice lines
        //youre not limited to just those tho, you can try making new ones if you actually know what youre doing

        public static NetworkSoundEventDef nseArleMercPrimary;
        public static NetworkSoundEventDef nseArleMercSecondary;

        public static NetworkSoundEventDef nseUtility1;
        public static NetworkSoundEventDef nseUtility2;
        public static NetworkSoundEventDef nseUtility3;

        public static NetworkSoundEventDef nseSpecial1;
        public static NetworkSoundEventDef nseSpecial2;
        public static NetworkSoundEventDef nseSpecial3;

        public static NetworkSoundEventDef nseChest1;
        public static NetworkSoundEventDef nseChest2;
        public static NetworkSoundEventDef nseChest3;

        //these cooldowns are made so we can apply cooldowns for specific actions or situations
        //the cooldown field in TryPlayNetworkSound and TryPlaySound are global cooldowns that all other sounds played with those will respect*
        //*except the ones marked with forcePlay, like the death ones
        private float lowHealthCooldown = 0f;
        private float secondaryCooldown = 0f;
        private float itemGetCooldown = 0f;

        //if you look at the code, a bunch of times its used Util.CheckRoll
        //i separated most voicelines in sound events by themselves and im rolling to have them play randomly
        //this seems pointless because you can make sound events in wwise have random chances, so why do it in code?
        //this is because of the global cooldown of TryPlayNetworkSound and TryPlaySound
        //This way we can set the cooldown duration to the duration of that sound, so we dont have overlaps
        //in places where i dont use CheckRoll, i made random sound events in wwise, because i dont care if those overlap
        // /\ with the exception of the m1, i just made that random sound event have a random chance of playing at all
        //if youre going to put voicelines in the m1, i recommend you do something similar(like a primaryCooldown or something)
        //not doing it leads to a lot of moaning, and not everyone likes that
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (lowHealthCooldown > 0f)
            {
                lowHealthCooldown -= Time.fixedDeltaTime;
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
            if (Util.CheckRoll(33.333332f))
            {
                TryPlayNetworkSound(nseArleMercPrimary, 0f, false);
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
                TryPlayNetworkSound(nseUtility1, 0.9f, false);
            }
            else if (Util.CheckRoll(50f))
            {
                TryPlayNetworkSound(nseUtility2, 0.8f, false);
            }
            else
            {
                TryPlayNetworkSound(nseUtility3, 0.8f, false);
            }
        }
        public override void PlaySpecialAuthority(GenericSkill skill)
        {
            if (Util.CheckRoll(33.333332f))
            {
                TryPlayNetworkSound(nseSpecial1, 2.1f, false);
            }
            else if (Util.CheckRoll(50f))
            {
                TryPlayNetworkSound(nseSpecial2, 2f, false);
            }
            else
            {
                TryPlayNetworkSound(nseSpecial3, 1.7f, false);
            }
        }
        public override void PlayDeath()
        {
            if (Util.CheckRoll(33.333332f))
            {
                TryPlaySound("Play_ArleMerc_Fallen1", 4f, true);
            }
            else if (Util.CheckRoll(50f))
            {
                TryPlaySound("Play_ArleMerc_Fallen2", 3.2f, true);
            }
            else
            {
                TryPlaySound("Play_ArleMerc_Fallen3", 4f, true);
            }
        }
        public override void PlayLevelUp()
        {
            if (Util.CheckRoll(33.333332f))
            {
                TryPlaySound("Play_ArleMerc_Ascention1", 4.9f, false);
            }
            else if (Util.CheckRoll(50f))
            {
                TryPlaySound("Play_ArleMerc_Ascention2", 8.2f, false);
            }
            else
            {
                TryPlaySound("Play_ArleMerc_Ascention3", 7.1f, false);
            }
        }
        public override void PlayHurt(float percentHPLost)
        {
            if (percentHPLost >= 0.1f)
            {
                TryPlaySound("Play_ArleMerc_Hurt", 0f, false);
            }
        }
        public override void PlayTeleporterStart()
        {
            if (Util.CheckRoll(50f))
            {
                TryPlaySound("Play_ArleMerc_Join1", 3f, false);
            }
            else
            {
                TryPlaySound("Play_ArleMerc_Join2", 2.9f, false);
            }
        }
        public override void PlayTeleporterFinish()
        {
            if (Util.CheckRoll(50f))
            {
                TryPlaySound("Play_ArleMerc_Sun", 4.2f, false);
            }
            else
            {
                TryPlaySound("Play_ArleMerc_Thunder", 5f, false);
            }
        }
        public override void PlayVictory()
        {
            if (Util.CheckRoll(50f))
            {
                TryPlaySound("Play_ArleMerc_Sun", 4.2f, true);
            }
            else
            {
                TryPlaySound("Play_ArleMerc_Thunder", 5f, true);
            }
        }
        public override void PlayLowHealth()
        {
            if (!(lowHealthCooldown > 0f))
            {
                if (Util.CheckRoll(33.333332f))
                {
                    TryPlaySound("Play_ArleMerc_LowHP1", 2f, false);
                }
                else if (Util.CheckRoll(50f))
                {
                    TryPlaySound("Play_ArleMerc_LowHP2", 2.2f, false);
                }
                else
                {
                    TryPlaySound("Play_ArleMerc_LowHP3", 2.7f, false);
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
                    TryPlayNetworkSound(nseChest1, 3f, false);
                }
                else if (Util.CheckRoll(50f))
                {
                    TryPlayNetworkSound(nseChest2, 1.5f, false);
                }
                else
                {
                    TryPlayNetworkSound(nseChest3, 3.6f, false);
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
