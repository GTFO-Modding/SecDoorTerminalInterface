using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SecDoorTerminalInterface;
public sealed partial class SecDoorTerminal
{
    private Vector3 _SavedIntOpenDoorPos;
    private Vector3 _SavedIntUseKeyPos;
    private Vector3 _SavedIntCustomMessagePos;
    private Vector3 _SavedIntHackPos;

    public void SetOpenInteractActive(bool active)
    {
        var interact = LinkedDoorLocks.m_intOpenDoor;
        interact.transform.position = active ? _SavedIntOpenDoorPos : BEGONE;
        interact.SetActive(active);
    }

    public void SetUseKeyInteractActive(bool active)
    {
        var interact = LinkedDoorLocks.m_intUseKeyItem;
        interact.transform.position = active ? _SavedIntUseKeyPos : BEGONE;
        interact.SetActive(active);
    }

    public void SetHackingInteractActive(bool active)
    {
        var interact = LinkedDoorLocks.m_intHack;
        interact.transform.position = active ? _SavedIntHackPos : BEGONE;
        interact.SetActive(active);
    }

    public void SetCustomMessageActive(bool active)
    {
        var interact = LinkedDoorLocks.m_intCustomMessage;
        interact.transform.position = active ? _SavedIntCustomMessagePos : BEGONE;
        interact.SetActive(active);
    }

    public void OpenOrStartChainPuzzle()
    {
        var cp = LinkedDoorLocks.ChainedPuzzleToSolve;
        if (cp != null && !cp.IsSolved)
        {
            LinkedDoor.m_sync.AttemptDoorInteraction(eDoorInteractionType.ActivateChainedPuzzle, 0f, 0f, default, null);
        }
        else
        {
            LinkedDoor.m_sync.AttemptDoorInteraction(eDoorInteractionType.Open, 0f, 0f, default, null);
        }
    }

    public void ForceOpenDoor()
    {
        LinkedDoor.m_sync.AttemptDoorInteraction(eDoorInteractionType.Open, 0f, 0f, default, null);
    }

    private void Setup_DoorInteractModule()
    {
        _SavedIntOpenDoorPos = LinkedDoorLocks.m_intOpenDoor.transform.position;
        _SavedIntUseKeyPos = LinkedDoorLocks.m_intUseKeyItem.transform.position;
        _SavedIntCustomMessagePos = LinkedDoorLocks.m_intCustomMessage.transform.position;
        _SavedIntHackPos = LinkedDoorLocks.m_intHack.transform.position;
    }
}
