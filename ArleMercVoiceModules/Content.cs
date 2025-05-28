using System.Collections;
using System.Collections.Generic;
using RoR2;
using RoR2.ContentManagement;

namespace ArleMercVoice
{
    public class Content : IContentPackProvider
    {
        internal ContentPack contentPack = new ContentPack();

        public static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();
        public string identifier => "com.miminisusuki.ArleMercVoice";
        public void Initialize()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            contentPack.networkSoundEventDefs.Add(networkSoundEventDefs.ToArray());
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}
