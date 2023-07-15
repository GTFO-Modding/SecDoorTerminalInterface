using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecDoorTerminalInterface;
public struct CommandDescriptor
{
    public TERM_Command Type;
    public string Command;
    public string Description;
    public TERM_CommandRule Rule;
}
