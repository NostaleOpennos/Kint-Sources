﻿//// <auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)
using OpenNos.Core;

namespace OpenNos.GameObject
{
    [PacketHeader("wear")]
    public class WearPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte InventorySlot { get; set; }

        [PacketIndex(1)]
        public byte Type { get; set; }

        #endregion
    }
}