﻿using System.Threading.Tasks;

namespace Modbus.Net.Siemens
{
    public class SiemensTcpProtocal : SiemensProtocal
    {
        private readonly ushort _taspSrc;
        private readonly ushort _tsapDst;
        private readonly ushort _maxCalling;
        private readonly ushort _maxCalled;
        private readonly ushort _maxPdu;
        private readonly byte _tdpuSize;

        private readonly string _ip;
        private readonly int _port;
        private int _connectTryCount;

        public SiemensTcpProtocal(byte tdpuSize, ushort tsapSrc, ushort tsapDst, ushort maxCalling, ushort maxCalled, ushort maxPdu) : this(tdpuSize, tsapSrc, tsapDst, maxCalling, maxCalled, maxPdu, ConfigurationManager.IP)
        {
        }

        public SiemensTcpProtocal(byte tdpuSize, ushort tsapSrc, ushort tsapDst, ushort maxCalling, ushort maxCalled,
            ushort maxPdu, string ip) : this (tdpuSize, tsapSrc, tsapDst, maxCalling, maxCalled, maxPdu, ip, 0)
        {
            
        }

        public SiemensTcpProtocal(byte tdpuSize, ushort tsapSrc, ushort tsapDst, ushort maxCalling, ushort maxCalled, ushort maxPdu, string ip, int port)
        {
            _taspSrc = tsapSrc;
            _tsapDst = tsapDst;
            _maxCalling = maxCalling;
            _maxCalled = maxCalled;
            _maxPdu = maxPdu;
            _tdpuSize = tdpuSize;
            _ip = ip;
            _port = port;
            _connectTryCount = 0;
        }

        public override byte[] SendReceive(bool isLittleEndian, params object[] content)
        {
            return AsyncHelper.RunSync(() => SendReceiveAsync(isLittleEndian, content));
        }

        public override async Task<byte[]> SendReceiveAsync(bool isLittleEndian, params object[] content)
        {
            if (ProtocalLinker == null || !ProtocalLinker.IsConnected)
            {
                await ConnectAsync();
            }
            return await base.SendReceiveAsync(isLittleEndian, content);
        }

        public override OutputStruct SendReceive(ProtocalUnit unit, InputStruct content)
        {
            return AsyncHelper.RunSync(() => SendReceiveAsync(unit, content));
        }

        public override async Task<OutputStruct> SendReceiveAsync(ProtocalUnit unit, InputStruct content)
        {
            if (ProtocalLinker != null && ProtocalLinker.IsConnected) return await base.SendReceiveAsync(unit, content);
            if (_connectTryCount > 10) return null;
            return await await ConnectAsync().ContinueWith(answer => answer.Result ? base.SendReceiveAsync(unit, content) : null);
        }

        private async Task<OutputStruct> ForceSendReceiveAsync(ProtocalUnit unit, InputStruct content)
        {
            return await base.SendReceiveAsync(unit, content);
        }

        public override bool Connect()
        {
            return AsyncHelper.RunSync(ConnectAsync);
        }

        public override async Task<bool> ConnectAsync()
        {
            _connectTryCount++;
            ProtocalLinker = _port == 0 ? new SiemensTcpProtocalLinker(_ip) : new SiemensTcpProtocalLinker(_ip, _port);
            if (!await ProtocalLinker.ConnectAsync()) return false;
            _connectTryCount = 0;
            var inputStruct = new CreateReferenceSiemensInputStruct(_tdpuSize, _taspSrc, _tsapDst);
            return
                await await
                    ForceSendReceiveAsync(this[typeof (CreateReferenceSiemensProtocal)], inputStruct)
                        .ContinueWith(async answer =>
                        {
                            if (!ProtocalLinker.IsConnected) return false;
                            var inputStruct2 = new EstablishAssociationSiemensInputStruct(0x0101, _maxCalling,
                                _maxCalled,
                                _maxPdu);
                            var outputStruct2 =
                                (EstablishAssociationSiemensOutputStruct)
                                    await
                                        SendReceiveAsync(this[typeof (EstablishAssociationSiemensProtocal)],
                                            inputStruct2);
                            return outputStruct2 != null;
                        });
        }
    }
}
