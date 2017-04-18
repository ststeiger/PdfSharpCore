using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Resources
{
    internal static class DomSR
    {
        internal static string GetString(DomMsgID id)
        {
            return (string)typeof(AppResources).GetProperties().Where(x => x.Name == id.ToString() && x.PropertyType == typeof(string)).FirstOrDefault()?.GetValue(null); 
        }

        internal static string FormatMessage(DomMsgID id, params object[] args)
        {
            string message;
            try
            {
                message = GetString(id);
                if (message != null)
                {
#if DEBUG
                    if (Regex.Matches(message, @"\{[0-9]\}").Count > args.Length)
                    {
                        //TODO too many placeholders or too less args...
                    }
#endif
                    message = String.Format(message, args);
                }
                else
                    message = "<<<error: message not found>>>";
                return message;
            }
            catch (Exception ex)
            {
                message = "INTERNAL ERROR while formatting error message: " + ex.ToString();
            }
            return message;
        }

        internal static string InvalidValueName(string name)
        {
            return string.Format(AppResources.InvalidValueName, name);
        }

        internal static string ParentAlreadySet(DocumentObject value, DocumentObject docObject)
        {
            return string.Format("Value of type '{0}' must be cloned before set into '{1}'.",
                value.GetType().ToString(), docObject.GetType().ToString());
        }

        internal static string UndefinedBaseStyle(string baseStyle)
        {
            return string.Format(AppResources.UndefinedBaseStyle, baseStyle);
        }

        internal static string InvalidUnitValue(string value)
        {
            return string.Format(AppResources.InvalidUnitValue, value);
        }

        internal static string InvalidUnitType(string value)
        {
            return string.Format(AppResources.InvalidUnitType, value);
        }

        internal static string MissingObligatoryProperty(string v1, string v2)
        {
            return string.Format(AppResources.MissingObligatoryProperty, v1, v2);
        }

        internal static string InvalidInfoFieldName(string value)
        {
            return string.Format(AppResources.InvalidInfoFieldName, value);
        }

        internal static string InvalidFieldFormat(string value)
        {
            return string.Format(AppResources.InvalidFieldFormat, value);
        }

        internal static string InvalidColorString(string color)
        {
            return string.Format(AppResources.InvalidColorString, color);
        }
    }
}
