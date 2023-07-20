using GTFO.API;
using HarmonyLib;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecDoorTerminalInterface.Inject;
[HarmonyPatch(typeof(LG_ComputerTerminalCommandInterpreter), nameof(LG_ComputerTerminalCommandInterpreter.ReceiveCommand))]
internal class Inject_Terminal_ReceiveCmd
{
    public static event Action<LG_ComputerTerminalCommandInterpreter, TERM_Command, string, string, string> OnCmdUsed_LevelInstanced;

    static Inject_Terminal_ReceiveCmd()
    {
        LevelAPI.OnLevelCleanup += () =>
        {
            OnCmdUsed_LevelInstanced = null;
        };
    }

    private static void Prefix(LG_ComputerTerminalCommandInterpreter __instance, TERM_Command cmd, string inputLine, string param1, string param2)
    {
        OnCmdUsed_LevelInstanced?.Invoke(__instance, cmd, inputLine, param1, param2);
    }
}
