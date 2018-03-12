﻿//// <auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Effect", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class EffectCommandPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int EffectId { get; set; }

        public static string ReturnHelp() => "$Effect <Id>";

        #endregion
    }
}