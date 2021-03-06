﻿using System;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Reference;
using Iso7816;

namespace GlobalPlatform.NET.Commands
{
    public interface IChannelOperationPicker
    {
        IApduBuilder OpenChannel();

        IChannelPicker CloseChannel();
    }

    public interface IChannelPicker
    {
        IApduBuilder WithIdentifier(byte identifier);
    }

    /// <summary>
    /// TThe MANAGE CHANNEL command is processed by the OPEN on cards that are aware of logical
    /// channels. It is used to open and close Supplementary Logical Channels. The Basic Logical
    /// Channel (channel number zero) can never be closed.
    /// <para> Based on section 11.7 of the v2.3 GlobalPlatform Card Specification. </para>
    /// </summary>
    public class ManageChannelCommand : CommandBase<ManageChannelCommand, IChannelOperationPicker>,
        IChannelOperationPicker,
        IChannelPicker
    {
        public IApduBuilder OpenChannel()
        {
            this.operation = Operation.Open;

            return this;
        }

        public IChannelPicker CloseChannel()
        {
            this.operation = Operation.Close;

            return this;
        }

        public IApduBuilder WithIdentifier(byte identifier)
        {
            if (identifier > 19)
            {
                throw new ArgumentException("Identifier must be between 0-19 (inclusive).", nameof(identifier));
            }

            this.identifier = identifier;

            return this;
        }

        public override CommandApdu AsApdu()
        {
            this.P1 = (byte)this.operation;
            this.P2 = this.operation == Operation.Close ? this.identifier : (byte)0x00;

            switch (this.operation)
            {
                case Operation.Open:
                    return CommandApdu.Case2S(ApduClass.GlobalPlatform, ApduInstruction.ManageChannel, this.P1, this.P2, 0x01);

                case Operation.Close:
                    return CommandApdu.Case1(ApduClass.GlobalPlatform, ApduInstruction.ManageChannel, this.P1, this.P2);

                default:
                    throw new NotSupportedException("Operation not supported.");
            }
        }

        private enum Operation : byte
        {
            Open = 0x00,
            Close = 0x80
        }

        private Operation operation;
        private byte identifier;
    }
}
