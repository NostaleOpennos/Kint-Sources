﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$Maintenance", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]

    public class MaintenancePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Delay { get; set; }

        [PacketIndex(1)]
        public int Duration { get; set; }

        [PacketIndex(2, SerializeToEnd = true)]
        public string Reason { get; set; }

        public static string ReturnHelp()
        {
            return "$Maintenance DELAY(MIN) DURATION(MIN) REASON";
        }

        #endregion
    }
}
