using BepInEx;
using BepInEx.Unity.IL2CPP;
using GTFO.API;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using SecDoorTerminalInterface.Inject;
using System.Linq;

namespace SecDoorTerminalInterface;
[BepInPlugin("SecDoorTerminalInterface", "SecDoorTerminalInterface", VersionInfo.Version)]
[BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
internal class EntryPoint : BasePlugin
{
    private Harmony _Harmony;

    public override void Load()
    {
        AssetAPI.OnAssetBundlesLoaded += AssetAPI_OnAssetBundlesLoaded;
        _Harmony = new Harmony("SecDoorTerminalInterface.Harmony");
        _Harmony.PatchAll();
    }

    private void AssetAPI_OnAssetBundlesLoaded()
    {
        Assets.Init();
    }
}
