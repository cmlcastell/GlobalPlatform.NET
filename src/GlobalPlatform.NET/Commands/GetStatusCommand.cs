﻿using System.Collections.Generic;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.Tools;
using Iso7816;

namespace GlobalPlatform.NET.Commands
{
    public enum GetStatusScope : byte
    {
        IssuerSecurityDomain = 0b10000000,
        Applications = 0b01000000,
        ExecutableLoadFiles = 0b00100000,
        ExecutableLoadFilesAndModules = 0b00010000
    }

    public interface IGetStatusScopePicker
    {
        IGetStatusApplicationFilter GetStatusOf(GetStatusScope scope);
    }

    public interface IGetStatusApplicationFilter : IGetStatusOccurrencePicker
    {
        IGetStatusOccurrencePicker WithFilter(byte[] applicationFilter);
    }

    public interface IGetStatusOccurrencePicker : IApduBuilder
    {
        IApduBuilder ReturnFirstOrAllOccurrences();

        IApduBuilder ReturnNextOccurrence();
    }

    /// <summary>
    /// The GET STATUS command is used to retrieve Issuer Security Domain, Executable Load File,
    /// Executable Module, Application or Security Domain Life Cycle status information according to
    /// a given match/search criteria.
    /// <para> Based on section 11.4 of the v2.3 GlobalPlatform Card Specification. </para>
    /// </summary>
    public class GetStatusCommand : CommandBase<GetStatusCommand, IGetStatusScopePicker>,
        IGetStatusScopePicker,
        IGetStatusApplicationFilter
    {
        private byte[] applicationFilter = new byte[0];

        public enum Tag : byte
        {
            ApplicationAID = 0x4F
        }

        public IGetStatusApplicationFilter GetStatusOf(GetStatusScope scope)
        {
            this.P1 = (byte)scope;

            return this;
        }

        public IGetStatusOccurrencePicker WithFilter(byte[] applicationFilter)
        {
            Ensure.IsNotNull(applicationFilter, nameof(applicationFilter));

            this.applicationFilter = applicationFilter;

            return this;
        }

        public IApduBuilder ReturnFirstOrAllOccurrences() => this;

        public IApduBuilder ReturnNextOccurrence()
        {
            this.P2 |= 0b00000001;

            return this;
        }

        public override CommandApdu AsApdu()
        {
            var apdu = CommandApdu.Case2S(ApduClass.GlobalPlatform, ApduInstruction.GetStatus, this.P1, this.P2 |= 0b00000010, 0x00);

            var data = new List<byte>();

            data.AddTLV(TLV.Build((byte)Tag.ApplicationAID, this.applicationFilter));

            apdu.CommandData = data.ToArray();

            return apdu;
        }
    }
}
