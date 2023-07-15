using AIGraph;
using GameData;
using LevelGeneration;
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
    private static readonly Vector3 BEGONE = Vector3.one * 10000.0f;

    [SuppressMessage("Type Safety", "UNT0014:Invalid type for call to GetComponent", Justification = "iTerminalItem is valid Interface")]
    public static SecDoorTerminal Place(LG_SecurityDoor secDoor, TerminalStartStateData startData = null, TerminalPlacementData placementData = null)
    {
        if (secDoor.m_securityDoorType != eSecurityDoorType.Security)
        {
            return null;
        }

        var tr = secDoor.transform.FindChildRecursive("InteractionInterface", exactMatch: true);
        if (tr == null)
            return null;

        tr.gameObject.SetActiveRecursively(false);
        tr.gameObject.SetActive(true);

        var terminalObject = UnityEngine.Object.Instantiate(Assets.SecDoorTerminalPrefab, tr);
        terminalObject.transform.localPosition = secDoor.Gate.Type == LG_GateType.Small ? new Vector3(0.0f, -0.006f, 0.0f) : new Vector3(0.0f, -0.026f, 0.0f);
        terminalObject.transform.localRotation = Quaternion.identity;
        terminalObject.transform.localScale = Vector3.one;

        var terminalItem = terminalObject.GetComponentInChildren<iTerminalItem>();
        if (terminalItem != null && TryGetSecDoorSpawnedNode(secDoor, out var spawnedNode))
        {
            terminalItem.SpawnNode = spawnedNode;
        }

        var lgTerminal = terminalObject.GetComponent<LG_ComputerTerminal>();
        lgTerminal.Setup(startData, placementData);

        MeshFilter[] filters = secDoor.GetComponentsInChildren<MeshFilter>(includeInactive: true);
        foreach (var filter in filters)
        {
            if (filter.sharedMesh == null)
                continue;

            var name = filter.sharedMesh.name;
            if (string.IsNullOrEmpty(name))
                continue;

            if (name.Equals("g_security_door_display"))
            {
                filter.gameObject.transform.localPosition += BEGONE;
            }
        }

        var secDoorTerminal = new SecDoorTerminal
        {
            LinkedDoor = secDoor,
            LinkedDoorLocks = secDoor.m_locks.Cast<LG_SecurityDoor_Locks>(),
            ComputerTerminal = lgTerminal,
            CmdProcessor = lgTerminal.m_command,
            Interaction = lgTerminal.GetComponentInChildren<Interact_ComputerTerminal>(includeInactive: true),
            IdleIcon = lgTerminal.m_loginScreen.GetComponent<SpriteRenderer>(),
            IdleText = lgTerminal.m_loginScreen.GetComponentInChildren<TextMeshPro>()
        };
        secDoorTerminal.Setup();
        
        return secDoorTerminal;
    }

    private static bool TryGetSecDoorSpawnedNode(LG_SecurityDoor secDoor, out AIG_CourseNode spawnedNode)
    {
        if (secDoor == null)
        {
            spawnedNode = null;
            return false;
        }

        if (secDoor.Gate == null)
        {
            spawnedNode = null;
            return false;
        }

        if (secDoor.Gate.m_linksFrom == null)
        {
            spawnedNode = null;
            return false;
        }

        spawnedNode = secDoor.Gate.m_linksFrom.m_courseNode;
        return spawnedNode != null;
    }
}
