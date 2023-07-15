using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SecDoorTerminalInterface;
public class SDT_StateBehaviour
{
    internal void SetContext(SecDoorTerminal secDoorTerminal)
    {
        Context = secDoorTerminal;
    }

    public SecDoorTerminal Context { get; private set; }

    public virtual void UpdateInteractionState(SecDoorState state, bool isRecall)
    {
        switch (state.Status)
        {
            case eDoorStatus.None:
            case eDoorStatus.Closed:
            case eDoorStatus.Closed_BrokenCantOpen:
            case eDoorStatus.Closed_LockedWithChainedPuzzle_Alarm:
            case eDoorStatus.Closed_LockedWithChainedPuzzle:
            case eDoorStatus.Closed_LockedWithNoKey:
            case eDoorStatus.TryOpenStuckBroken:
            case eDoorStatus.TryOpenStuckInGlue:
            case eDoorStatus.GluedMax:
            case eDoorStatus.Unlocked:
                //Enable Terminal
                //Interaction All Off
                Context.SetTerminalActive(true);

                Context.SetOpenInteractActive(false);
                Context.SetHackingInteractActive(false);
                Context.SetUseKeyInteractActive(false);
                Context.SetCustomMessageActive(false);
                break;

            case eDoorStatus.Closed_LockedWithKeyItem:
                //Disable Terminal
                //Key Interaction ON
                Context.SetTerminalActive(false);

                Context.SetOpenInteractActive(false);
                Context.SetHackingInteractActive(false);
                Context.SetUseKeyInteractActive(true);
                Context.SetCustomMessageActive(false);
                break;

            case eDoorStatus.Closed_LockedWithPowerGenerator:
            case eDoorStatus.Closed_LockedWithBulkheadDC:
                //Disable Terminal
                //Custom Message On
                Context.SetTerminalActive(false);

                Context.SetOpenInteractActive(false);
                Context.SetHackingInteractActive(false);
                Context.SetUseKeyInteractActive(false);
                Context.SetCustomMessageActive(true);
                break;

            case eDoorStatus.Open:
            case eDoorStatus.Opening:
            case eDoorStatus.Destroyed:
            case eDoorStatus.ChainedPuzzleActivated:
                //Disable Terminal
                //All Interaction Off
                Context.SetTerminalActive(false);

                Context.SetOpenInteractActive(false);
                Context.SetHackingInteractActive(false);
                Context.SetUseKeyInteractActive(false);
                Context.SetCustomMessageActive(false);
                break;
        }
    }

    public virtual void UpdateGraphicState(SecDoorState state, bool isRecall)
    {
        string text;
        Color? color;
        switch (state.Status)
        {
            case eDoorStatus.Closed_LockedWithBulkheadDC:
                text = "::<color=orange>Bulkhead Override</color> Required::";
                color = Color.yellow;
                break;

            case eDoorStatus.Closed_LockedWithKeyItem:
                text = "::<color=orange>Keycard</color> Required::";
                color = Color.red;
                break;

            case eDoorStatus.Closed_LockedWithChainedPuzzle:
                text = "<color=orange>BIOSCAN Protocol</color> pending...";
                color = Color.cyan;
                break;

            case eDoorStatus.Closed_LockedWithChainedPuzzle_Alarm:
                text = $"<color=orange>BIOSCAN Protocol</color> pending...";
                color = Color.red;
                break;

            case eDoorStatus.Closed_LockedWithPowerGenerator:
                text = $"BOOT ERR://Power Level - <color=red>LOW</color>";
                color = ColorExt.Hex("#FFA500");
                break;

            case eDoorStatus.Closed_LockedWithNoKey:
                text = $"<color=orange>LOCKDOWN Protocol</color> engaged!";
                color = Color.red;
                break;

            case eDoorStatus.ChainedPuzzleActivated:
                if (Context.LinkedDoorLocks.ChainedPuzzleToSolve.Data.TriggerAlarmOnActivate)
                {
                    text = "<color=red>BIOSCAN Protocol</color> processing!";
                    color = Color.red;
                }
                else
                {
                    text = "<color=blue>BIOSCAN Protocol</color> processing!";
                    color = Color.cyan;
                }
                break;

            case eDoorStatus.Open:
            case eDoorStatus.Opening:
                text = "::DOOR OVERRIDING::";
                color = Color.cyan;
                break;

            case eDoorStatus.Closed:
                text = "::DOOR LOCKING::";
                color = Color.cyan;
                break;

            default:
                text = "Waiting...";
                color = Color.green;
                break;
        }

        if (color.HasValue)
        {
            Context.SetIdleIconColor(color.Value);
        }

        if (text != null)
        {
            Context.SetIdleText(text);
        }
    }
}

public struct SecDoorState
{
    public eDoorStatus Status;
    public pDoorState State;
    public bool IsBloodyDoor;
}