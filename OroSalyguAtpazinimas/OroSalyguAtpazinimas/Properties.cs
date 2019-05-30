using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OroSalyguAtpazinimas
{
    class Properties
    {
        private void getProperties(object o, out double[] arr)
        {
            arr = new double[0];
            MemberInfo[] members = o.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            int i = 0;
            foreach (MemberInfo m in members)
            {
                FieldInfo f = m as FieldInfo;
                PropertyInfo p = m as PropertyInfo;
                if (f != null || p != null)
                {
                    Type t = f != null ? f.FieldType : p.PropertyType;
                    if (t.IsValueType || t == typeof(string))
                    {
                        arr[i] = (double) p.GetValue(o, null);
                    }
                }
            }
        }
    }
}
