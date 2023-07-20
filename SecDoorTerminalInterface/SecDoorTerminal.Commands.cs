using LevelGeneration;
using Localization;
using Player;
using SecDoorTerminalInterface.Inject;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecDoorTerminalInterface;
public sealed partial class SecDoorTerminal
{
    public const TERM_Command COMMAND_OPEN = (TERM_Command)byte.MaxValue;
    public const TERM_Command COMMAND_OVERRIDE = (TERM_Command)(byte.MaxValue - 1);

    public event Action<TERM_Command, string, string, string> OnCmdUsed;

    public string OpenCommandName { get; set; } = "OPEN_LINKED";
    public string OpenCommandDescription { get; set; } = "Open the linked security door";

    private LocalizedText _GCTextHolder;

    private void Setup_CommandModule()
    {
        Inject_Terminal_ReceiveCmd.OnCmdUsed_LevelInstanced += OnReceiveCommand;
    }

    private void OnReceiveCommand(LG_ComputerTerminalCommandInterpreter interpreter, TERM_Command cmd, string inputLine, string param1, string param2)
    {
        if (interpreter.m_terminal.m_syncID == ComputerTerminal.m_syncID)
        {
            OnCmdUsed?.Invoke(cmd, inputLine, param1, param2);
        }
    }

    public void AddCommand(CommandDescriptor descriptor, Action<LG_ComputerTerminalCommandInterpreter> onCommandUsed = null)
    {
        if (CmdProcessor.HasRegisteredCommand(descriptor.Type))
        {
            Logger.Error($"Command Type: {descriptor.Type} is Already Added!!");
            return;
        }

        _GCTextHolder = new LocalizedText
        {
            UntranslatedText = descriptor.Description,
            Id = 0
        };
        CmdProcessor.AddCommand(descriptor.Type, descriptor.Command, _GCTextHolder, descriptor.Rule);
        OnCmdUsed += (cmdType, cmdStr, param1, param2) =>
        {
            if (cmdType != descriptor.Type)
                return;

            onCommandUsed?.Invoke(CmdProcessor);
        };
    }

    public void AddOverrideCommand(string cmd, string helpText, Action<LG_ComputerTerminalCommandInterpreter> onCommandUsed = null)
    {
        var cmdDescriptor = new CommandDescriptor()
        {
            Type = COMMAND_OVERRIDE,
            Command = cmd,
            Description = helpText,
            Rule = TERM_CommandRule.OnlyOnce
        };

        AddCommand(cmdDescriptor, (interpreter) =>
        {
            onCommandUsed?.Invoke(CmdProcessor);

            interpreter.AddOutput(TerminalLineType.Normal, "Desired Action: <color=orange>OVERRIDE</color>", 0.5f);
            interpreter.AddOutput(TerminalLineType.ProgressWait, "Decrpting Authorize ID..", 0.85f);
            interpreter.AddOutput(TerminalLineType.SpinningWaitDone, "Sending..", 1.2f);
            interpreter.AddOutput(TerminalLineType.Warning, "<color=orange>OVERRIDE</color> Action sent!", 0.65f);
            SetEndOfQueue(() =>
            {
                if (SNet.IsMaster)
                {
                    LinkedDoorLocks.m_intOpenDoor.OnInteractionTriggered.Invoke(PlayerManager.GetLocalPlayerAgent());
                }
            });
        });
    }

    public void AddOpenCommand(string cmd, string helpText, Action<LG_ComputerTerminalCommandInterpreter> onCommandUsed = null)
    {
        var cmdDescriptor = new CommandDescriptor()
        {
            Type = COMMAND_OPEN,
            Command = cmd,
            Description = helpText,
            Rule = TERM_CommandRule.OnlyOnce
        };

        AddCommand(cmdDescriptor, (interpreter) =>
        {
            onCommandUsed?.Invoke(interpreter);

            interpreter.AddOutput(TerminalLineType.Normal, "Desired Action: <color=orange>OPEN</color>", 0.5f);
            interpreter.AddOutput(TerminalLineType.ProgressWait, "Decrpting Authorize ID..", 0.85f);
            interpreter.AddOutput(TerminalLineType.SpinningWaitDone, "Sending..", 1.2f);
            interpreter.AddOutput(TerminalLineType.Warning, "<color=orange>OVERRIDE</color> Action sent!", 0.65f);
            SetEndOfQueue(() =>
            {
                if (SNet.IsMaster)
                {
                    LinkedDoorLocks.m_intOpenDoor.OnInteractionTriggered.Invoke(PlayerManager.GetLocalPlayerAgent());
                }
            });
        });
    }

    public void SetEndOfQueue(Action onEndOfQueue)
    {
        CmdProcessor.OnEndOfQueue = onEndOfQueue;
    }
}
