﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$MapPVP", PassNonParseablePacket = true, Authority = AuthorityType.Operator)]
    public class MapPVPPacket : PacketDefinition
    {
        public static string ReturnHelp()
        {
            return "$MapPVP";
        }
    }
}