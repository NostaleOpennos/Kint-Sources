﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$MapDance", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class MapDancePacket : PacketDefinition
    {
        public static string ReturnHelp()
        {
            return "$MapDance";
        }
    }
}