﻿using System;
using System.Collections.Generic;

namespace Modbus.Net.Siemens
{
    public class AddressTranslatorSiemens : AddressTranslator
    {
        protected Dictionary<string, int> AreaCodeDictionary;

        public AddressTranslatorSiemens()
        {
            AreaCodeDictionary = new Dictionary<string, int>
            {
                {"S", 0x04},
                {"SM", 0x05},
                {"AI", 0x06},
                {"AQ", 0x07},
                {"C", 0x1E},
                {"T", 0x1F},
                {"HC", 0x20},
                {"I", 0x81},
                {"Q", 0x82},
                {"M", 0x83},
                {"DB", 0x84},
                {"V", 0x184},
            };
        }

        public override AddressDef AddressTranslate(string address, bool isRead)
        {
            /*
            address = address.ToUpper();
            if (address.Substring(0, 2) == "DB")
            {
                var addressSplit = address.Split('.');
                if (addressSplit.Length != 2) throw new FormatException();
                addressSplit[0] = addressSplit[0].Substring(2);
                if (addressSplit[1].Substring(0, 2) == "DB")
                    addressSplit[1] = addressSplit[1].Substring(2);
                return new AddressDef()
                {
                    Area = int.Parse(addressSplit[0])*256 + AreaCodeDictionary["DB"],
                    Address = int.Parse(addressSplit[1])
                };
            }
            int i = 0;
            int t;
            while (!int.TryParse(address[i].ToString(), out t) && i < address.Length)
            {
                i++;
            }
            if (i == 0 || i >= address.Length) throw new FormatException();
            string head = address.Substring(0, i);
            string tail = address.Substring(i);
            return
                new AddressDef()
                {
                    Area = AreaCodeDictionary[head],
                    Address = int.Parse(tail)
                };
            */
            string[] splitString = address.Split(' ');
            string head = splitString[0];
            string tail = splitString[1];
            if (head.Length > 1 && head.Substring(0, 2) == "DB")
            {
                head = head.Substring(2);
                return new AddressDef()
                {
                    Area = int.Parse(head)*256 + AreaCodeDictionary["DB"],
                    Address = int.Parse(tail)
                };
            }
            return 
                new AddressDef()
                {
                    Area = AreaCodeDictionary[head],
                    Address = int.Parse(tail)
                };
        }
    }
}