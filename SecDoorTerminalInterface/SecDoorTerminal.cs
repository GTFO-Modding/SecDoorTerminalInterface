using AIGraph;
using GameData;
using GTFO.API;
using LevelGeneration;
using Localization;
using SecDoorTerminalInterface.Detour;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SecDoorTerminalInterface;
public sealed partial class SecDoorTerminal
{
    public LG_SecurityDoor LinkedDoor { get; private set; }
    public LG_SecurityDoor_Locks LinkedDoorLocks { get; private set; }
    public LG_ComputerTerminal ComputerTerminal { get; private set; }
    public LG_ComputerTerminalCommandInterpreter CmdProcessor { get; private set; }
    public Interact_ComputerTerminal Interaction { get; private set; }
    public TextMeshPro IdleText { get; private set; }
    public SpriteRenderer IdleIcon { get; private set; }

    public AIG_CourseNode SpawnNode => ComputerTerminal.m_terminalItem.SpawnNode;
    public bool IsTerminalActive { get; private set; } = true;

    private void Setup()
    {
        Setup_GraphicModule();
        Setup_DoorInteractModule();
        Setup_DoorStateModule();
        Setup_CommandModule();
    }

    public void SetTerminalActive(bool active)
    {
        IsTerminalActive = active;

        ComputerTerminal.enabled = active;

        var interaction = ComputerTerminal.GetComponentInChildren<Interact_ComputerTerminal>(includeInactive: true);
        if (interaction != null)
        {
            interaction.enabled = active;
            interaction.SetActive(active);
        }

        if (!active)
        {
            var interactionSource = ComputerTerminal.m_localInteractionSource;
            if (interactionSource != null && interactionSource.FPItemHolder.InTerminalTrigger)
            {
                ComputerTerminal.ExitFPSView();
            }

            if (SNet.IsMaster)
            {
                ComputerTerminal.ChangeState(TERM_State.Sleeping);
            }
        }
    }

    public void SetSpawnNode(AIG_CourseNode node)
    {
        ComputerTerminal.m_terminalItem.SpawnNode = node;
    }

    public void SetLocationTextToSpawnZone()
    {
        SetLocationText(SpawnNode.m_zone.NavInfo.GetFormattedText(LG_NavInfoFormat.Full_And_Number_With_Underscore));
    }

    public void SetLocationText(string text)
    {
        ComputerTerminal.m_terminalItem.FloorItemLocation = text;
    }

    private SecDoorTerminal()
    {

    }
}
