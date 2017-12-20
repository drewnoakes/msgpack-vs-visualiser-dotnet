using System;
using System.IO;
using System.Text;
using Dasher;

namespace MsgPackVisualizer
{
    public static class MessagePackFormatter
    {
        public static string ToJsonString(byte[] bytes, bool throwIfEndOfStream = false, int? maxByteArrayLength = null)
        {
            return ToJsonString(new Unpacker(new MemoryStream(bytes)), throwIfEndOfStream, maxByteArrayLength);
        }

        public static string ToJsonString(Unpacker unpacker, bool throwIfEndOfStream = false, int? maxByteArrayLength = null)
        {
            var s = new StringBuilder();
            ToJsonString(unpacker, s, throwIfEndOfStream, maxByteArrayLength);
            return s.ToString();
        }

        public static void ToJsonString(Unpacker unpacker, StringBuilder s, bool throwIfEndOfStream = false, int? maxByteArrayLength = null)
        {
            if (unpacker.HasStreamEnded)
            {
                if (throwIfEndOfStream)
                    throw new IOException("Unexpected end of stream.");
                return;
            }

            if (!unpacker.TryPeekFormat(out Format format))
                throw new Exception("Unable to determine format of MsgPack value.");

            switch (format)
            {
                case Format.Str8:
                case Format.Str16:
                case Format.Str32:
                case Format.FixStr:
                {
                    unpacker.TryReadString(out string str);
                    s.Append('"').Append(str).Append('"');
                    break;
                }
                case Format.Null:
                {
                    unpacker.TryReadNull();
                    s.Append("null");
                    break;
                }
                case Format.False:
                case Format.True:
                {
                    unpacker.TryReadBoolean(out bool b);
                    s.Append(b ? "true" : "false");
                    break;
                }
                case Format.Bin8:
                case Format.Bin16:
                case Format.Bin32:
                {
                    unpacker.TryReadBinary(out byte[] bytes);

                    // Attempt to interpret this nested byte[] as an embedded MsgPack object
                    try
                    {
                        var nestedStream = new MemoryStream(bytes);
                        var nestedUnpacker = new Unpacker(nestedStream);
                        var nestedStrBuilder = new StringBuilder();
                        ToJsonString(nestedUnpacker, nestedStrBuilder, maxByteArrayLength: maxByteArrayLength, throwIfEndOfStream: true);
                        if (nestedStream.Position == nestedStream.Length)
                        {
                            s.Append(nestedStrBuilder);
                            break;
                        }
                    }
                    catch
                    {
                        // Fall through
                    }

                    s.Append('[');
                    if (maxByteArrayLength.HasValue && bytes.Length > maxByteArrayLength)
                    {
                        s.Append($"<{bytes.Length} {(bytes.Length == 1 ? "byte" : "bytes")} omitted>");
                    }
                    else
                    {
                        for (var i = 0; i < bytes.Length; i++)
                        {
                            if (i != 0)
                                s.Append(", ");
                            s.Append(bytes[i]);
                        }
                    }
                    s.Append(']');
                    break;
                }
                case Format.Float32:
                {
                    unpacker.TryReadSingle(out float f);
                    s.Append(f);
                    break;
                }
                case Format.Float64:
                {
                    unpacker.TryReadDouble(out double d);
                    s.Append(d);
                    break;
                }
                case Format.PositiveFixInt:
                case Format.NegativeFixInt:
                case Format.Int8:
                case Format.Int16:
                case Format.UInt8:
                case Format.UInt16:
                case Format.Int32:
                {
                    unpacker.TryReadInt32(out int i);
                    s.Append(i);
                    break;
                }
                case Format.UInt32:
                {
                    unpacker.TryReadUInt32(out uint i);
                    s.Append(i);
                    break;
                }
                case Format.UInt64:
                {
                    unpacker.TryReadUInt64(out ulong l);
                    s.Append(l);
                    break;
                }
                case Format.Int64:
                {
                    unpacker.TryReadInt64(out long l);
                    s.Append(l);
                    break;
                }
                case Format.FixArray:
                case Format.Array16:
                case Format.Array32:
                {
                    s.Append('[');
                    unpacker.TryReadArrayLength(out int len);
                    for (var i = 0; i < len; i++)
                    {
                        if (i != 0)
                            s.Append(", ");
                        ToJsonString(unpacker, s, maxByteArrayLength: maxByteArrayLength, throwIfEndOfStream: true);
                    }
                    s.Append(']');
                    break;
                }
                case Format.FixMap:
                case Format.Map16:
                case Format.Map32:
                {
                    s.Append('{');
                    unpacker.TryReadMapLength(out int len);
                    for (var i = 0; i < len; i++)
                    {
                        if (i != 0)
                            s.Append(", ");
                        ToJsonString(unpacker, s, maxByteArrayLength: maxByteArrayLength, throwIfEndOfStream: true);
                        s.Append(": ");
                        ToJsonString(unpacker, s, maxByteArrayLength: maxByteArrayLength, throwIfEndOfStream: true);
                    }
                    s.Append('}');
                    break;
                }
                default:
                {
                    throw new Exception($"Unsupported MsgPack format {format}.");
                }
            }
        }
    }
}