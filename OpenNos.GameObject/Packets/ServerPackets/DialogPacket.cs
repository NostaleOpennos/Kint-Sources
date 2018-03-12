﻿//// <auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;

namespace OpenNos.GameObject
{
    [PacketHeader("dlg")]
    public class DialogPacket<TAnswerYesPacket, TAnswerNoPacket> : PacketDefinition
        where TAnswerYesPacket : PacketDefinition
        where TAnswerNoPacket : PacketDefinition
    {
        [PacketIndex(0, true)]
        public TAnswerYesPacket AnswerYesReturnPacket { get; set; }

        [PacketIndex(1, true)]
        public TAnswerNoPacket AnswerNoReturnPacket { get; set; }

        [PacketIndex(2, SerializeToEnd = true)]
        public string DialogText { get; set; }
    }
}