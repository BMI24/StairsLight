﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.Networking
{
    public sealed class FluentReader
    {
        private readonly byte[] Array;
        private int Index;

        public FluentReader(byte[] array)
        {
            Array = array;
            Index = 0;
        }

        public FluentReader ReadString(out string output)
        {
            try
            {
                output = UTF8ToString(Array, Index, out Index);
                return this;
            }
            catch (Exception e)
            {
                throw new ParsingException(e);
            }
        }

        public FluentReader ReadInt(out int output)
        {
            try
            {
                output = BitConverter.ToInt32(Array, Index);
                Index += 4;
                return this;
            }
            catch (Exception e)
            {
                throw new ParsingException(e);
            }
        }

        public FluentReader ReadEnum<T>(out T output)
        {
            object enumNumericValue = null;
            var underlyingType = Enum.GetUnderlyingType(typeof(T));

            if (underlyingType == typeof(byte))
            {
                ReadByte(out byte numericValue);
                enumNumericValue = numericValue;
            }
            else if (underlyingType == typeof(int))
            {
                ReadInt(out int numericValue);
                enumNumericValue = numericValue;
            }
            else
            {
                throw new NotImplementedException($"The enumtype {typeof(T)} has an underlying type that is not supported");
            }

            if (Enum.IsDefined(typeof(T), enumNumericValue))
            {
                output = (T)enumNumericValue;
                return this;
            }
            throw new ParsingException(null);
        }

        public FluentReader ReadByte(out byte output)
        {
            try
            {
                output = Array[Index];
                Index++;
                return this;
            }
            catch (Exception e)
            {
                throw new ParsingException(e);
            }
        }

        public FluentReader ReadLong(out long output)
        {
            try
            {
                output = BitConverter.ToInt64(Array, Index);
                Index += 8;
                return this;
            }
            catch (Exception e)
            {
                throw new ParsingException(e);
            }
        }

        public FluentReader ReadDateTime(out DateTime output)
        {
            try
            {
                ReadLong(out long ticks);
                output = new DateTime(ticks);
                return this;
            }
            catch (Exception e)
            {
                throw new ParsingException(e);
            }
        }

        public static string UTF8ToString(byte[] byteArray, int startIndex, out int afterIndex)
        {
            byte terminator = Encoding.UTF8.GetBytes("\0")[0];
            int i;
            List<byte> byteString = new List<byte>();
            for (i = startIndex; i < byteArray.Length; i++)
            {
                if (byteArray[i] == terminator)
                    break;
                byteString.Add(byteArray[i]);
            }
            afterIndex = i + 1;
            return Encoding.UTF8.GetString(byteString.ToArray());
        }

        public FluentReader ReadULong(out ulong output)
        {
            try
            {
                output = BitConverter.ToUInt64(Array, Index);
                Index += 8;
                return this;
            }
            catch (Exception e)
            {
                throw new ParsingException(e);
            }
        }

        public FluentReader ReadBool(out bool output)
        {
            try
            {
                output = BitConverter.ToBoolean(Array, Index);
                Index++;
                return this;
            }
            catch (Exception e)
            {
                throw new ParsingException(e);
            }
        }

        public byte[] ReadToEnd()
        {
            try
            {
                var value = Array.Skip(Index).ToArray();
                Index = Array.Length;
                return value;
            }
            catch (Exception e)
            {
                throw new ParsingException(e);
            }
        }
    }
}
