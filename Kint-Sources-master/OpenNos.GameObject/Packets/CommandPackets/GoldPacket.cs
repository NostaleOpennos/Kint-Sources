﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Gold", PassNonParseablePacket = true, Authority = AuthorityType.Operator)]
    public class GoldPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public long Amount { get; set; }

        public static string ReturnHelp()
        {
            return "$Gold AMOUNT";
        }

        #endregion
    }
}