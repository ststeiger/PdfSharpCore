using System;
using System.Reflection;
namespace MigraDocCore.Rendering.UnitTest
{
  /// <summary>
  /// Summary description for ValueDumper.
  /// </summary>
  internal class ValueDumper
  {
    internal ValueDumper()
    {
    }

    internal static string DumpValues(object obj)
    {
      string dumpString = "[" + obj.GetType() + "]\r\n";
      foreach (FieldInfo fieldInfo in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
      {
        if (fieldInfo.FieldType.GetTypeInfo().IsValueType)
        {
          dumpString += "  " + fieldInfo.Name + " = " + fieldInfo.GetValue(obj) + "\r\n";
        }
      }
      return dumpString;

    }
  }
}
