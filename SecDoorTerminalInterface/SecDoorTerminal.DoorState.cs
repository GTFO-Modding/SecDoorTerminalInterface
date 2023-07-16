using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SecDoorTerminalInterface;
public sealed partial class SecDoorTerminal
{
    public CPSolvedBehaviour BioscanScanSolvedBehaviour { get; set; } = CPSolvedBehaviour.AddOpenCommand;


    private SDT_StateBehaviour _StateBehaviour;
    public SDT_StateBehaviour StateBehaviour
    {
        get
        {
            return _StateBehaviour;
        }
        set
        {
            _StateBehaviour?.SetContext(null);
            _StateBehaviour = value;
            _StateBehaviour?.SetContext(this);
        }
    }

    private void Setup_DoorStateModule()
    {
        StateBehaviour = new SDT_StateBehaviour();
        LinkedDoor.m_sync.add_OnDoorStateChange((Il2CppSystem.Action<pDoorState, bool>)OnStateChange);
        LinkedDoorLocks.add_OnChainedPuzzleSolved((Il2CppSystem.Action)OnChainedPuzzleSolved);
        OnStateChange(LinkedDoor.m_sync.GetCurrentSyncState(), isRecall: false);
    }

    private void OnChainedPuzzleSolved()
    {
        switch (BioscanScanSolvedBehaviour)
        {
            case CPSolvedBehaviour.OpenDoor:
                if (SNet.IsMaster)
                {
                    ForceOpenDoor();
                }
                break;

            case CPSolvedBehaviour.AddOpenCommand:
                CmdProcessor.AddOutput(TerminalLineType.Warning, $"Bioscan Sequence Completed - <color=orange>{OpenCommandName}</color> Command is now accessible!");
                AddOpenCommand(OpenCommandName, OpenCommandDescription);
                break;
        }
    }

    private void OnStateChange(pDoorState state, bool isRecall)
    {
        if (StateBehaviour == null)
            return;

        var data = LinkedDoor.ActiveEnemyWaveData;
        var isBloodyDoor = (data != null && data.HasActiveEnemyWave);
        var doorState = new SecDoorState()
        {
            Status = state.status,
            State = state,
            IsBloodyDoor = isBloodyDoor
        };
        StateBehaviour.UpdateInteractionState(doorState, isRecall);
        StateBehaviour.UpdateGraphicState(doorState, isRecall);
    }
}
