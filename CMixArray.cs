using System;
using System.Collections.Generic;

namespace AmfData
{
    public class CMixArray
    {
        object[] m_fixArray;
        Dictionary<string, object> m_dynArray = new Dictionary<string, object>();

        public CMixArray(int size)
        {
            m_fixArray = new object[size];
        }

        public int Int(int index)
        {
            return Convert.ToInt32(this[index]);
        }

        public CNameObjDict Obj(int index)
        {
            return this[index] as CNameObjDict;
        }

        public CMixArray Ary(int index)
        {
            return this[index] as CMixArray;
        }

        public override string ToString()
        {
            return string.Format("CMixArray[{0}+{1}]", m_fixArray.Length, m_dynArray.Count);
        }

        public object this[int index]
        {
            get { return m_fixArray[index]; }
            set { m_fixArray[index] = value; }
        }

        public object this[string key]
        {
            get { return m_dynArray[key]; }
            set { m_dynArray[key] = value; }
        }

        public object[] Fixed { get { return m_fixArray; } }
        public int FixedLength { get { return m_fixArray.Length; } }
        public Dictionary<string, object> Dynamic { get { return m_dynArray; } }
    }
}
