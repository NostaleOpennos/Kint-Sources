﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Shutdown", PassNonParseablePacket = true, Authority = AuthorityType.Operator)]
    public class ShutdownPacket : PacketDefinition
    {
        public static string ReturnHelp()
        {
            return "$Shutdown";
        }
    }
}