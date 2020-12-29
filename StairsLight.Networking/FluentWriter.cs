using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StairsLight.Networking
{
    public sealed class FluentWriter
    {
        List<byte> ByteList = new List<byte>();

        public FluentWriter WriteString(string input)
        {
            ByteList.AddRange(ToUTF8(input));
            return this;
        }

        public FluentWriter WriteInt(int input)
        {
            ByteList.AddRange(BitConverter.GetBytes(input));
            return this;
        }

        public FluentWriter WriteEnum<T>(T input)
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(T));

            if (underlyingType == typeof(byte))
                WriteByte(Convert.ToByte(input));
            else if (underlyingType == typeof(int))
                WriteInt(Convert.ToInt32(input));
            else
                throw new NotImplementedException($"The enumtype {typeof(T)} has an underlying type that is not supported");

            return this;
        }

        public FluentWriter WriteDateTime(DateTime input) => WriteLong(input.Ticks);

        public FluentWriter WriteLong(long input)
        {
            ByteList.AddRange(BitConverter.GetBytes(input));
            return this;
        }

        public FluentWriter WriteULong(ulong input)
        {
            ByteList.AddRange(BitConverter.GetBytes(input));
            return this;
        }

        public FluentWriter WriteByte(byte input)
        {
            ByteList.Add(input);
            return this;
        }

        public byte[] ToByteArray() => ByteList.ToArray();

        public static implicit operator byte[] (FluentWriter coder) => coder.ToByteArray();

        public FluentWriter WriteByteArray(byte[] b)
        {
            ByteList.AddRange(b);
            return this;
        }

        public FluentWriter WriteBool(bool input)
        {
            ByteList.AddRange(BitConverter.GetBytes(input));
            return this;
        }

        public FluentWriter WriteSingle(float input)
        {
            ByteList.AddRange(BitConverter.GetBytes(input));
            return this;
        }

        public FluentWriter WriteColor(byte r, byte g, byte b)
        {
            WriteByte(r);
            WriteByte(g);
            WriteByte(b);
            return this;
        }

        public FluentWriter WriteTimeSpan(TimeSpan timeSpan)
        {
            WriteLong(timeSpan.Ticks);
            return this;
        }

        public static byte[] ToUTF8(object o) => Encoding.UTF8.GetBytes(o.ToString() + '\0');
    }
}
