using BepInEx.Unity.IL2CPP.Hook;
using GTFO.API;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecDoorTerminalInterface.Detour;
internal unsafe static class Detour_Terminal_ReceiveCmd
{
    public static event Action<LG_ComputerTerminalCommandInterpreter, TERM_Command, string, string, string> OnCmdUsed_LevelInstanced;
    private delegate void ReceiveCommandDel(IntPtr instance, byte cmd, IntPtr inputStr, IntPtr param1, IntPtr param2, Il2CppMethodInfo* methodInfo);

    private static INativeDetour _Detour;
    private static ReceiveCommandDel _Original;

    public static void CreateDetour()
    {
        LevelAPI.OnLevelCleanup += () =>
        {
            OnCmdUsed_LevelInstanced = null;
        };

        var methodPtr = (nint)Il2CppAPI.GetIl2CppMethod<LG_ComputerTerminalCommandInterpreter>(
            nameof(LG_ComputerTerminalCommandInterpreter.ReceiveCommand),
            GetNameOf(typeof(void)),
            false,
            new string[] {
                GetNameOf(typeof(TERM_Command)),
                GetNameOf(typeof(string)),
                GetNameOf(typeof(string)),
                GetNameOf(typeof(string))
            });

        if (methodPtr != 0)
        {
            _Detour = INativeDetour.CreateAndApply(methodPtr, Detour, out _Original);
        }
    }

    private static void Detour(IntPtr instance, byte cmdType, IntPtr inputStr, IntPtr param1, IntPtr param2, Il2CppMethodInfo* methodInfo)
    {
        OnCmdUsed_LevelInstanced?.Invoke(new LG_ComputerTerminalCommandInterpreter(instance),
            (TERM_Command)cmdType,
            IL2CPP.Il2CppStringToManaged(inputStr),
            IL2CPP.Il2CppStringToManaged(param1),
            IL2CPP.Il2CppStringToManaged(param2));
        _Original(instance, cmdType, inputStr, param1, param2, methodInfo);
    }

    private static string GetNameOf(Type type)
    {
        return Il2CppType.From(type).FullName;
    }
}
