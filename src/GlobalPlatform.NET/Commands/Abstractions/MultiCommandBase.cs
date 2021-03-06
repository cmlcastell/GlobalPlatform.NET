﻿using System.Collections.Generic;
using System.Linq;
using GlobalPlatform.NET.Commands.Interfaces;
using Iso7816;

namespace GlobalPlatform.NET.Commands.Abstractions
{
    public abstract class MultiCommandBase<TCommand, TBuilder> : IMultiApduBuilder
        where TCommand : TBuilder, new()
    {
        protected byte P1;

        protected byte P2;

        /// <summary>
        /// Starts building the command. 
        /// </summary>
        public static TBuilder Build => new TCommand();

        public abstract IEnumerable<CommandApdu> AsApdus();

        public IEnumerable<byte[]> AsBytes() => this.AsApdus().Select(apdu => apdu.Buffer.ToArray());
    }
}
