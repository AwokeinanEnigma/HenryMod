using R2API;
using RoR2;
using System.Reflection;
using UnityEngine;

public class Assets
{
    //yeah no instance 
    public static AssetBundle mainAssetBundle = null;
    public static AssetBundleResourcesProvider provider;

    public static Material commandoMaterial;
    
    public Assets()
    {
        Log.LogI("Loading assets...");


        commandoMaterial = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

        // populate ASSETS
        if (mainAssetBundle == null)
        {
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Henry.henrybundle"))
            {
                mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                provider = new AssetBundleResourcesProvider("@IGiveEnigmaAllRightsToMyMod", mainAssetBundle);
                ResourcesAPI.AddProvider(provider);
            }
        }
    }
}
