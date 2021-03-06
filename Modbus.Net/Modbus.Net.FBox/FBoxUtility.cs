﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modbus.Net.FBox
{
    public enum FBoxType
    {
        AddressSync = 0,
        CommunicationTagSync = 1
    }

    public class FBoxUtility :BaseUtility
    {
        private FBoxType _fboxType;
        private SignalRSigninMsg SigninMsg { get; set; }
        
        public string LocalSequence { get; set; }

        protected IEnumerable<CommunicationUnit> CommunicationUnits { get; set; } 
        public FBoxType ConnectionType
        {
            get
            {
                return _fboxType;
            }
            set
            {
                _fboxType = value;
                switch (_fboxType)
                {
                    case FBoxType.AddressSync:
                        {
                            throw new NotImplementedException();
                        }
                    case FBoxType.CommunicationTagSync:
                        {
                            Wrapper = new FBoxSignalRProtocal(ConnectionString, LocalSequence, SigninMsg);
                            break;
                        }
                }
            }
        }

        public FBoxUtility(FBoxType fBoxType, string connectionString, string localSequence, IEnumerable<CommunicationUnit> communicationUnits, SignalRSigninMsg msg)
        {
            ConnectionString = connectionString;
            LocalSequence = localSequence;
            CommunicationUnits = communicationUnits.AsEnumerable();
            SigninMsg = msg;

            ConnectionType = fBoxType;           
            AddressTranslator = new AddressTranslatorFBox();
        }

        public override void SetConnectionType(int connectionType)
        {
            ConnectionType = (FBoxType) connectionType;
        }

        public override async Task<GetDataReturnDef> GetDatasAsync(byte belongAddress, byte masterAddress, string startAddress, int getByteCount)
        {
            try
            {
                var readRequestFBoxInputStruct = new ReadRequestFBoxInputStruct(startAddress, (ushort)getByteCount, AddressTranslator);
                var readRequestSiemensOutputStruct =
                     await
                         Wrapper.SendReceiveAsync(Wrapper[typeof(ReadRequestFBoxProtocal)], readRequestFBoxInputStruct) as ReadRequestFBoxOutputStruct;
                return new GetDataReturnDef()
                {
                    ReturnValue = readRequestSiemensOutputStruct?.GetValue,
                    IsLittleEndian = Wrapper[typeof (ReadRequestFBoxProtocal)].IsLittleEndian
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override Task<bool> SetDatasAsync(byte belongAddress, byte masterAddress, string startAddress, object[] setContents)
        {
            throw new NotImplementedException();
        }
    }
}
