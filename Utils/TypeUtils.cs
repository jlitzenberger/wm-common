using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace WM.Common
{
    public class TypeUtils
    {
        public TypeUtils()
        {
        }

        public static string InvokeStringMethod(string typeName, string methodName)
        {
            // Get the Type for the class
            Type calledType = Type.GetType(typeName);

            // Invoke the method itself. The string returned by the method winds up in s
            String s = (String)calledType.InvokeMember(
                            methodName,
                            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                            null,
                            null,
                            null);

            // Return the string that was returned by the called method.
            return s;
        }

        public static string InvokeStringMethod(string typeName, string methodName, string stringParam)
        {
            // Get the Type for the class
            String s = string.Empty;
            Type calledType = Type.GetType(typeName);
            if (calledType != null)
            {
                // Invoke the method itself. The string returned by the method winds up in s.
                // Note that stringParam is passed via the last parameter of InvokeMember,
                // as an array of Objects.
                s = (String)calledType.InvokeMember(
                               methodName,
                               BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                               null,
                               null,
                               new Object[] { stringParam });
            }
            // Return the string that was returned by the called method.
            return s;
        }

        public static string InvokeStringMethod(string assemblyName, string namespaceName, string typeName, string methodName)
        {
            // Get the Type for the class
            Type calledType = Type.GetType(namespaceName + "." + typeName + "," + assemblyName);

            // Invoke the method itself. The string returned by the method winds up in s
            String s = (String)calledType.InvokeMember(
                            methodName,
                            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                            null,
                            null,
                            null);

            // Return the string that was returned by the called method.
            return s;
        }
    }
}
