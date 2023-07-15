using BepInEx;
using BepInEx.Unity.IL2CPP;
using GTFO.API;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using SecDoorTerminalInterface.Detour;
using System.Linq;

namespace SecDoorTerminalInterface;
[BepInPlugin("SecDoorTerminalInterface", "SecDoorTerminalInterface", VersionInfo.Version)]
[BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
internal class EntryPoint : BasePlugin
{
    public override void Load()
    {
        AssetAPI.OnAssetBundlesLoaded += AssetAPI_OnAssetBundlesLoaded;
        Detour_Terminal_ReceiveCmd.CreateDetour();
    }

    private void AssetAPI_OnAssetBundlesLoaded()
    {
        Assets.Init();
    }
}
