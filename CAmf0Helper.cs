﻿// AMF协议格式的部分处理
//
// see: http://osflash.org/documentation/amf/astypes
//

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AmfData
{
    class CAmf0Helper
    {
        protected enum DataType
        {
            Number = 0x0,
            Boolean = 0x1,
            String = 0x2,
            Object = 0x3,
            MovieClip = 0x4,
            Null = 0x5,
            Undefined = 0x6,
            Reference = 0x7,
            MixedArray = 0x8,
            EndOfObject = 0x9,
            Array = 0xa,
            Date = 0xb,
            LongString = 0xc,
            Unsupported = 0xd,
            Recordset = 0xe,
            XML = 0xf,
            TypedObject = 0x10,
            AMF3data = 0x11,
        }

        // 无需构造实例
        private CAmf0Helper()
        {
        }

        public static object Read(Stream stm)
        {
            bool oe = true;
            return ReadAmf(stm, ref oe);
        }

        protected static object ReadAmf(Stream stm, ref bool objEnd)
        {
            int type = stm.ReadByte();
            bool oe = true;
            switch ((DataType)type)
            {
                case DataType.Number:
                    return CDataHelper.BE_ReadDouble(stm);
                case DataType.Boolean:
                    return stm.ReadByte() != 0x0;
                case DataType.String:
                    return CDataHelper.BE_ReadShortStr(stm);
                case DataType.Object:
                    return ReadHashObject(stm);
                case DataType.MovieClip:
                    break;
                case DataType.Null:
                    return null;
                case DataType.Undefined:
                    return null;
                case DataType.Reference:
                    break;
                case DataType.MixedArray:
                    CDataHelper.BE_ReadUInt32(stm); // highest numeric index
                    return ReadHashObject(stm);
                case DataType.EndOfObject:
                    Trace.Assert(!objEnd, "此处不应出现EndOfObject");
                    objEnd = true;
                    return null;
                case DataType.Array:
                    {
                        uint len = CDataHelper.BE_ReadUInt32(stm);
                        object[] ary = new object[len];
                        for (uint i = 0; i < len; i++)
                            ary[i] = ReadAmf(stm, ref oe);
                        return ary;
                    }
                case DataType.Date:
                    break;
                case DataType.LongString:
                    return CDataHelper.BE_ReadLongStr(stm);
                case DataType.Unsupported:
                    return null;
                case DataType.Recordset:
                    break;
                case DataType.XML:
                    break;
                case DataType.TypedObject:
                    break;
                case DataType.AMF3data:
                    return CAmf3Helper.Read(stm);
                default:
                    break;
            }

            Trace.Assert(false, "暂未处理的数据类型");
            return null;
        }

        protected static CNameObjDict ReadHashObject(Stream stm)
        {
            CNameObjDict dic = new CNameObjDict();
            bool oe = false;
            while (true)
            {
                string key = CDataHelper.BE_ReadShortStr(stm);
                object value = ReadAmf(stm, ref oe);
                if (oe)
                {
                    Trace.Assert(key == "");
                    break;
                }
                dic[key] = value;
            }
            return dic;
        }

        public static void Write(Stream stm, object obj)
        {
            WriteAmf(stm, obj);
        }

        public static void Write3(Stream stm, object obj)
        {
            stm.WriteByte((byte)DataType.AMF3data);
            CAmf3Helper.Write(stm, obj);
        }

        protected static void WriteAmf(Stream stm, object obj)
        {
            if (obj == null)
            {
                stm.WriteByte((byte)DataType.Null);
            }
            else if (obj is byte || obj is int || obj is uint || obj is float || obj is double) // 只列举了常用的
            {
                stm.WriteByte((byte)DataType.Number);
                CDataHelper.BE_WriteDouble(stm, double.Parse(obj.ToString()));
            }
            else if (obj is bool)
            {
                stm.WriteByte((byte)DataType.Boolean);
                stm.WriteByte((byte)((bool)obj ? 1 : 0));
            }
            else if (obj is string)
            {
                string str = obj as string;
                int len = Encoding.UTF8.GetByteCount(str);
                if (len > 0xffff)
                {
                    stm.WriteByte((byte)DataType.LongString);
                    CDataHelper.BE_WriteLongStr(stm, str);
                }
                else
                {
                    stm.WriteByte((byte)DataType.String);
                    CDataHelper.BE_WriteShortStr(stm, str);
                }
            }
            else if (obj is Array)
            {
                stm.WriteByte((byte)DataType.Array);
                Array ary = obj as Array;
                CDataHelper.BE_WriteUInt32(stm, (uint)ary.Length);
                foreach (object o in ary)
                    WriteAmf(stm, o);
            }
            else if (obj is IDictionary)
            {
                stm.WriteByte((byte)DataType.Object);
                foreach (DictionaryEntry e in obj as IDictionary)
                {
                    CDataHelper.BE_WriteShortStr(stm, e.Key.ToString());
                    WriteAmf(stm, e.Value);
                }
                CDataHelper.BE_WriteShortStr(stm, "");  // write only a short(0)
                stm.WriteByte((byte)DataType.EndOfObject);
            }
            else
            {
                Trace.Assert(false, "暂未处理的数据类型");
                stm.WriteByte((byte)DataType.Unsupported);
            }
        }

        public static object GetObject(byte[] buf)
        {
            return Read(new MemoryStream(buf));
        }

        public static byte[] GetBytes(object obj)
        {
            MemoryStream stm = new MemoryStream();
            Write(stm, obj);
            return stm.ToArray();
        }
    }
}
