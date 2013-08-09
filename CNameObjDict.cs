using System;
using System.Collections.Generic;

namespace AmfData
{
    public class CNameObjDict : Dictionary<string, object>
    {
        string m_className;

        public string className { get { return m_className; } }

        public CNameObjDict(string name = "")
        {
            m_className = name;
        }

        public string Str(string key)
        {
            return this[key].ToString();
        }

        public int Int(string key)
        {
            object o = this[key];
            if (!(o is UInt32))
                return Convert.ToInt32(o);
            uint v = (uint)o;
            if ((v >> 28) > 0)  // 负数
                v |= (uint)7 << 29;
            return (int)v;
        }

        public CNameObjDict Obj(string key)
        {
            return this[key] as CNameObjDict;
        }

        public CMixArray Ary(string key)
        {
            return this[key] as CMixArray;
        }

        public override string ToString()
        {
            return string.Format("CNameObjDict[{0}]", Count);
        }
    }
}
