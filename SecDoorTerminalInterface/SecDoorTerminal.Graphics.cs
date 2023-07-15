using BepInEx.Unity.IL2CPP.Utils.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SecDoorTerminalInterface;
public sealed partial class SecDoorTerminal
{
    private void Setup_GraphicModule()
    {
        CoroutineManager.StartCoroutine(ForceScreenOffOnDeactive().WrapToIl2Cpp());
    }

    private IEnumerator ForceScreenOffOnDeactive()
    {
        while(true)
        {
            if (IsTerminalActive)
            {
                ComputerTerminal.m_text.enabled = true;
            }
            else
            {
                ComputerTerminal.m_text.enabled = false;
                ComputerTerminal.m_loginScreen.SetActive(true);
                ComputerTerminal.m_interfaceScreen.SetActive(true);
            }
            yield return null;
        }
    }

    public void SetIdleIconColor(Color color)
    {
        IdleIcon.color = color;
    }

    public void SetIdleText(string text)
    {
        IdleText.text = text;
    }

    public void SetIdleTextFontSize(float size)
    {
        IdleText.fontSize = size;
    }
}
