#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Stefan Lange (mailto:Stefan.Lange@PdfSharpCore.com)
//   Klaus Potzesny (mailto:Klaus.Potzesny@PdfSharpCore.com)
//   David Stephensen (mailto:David.Stephensen@PdfSharpCore.com)
//
// Copyright (c) 2001-2009 empira Software GmbH, Cologne (Germany)
//
// http://www.PdfSharpCore.com
// http://www.migradoc.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace MigraDocCore.DocumentObjectModel.Internals
{
    /// <summary>
    /// Base class of all value descriptor classes.
    /// </summary>
    public abstract class ValueDescriptor
    {
        internal ValueDescriptor(
            string valueName, 
            [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            Type valueType, 
            Type memberType, 
            MemberInfo memberInfo, 
            VDFlags flags
        )
        {
            this.ValueName = valueName;
            this.ValueType = valueType;
            this.MemberType = memberType;
            this.memberInfo = memberInfo;
            this.flags = flags;
        }

        public object CreateValue()
        {
            ConstructorInfo constructorInfoObj = ValueType.GetConstructor(Type.EmptyTypes);
            
            return constructorInfoObj.Invoke(null);
        }

        public abstract object GetValue(DocumentObject dom, GV flags);
        public abstract void SetValue(DocumentObject dom, object val);
        public abstract void SetNull(DocumentObject dom);
        public abstract bool IsNull(DocumentObject dom);

        internal static ValueDescriptor CreateValueDescriptor(MemberInfo memberInfo, DVAttribute attr)
        {
            VDFlags flags = VDFlags.None;
            if (attr.RefOnly)
                flags |= VDFlags.RefOnly;

            string name = memberInfo.Name;
             
            Type type;
            if (memberInfo is FieldInfo)
                type = ((FieldInfo)memberInfo).FieldType;
            else
                type = ((PropertyInfo)memberInfo).PropertyType;

            if (type == typeof(NBool))
                return new NullableDescriptor(name, typeof(Boolean), type, memberInfo, flags);

            if (type == typeof(NInt))
                return new NullableDescriptor(name, typeof(Int32), type, memberInfo, flags);

            if (type == typeof(NDouble))
                return new NullableDescriptor(name, typeof(Double), type, memberInfo, flags);

            if (type == typeof(NString))
                return new NullableDescriptor(name, typeof(String), type, memberInfo, flags);

            if (type == typeof(String))
                return new ValueTypeDescriptor(name, typeof(String), type, memberInfo, flags);

            if (type == typeof(NEnum))
            {
                Type valueType = attr.Type;
                Debug.Assert(valueType.GetTypeInfo().IsSubclassOf(typeof(Enum)), "NEnum must have 'Type' attribute with the underlying type");
                return new NullableDescriptor(name, valueType, type, memberInfo, flags);
            }

            if (type.GetTypeInfo().IsSubclassOf(typeof(ValueType)))
                return new ValueTypeDescriptor(name, type, type, memberInfo, flags);

            if (typeof(DocumentObjectCollection).IsAssignableFrom(type))
                return new DocumentObjectCollectionDescriptor(name, type, type, memberInfo, flags);

            if (typeof(DocumentObject).IsAssignableFrom(type))
                return new DocumentObjectDescriptor(name, type, type, memberInfo, flags);

            Debug.Assert(false, type.FullName);
            return null;
        }

        public bool IsRefOnly
        {
            get { return (this.flags & VDFlags.RefOnly) == VDFlags.RefOnly; }
        }

        public FieldInfo FieldInfo
        {
            get { return this.memberInfo as FieldInfo; }
        }

        public PropertyInfo PropertyInfo
        {
            get { return this.memberInfo as PropertyInfo; }
        }

        /// <summary>
        /// Name of the value.
        /// </summary>
        public string ValueName;

        /// <summary>
        /// Type of the described value, e.g. typeof(Int32) for an NInt.
        /// </summary>
        ///
        [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        public Type ValueType;

        /// <summary>
        /// Type of the described field or property, e.g. typeof(NInt) for an NInt.
        /// </summary>
        public Type MemberType;

        /// <summary>
        /// FieldInfo of the described field.
        /// </summary>
        protected MemberInfo memberInfo;

        /// <summary>
        /// Flags of the described field, e.g. RefOnly.
        /// </summary>
        VDFlags flags;
    }

    /// <summary>
    /// Value descriptor of all nullable types.
    /// </summary>
    internal class NullableDescriptor : ValueDescriptor
    {
        internal NullableDescriptor(string valueName, [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]Type valueType, Type fieldType, MemberInfo memberInfo, VDFlags flags)
          : base(valueName, valueType, fieldType, memberInfo, flags)
        {
        }

        public override object GetValue(DocumentObject dom, GV flags)
        {
            if (!Enum.IsDefined(typeof(GV), flags))
                throw new ArgumentException("flags");
           // throw new InvalidEnumArgumentException("flags", (int)flags, typeof(GV));

            object val;
            if (FieldInfo != null)
                val = FieldInfo.GetValue(dom);
            else
                val = PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes);
            INullableValue ival = (INullableValue)val;
            if (ival.IsNull && flags == GV.GetNull)
                return null;
            return ival.GetValue();
        }

        public override void SetValue(DocumentObject dom, object value)
        {
            object val;
            INullableValue ival;
            if (FieldInfo != null)
            {
                val = FieldInfo.GetValue(dom);
                ival = (INullableValue)val;
                ival.SetValue(value);
                FieldInfo.SetValue(dom, ival);
            }
            else
            {
                val = PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes);
                ival = (INullableValue)val;
                ival.SetValue(value);
                PropertyInfo.GetSetMethod(true).Invoke(dom, new object[] { ival });
            }
        }

        public override void SetNull(DocumentObject dom)
        {
            object val;
            INullableValue ival;
            if (FieldInfo != null)
            {
                val = FieldInfo.GetValue(dom);
                ival = (INullableValue)val;
                ival.SetNull();
                FieldInfo.SetValue(dom, ival);
            }
            else
            {
                val = PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes);
                ival = (INullableValue)val;
                ival.SetNull();
                PropertyInfo.GetSetMethod(true).Invoke(dom, new object[] { ival });
            }
        }

        /// <summary>
        /// Determines whether the given DocumentObject is null (not set).
        /// </summary>
        public override bool IsNull(DocumentObject dom)
        {
            object val;
            if (FieldInfo != null)
                val = FieldInfo.GetValue(dom);
            else
                val = PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes);
            return ((INullableValue)val).IsNull;
        }
    }

    /// <summary>
    /// Value descriptor of value types.
    /// </summary>
    internal class ValueTypeDescriptor : ValueDescriptor
    {
        internal ValueTypeDescriptor(string valueName, [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]Type valueType, Type fieldType, MemberInfo memberInfo, VDFlags flags)
          :
          base(valueName, valueType, fieldType, memberInfo, flags)
        {
        }

        public override object GetValue(DocumentObject dom, GV flags)
        {
            if (!Enum.IsDefined(typeof(GV), flags))
                throw new ArgumentException("flags");
                //throw new InvalidEnumArgumentException("flags", (int)flags, typeof(GV));

            object val;
            if (FieldInfo != null)
                val = FieldInfo.GetValue(dom);
            else
                val = PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes);
            INullableValue ival = val as INullableValue;
            if (ival != null && ival.IsNull && flags == GV.GetNull)
                return null;
            return val;
        }

        public override void SetValue(DocumentObject dom, object value)
        {
            if (FieldInfo != null)
                FieldInfo.SetValue(dom, value);
            else
            {
                PropertyInfo.GetSetMethod(true).Invoke(dom, new object[] { value });
            }
        }

        public override void SetNull(DocumentObject dom)
        {
            object val;
            INullableValue ival;
            if (FieldInfo != null)
            {
                val = FieldInfo.GetValue(dom);
                ival = (INullableValue)val;
                ival.SetNull();
                FieldInfo.SetValue(dom, ival);
            }
            else
            {
                val = PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes);
                ival = (INullableValue)val;
                ival.SetNull();
                PropertyInfo.GetSetMethod(true).Invoke(dom, new object[] { ival });
            }
        }

        /// <summary>
        /// Determines whether the given DocumentObject is null (not set).
        /// </summary>
        public override bool IsNull(DocumentObject dom)
        {
            object val;
            if (FieldInfo != null)
                val = FieldInfo.GetValue(dom);
            else
                val = PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes);
            INullableValue ival = val as INullableValue;
            if (ival != null)
                return ival.IsNull;
            return false;
        }
    }

    /// <summary>
    /// Value descriptor of DocumentObject.
    /// </summary>
    internal class DocumentObjectDescriptor : ValueDescriptor
    {
        internal DocumentObjectDescriptor(string valueName, [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]Type valueType, Type fieldType, MemberInfo memberInfo, VDFlags flags)
          :
          base(valueName, valueType, fieldType, memberInfo, flags)
        {
        }

        public override object GetValue(DocumentObject dom, GV flags)
        {
            if (!Enum.IsDefined(typeof(GV), flags))
                //throw new InvalidEnumArgumentException("flags", (int)flags, typeof(GV));
                throw new ArgumentException("flags");

            FieldInfo fieldInfo = FieldInfo;
            DocumentObject val;
            if (fieldInfo != null)
            {
                // Member is a field
                val = FieldInfo.GetValue(dom) as DocumentObject;
                if (val == null && flags == GV.ReadWrite)
                {
                    val = CreateValue() as DocumentObject;
                    val.parent = dom;
                    FieldInfo.SetValue(dom, val);
                    return val;
                }
            }
            else
            {
                // Member is a property
                val = PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes) as DocumentObject;
            }
            if (val != null && (val.IsNull() && flags == GV.GetNull))
                return null;

            return val;
        }

        public override void SetValue(DocumentObject dom, object val)
        {
            FieldInfo fieldInfo = FieldInfo;
            // Member is a field
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(dom, val);
                return;
            }
            throw new InvalidOperationException("This value cannot be set.");
        }

        public override void SetNull(DocumentObject dom)
        {
            FieldInfo fieldInfo = FieldInfo;
            DocumentObject val;
            // Member is a field
            if (fieldInfo != null)
            {
                val = FieldInfo.GetValue(dom) as DocumentObject;
                if (val != null)
                    val.SetNull();
            }
            // Member is a property
            //REVIEW KlPo4All: Wird das gebraucht?
            if (PropertyInfo != null)
            {
                PropertyInfo propInfo = PropertyInfo;
                val = propInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes) as DocumentObject;
                if (val != null)
                    val.SetNull();
            }
            return;
        }

        /// <summary>
        /// Determines whether the given DocumentObject is null (not set).
        /// </summary>
        public override bool IsNull(DocumentObject dom)
        {
            FieldInfo fieldInfo = FieldInfo;
            DocumentObject val;
            // Member is a field
            if (fieldInfo != null)
            {
                val = FieldInfo.GetValue(dom) as DocumentObject;
                if (val == null)
                    return true;
                return val.IsNull();
            }
            // Member is a property
            PropertyInfo propInfo = PropertyInfo;
            val = propInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes) as DocumentObject;
            if (val != null)
                val.IsNull();
            return true;
        }
    }

    /// <summary>
    /// Value descriptor of DocumentObjectCollection.
    /// </summary>
    internal class DocumentObjectCollectionDescriptor : ValueDescriptor
    {
        internal DocumentObjectCollectionDescriptor(string valueName, [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]Type valueType, Type fieldType, MemberInfo memberInfo, VDFlags flags)
          :
          base(valueName, valueType, fieldType, memberInfo, flags)
        {
        }

        public override object GetValue(DocumentObject dom, GV flags)
        {
            if (!Enum.IsDefined(typeof(GV), flags))
                throw new ArgumentException("flags");
                //throw new InvalidEnumArgumentException("flags", (int)flags, typeof(GV));

            Debug.Assert(this.memberInfo is FieldInfo, "Properties of DocumentObjectCollection not allowed.");
            DocumentObjectCollection val = FieldInfo.GetValue(dom) as DocumentObjectCollection;
            if (val == null && flags == GV.ReadWrite)
            {
                val = CreateValue() as DocumentObjectCollection;
                val.parent = dom;
                FieldInfo.SetValue(dom, val);
                return val;
            }
            if (val != null && val.IsNull() && flags == GV.GetNull)
                return null;

            return val;
        }

        public override void SetValue(DocumentObject dom, object val)
        {
            FieldInfo.SetValue(dom, val);
        }

        public override void SetNull(DocumentObject dom)
        {
            DocumentObjectCollection val = FieldInfo.GetValue(dom) as DocumentObjectCollection;
            if (val != null)
                val.SetNull();
        }

        /// <summary>
        /// Determines whether the given DocumentObject is null (not set).
        /// </summary>
        public override bool IsNull(DocumentObject dom)
        {
            DocumentObjectCollection val = FieldInfo.GetValue(dom) as DocumentObjectCollection;
            if (val == null)
                return true;
            return val.IsNull();
        }
    }
}
