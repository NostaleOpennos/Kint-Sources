﻿//// <auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Lvl", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChangeLevelPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Level { get; set; }

        public static string ReturnHelp() => "$Lvl <Level>";

        #endregion
    }
}