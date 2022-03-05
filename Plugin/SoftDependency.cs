using BepInEx;
using System;
using System.Reflection;
using UnityEngine;

namespace HolloFox
{
    public partial class AudioPlugin : BaseUnityPlugin
    {

        public static class SoftDependency
        {
            public static void Invoke(string typeName, string methodName, object[] methodParameters)
            {
                Debug.Log("Audio Plugin: Soft Dependency: Invoking " + methodName + " On " + typeName);
                Type t = Type.GetType(typeName);
                if (t != null)
                {
                    MethodInfo m = t.GetMethod(methodName);
                    if (m != null)
                    {
                        try
                        {
                            m.Invoke(null, methodParameters);
                        }
                        catch (Exception x)
                        {
                            Debug.Log("Audio Plugin: Soft Dependency: Method Invoke Failed");
                            Debug.LogException(x);
                            Debug.Log("Audio Plugin: Soft Dependency: Trying Fallback");
                            InvokeEx(typeName, methodName, methodParameters);
                        }
                    }
                    else
                    {
                        Debug.Log("Audio Plugin: Soft Dependency: Method Reference Null");
                    }
                }
                else
                {
                    Debug.Log("Audio Plugin: Soft Dependency: Type Reference Null");
                }
            }

            public static void InvokeEx(string typeName, string methodName, object[] methodParameters)
            {
                Debug.Log("Audio Plugin: Soft Dependency: Ex Invoking " + methodName + " On " + typeName);
                Type t = Type.GetType(typeName);
                if (t != null)
                {
                    foreach (MethodInfo m in t.GetMethods())
                        if (m.Name == methodName)
                        {
                            try
                            {
                                m.Invoke(null, methodParameters);
                                return;
                            }
                            catch (Exception x)
                            {
                                Debug.Log("Audio Plugin: Soft Dependency: Ex Method Invoke Failed. Checking For Other Options");
                                Debug.LogException(x);
                                Debug.Log("Audio Plugin: Soft Dependency: Checking For Other Options");
                            }
                        }
                }
                else
                {
                    Debug.Log("Audio Plugin: Soft Dependency: Ex Type Reference Null");
                }
                Debug.Log("Audio Plugin: Soft Dependency: Ex Failed To Find Suitable Invoking Method");
            }

            public static T GetProperty<T>(string typeName, string propertyName)
            {
                Debug.Log("Audio Plugin: Soft Dependency: GetProperty " + propertyName + " Of " + typeName);
                Type t = Type.GetType(typeName);
                if (t != null)
                {
                    PropertyInfo p = t.GetProperty(propertyName);
                    if (p != null)
                    {
                        return (T)p.GetValue(null);
                    }
                    FieldInfo f = t.GetField(propertyName);
                    if (f != null)
                    {
                        return (T)f.GetValue(null);
                    }
                    Debug.Log("Audio Plugin: Soft Dependency: Property/Field Not Found");
                }
                else
                {
                    Debug.Log("Audio Plugin: Soft Dependency: Type Reference Null");
                }
                return default(T);
            }
        }
    }
}
