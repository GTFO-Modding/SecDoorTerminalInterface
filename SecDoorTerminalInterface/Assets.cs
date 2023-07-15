using GTFO.API;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SecDoorTerminalInterface;
internal static class Assets
{
    public static GameObject SecDoorTerminalPrefab;

    public static void Init()
    {
        SecDoorTerminalPrefab = AssetAPI.GetLoadedAsset<GameObject>("Assets/Modding/SecDoorTerminal/Terminal_SecDoor.prefab");
    }
}
