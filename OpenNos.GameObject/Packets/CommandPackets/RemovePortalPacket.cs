﻿//// <auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$RemovePortal", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class RemovePortalPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$RemovePortal";
    }
}