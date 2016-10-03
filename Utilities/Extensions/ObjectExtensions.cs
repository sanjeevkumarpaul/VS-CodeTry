using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Utilities.Constants;

namespace Utilities.Extensions
{

    public static partial class ObjectExtensions
    {
        private static Dictionary<string, PropertyInfo> BaseProperties<T>(this Type type, bool virtuals = false, bool privates = false)
        {
            Dictionary<string, PropertyInfo> pros = new Dictionary<string, PropertyInfo>();

            Type _type = type.DeclaringType;
            if (_type == null || _type.FullName.StartsWith("System")) return pros;

            BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance |
                                 (privates ? BindingFlags.NonPublic : BindingFlags.Instance);

            foreach (PropertyInfo property in _type.GetProperties(Flags))
            {
                if (!virtuals)
                    if (_type.GetProperty(property.Name).GetGetMethod().IsVirtual) continue;

                pros.Add(property.Name, property);
            }
            
            foreach (var item in _type.BaseProperties<T>(virtuals, privates))
            {
                if (!pros.ContainsKey(item.Key)) pros.Add(item.Key, item.Value);
            }

            return pros;
        }

        private static List<PropertyInfo> PropertiesTree<T>(this T obj, bool virtuals = false, bool privates = false)
        {
            var type = obj.GetType();

            Dictionary<string, PropertyInfo> pros = type.BaseProperties<T>(virtuals, privates);

            foreach (var property in obj.Properties(virtuals, privates))
            {
                if (!pros.ContainsKey(property.Name)) pros.Add(property.Name, property);
            }

            List<PropertyInfo> info = new List<PropertyInfo>();
            foreach( var item in pros  ) info.Add( item.Value );

            return info;
        }
    }


    public static partial class ObjectExtensions
    {
        public static bool Empty<T>(this T obj, bool considerZeroAsNull = false)
        {
            string value = string.Format("{0}", obj);
            if (considerZeroAsNull && value.Equals("0")) value = "";

            return value.Empty();
        }

        public static bool Null(this object obj)
        {
            return obj == null;
        }

        public static string Name(this object obj)
        {
            return obj.GetType().Name;
        }

        public static string ToStringExt(this object obj, bool minMaxDateToEmpty = false)
        {
            if (!obj.Null())
            {
                return (obj.ToString().ToLower() == "system.byte[]") ?
                    string.Format("content-length: {0}", ((byte[])obj).Length) :
                    (minMaxDateToEmpty && obj.GetType().Equals(typeof(DateTime)) ? obj.ToString().MaxMinDateToEmpty() : obj.ToString());
            }

            return null;
        }

        public static DataTypes DataType (this PropertyInfo property)
        {
            string _name = property
                                .PropertyType
                                .ToString()
                                .ToUpper()
                                .Replace("SYSTEM.", "")
                                .Replace("NULLABLE`1", "")
                                .Replace("[","")
                                .Replace("]", "");
            
            switch (_name.Trim())
            {
                case "BYTE"        : 
                case "SBYTE"       :
                case "SHORT"       :
                case "USHORT"      :
                case "INT"         :
                case "UINT"        :
                case "LONG"        :
                case "ULONG"       :
                case "FLOAT"       :
                case "DOUBLE"      :
                case "DECIMAL"     :
                case "MONEY"       :
                case "INT32"       :
                case "INT64"       : 
                case "SINGLE"      : 
                                return DataTypes.NUMERIC; 
                case "DATE"        :
                case "DATETIME"    :
                                return DataTypes.DATE;
                default: 
                                return DataTypes.STRING; 
            }
        }

        public static bool IsString(this PropertyInfo property)
        {
            return property.DataType() == DataTypes.STRING;
        }

        public static bool IsNumeric(this PropertyInfo property)
        {
            return property.DataType() == DataTypes.NUMERIC;
        }

        public static bool IsDate(this PropertyInfo property)
        {
            return property.DataType() == DataTypes.DATE;
        }
        
        public static object ConvertToPropertyType(this PropertyInfo prop, string value)
        {
            try
            {
                return Convert.ChangeType(value, prop.PropertyType);
            }
            catch
            {
                return value;
            }
        }

        public static string MaxMinDateToEmpty(this string date)
        {
            try
            {
                DateTime _date = DateTime.Parse(date);
                //return (_date == DateTime.MinValue || _date == DateTime.MaxValue) ? "" : date;
                return (_date <= AppConstant.SQLDateTimeMin || _date >= AppConstant.SQLDateTimeMax) ? "" : date;
            }
            catch { }

            return date;
        }

        public static void Fill<T>(this T obj) where T:class
        {
            obj.Properties().ForEach(p =>

                    {
                        try
                        {
                            if (p.PropertyType.Name.ToUpper() == "STRING")
                                obj.SetProperty(p.Name,  "1");
                            else
                                obj.SetProperty(p.Name, 999);
                        }
                        catch { }
                    }
                );
        }

        public static List<PropertyInfo> PublicFields(this Type t)
        {
            return t.GetTypeInfo().DeclaredProperties.ToList();
        }

        public static List<PropertyInfo> Properties<T>(this T obj, bool virtuals = false, bool privates = false)
        {
            List<PropertyInfo> pros = new List<PropertyInfo>();

            Type _type = obj.GetType();
            BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | (privates ? BindingFlags.NonPublic : BindingFlags.Instance);

            foreach (PropertyInfo property in _type.GetProperties(Flags))
            {
                if (!virtuals)
                    if (_type.GetProperty(property.Name).GetGetMethod().IsVirtual) continue;

                pros.Add(property);

            }

            return pros;
        }

        public static List<string> PropertyNames<T>(this T obj, bool virtuals = false, bool privates = false)
        {
            List<string> props = new List<string>();

            obj.Properties(virtuals, privates).ForEach(p => props.Add(p.Name));

            return props;
        }
        
        public static object CreateInstance(this PropertyInfo prop)
        {
            if (prop.PropertyType == null) return null;

            //var assembly = prop.PropertyType.GetTypeInfo().Assembly.ToString();
            //var nspace = prop.PropertyType.GetTypeInfo().FullName;
            
            var type = prop.PropertyType.GetTypeInfo().AsType();

            //var handle = Activator.CreateInstance(assembly, nspace);
            var handle = Activator.CreateInstance(type);

            //return handle.Unwrap();
            return handle;
        }

        public static List<string> PropertyNamesTree<T>(this T obj, bool virtuals = false, bool privates = false)
        {
            List<string> props = new List<string>();

            var tree = obj.PropertiesTree(virtuals, privates);

            tree.ForEach(p =>
            {
                if (p.PropertyType != null && !p.PropertyType.Namespace.ToLower().Equals("system"))
                {
                    var instance = p.CreateInstance();
                    if (instance != null)
                        props.AddRange(instance.PropertyNamesTree(virtuals, privates)  );
                }
                else props.Add(p.Name);
            });

            return props;
        }
        
        public static string DelimitedValues<T>(this T obj, string delimiter = ",", bool nullToEmpty = true, bool virtuals = false, bool privates = false)
        {
            string value = String.Empty;

            obj.Properties(virtuals, privates).ForEach((p) =>
            {
                if (p.PropertyType.Namespace.ToLower().Equals("system"))
                {
                    string val = obj.GetVal(p.Name);

                    if ((!val.Empty()) || (val.Empty() && nullToEmpty))
                        value += string.Format("{0}{1}", val.ToEmpty(), delimiter);
                }
            });

            return value;
        }
        
        public static string DelimitedValuesTree<T>(this T obj, string delimiter = ",", bool nullToEmpty = true, bool virtuals = false, bool privates = false)
        {
            string value = DelimitedValuesTree(obj, delimiter, virtuals, privates );

            return value.Substring(0, value.Length - 1);
        }
          

        public static string ClosestFieldName<T>(this T obj, string unAccountedfieldName)
        {
            if (obj == null) return null;

            unAccountedfieldName = unAccountedfieldName.ToLower();
            foreach(var pname in obj.PropertyNames())
            {
                if (unAccountedfieldName.Equals(pname.ToLower())) return pname;
            }

            return null;
        }

        public static string ClosestFieldName(this List<PropertyInfo> props, string unAccountedfieldName)
        {
            if (props == null) return null;

            unAccountedfieldName = unAccountedfieldName.ToLower();
            foreach (var pname in props.Select(p => p.Name))
            {
                if (unAccountedfieldName.Equals(pname.ToLower())) return pname;
            }

            return null;
        }

        public static T GetValue<T>(this object obj, string property, object defaultValue = null)
        {
            return (T)obj.GetType().GetProperty(property).GetValue(obj, null);
        }

        public static string GetVal(this object obj, string property)
        {
            try
            {
                return obj.GetType().GetProperty(property).GetValue(obj, null).ToString();
            }
            catch { }

            return string.Empty;
        }

        public static void SetProperty<T>(this T obj, string propertyName, object propertyvalue) where T : class
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, propertyvalue, null);
            }
        }

        public static dynamic GetPropertyValueViaAttribute<A, T>(this object obj, T argument) where A: Attribute
                                                                                                                    
        {
            // foreach (var property in obj.Properties())
            // {
            //     CustomAttributeData attribute = property.GetCustomAttributeData().FirstOrDefault(a => a.GetType() == typeof(A));
            //     if (attribute != null)
            //     {
            //         if (argument != null)
            //         {                        
            //             CustomAttributeTypedArgument typeArgument = attribute.ConstructorArguments.FirstOrDefault(c => c.GetType() == argument.GetType());
            //             if (typeArgument != null)
            //             {                            
            //                 if (((T)typeArgument.Value).Equals(argument)) return property.GetValue(obj, null);
            //             }
            //         }
            //         else return property.GetValue(obj, null);
            //     }                
            // }

            return null;
        }

        public static string CallerMethodName<T>(this T obj, int level = 3)
        {
            string name = "UnKnown";

            try
            {
                // StackTrace stackTrace = new StackTrace();           // get call stack
                // StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

                // StackFrame callingFrame = stackFrames[level];
                // name = callingFrame.GetMethod().Name;
            }
            catch { }

            return name;
        }

        public static Type CallerType<T>(this T obj, int level = 3)
        {
            Type t = null;

            try
            {
                // StackTrace stackTrace = new StackTrace();           // get call stack
                // StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

                // StackFrame callingFrame = stackFrames[level];
                // t = callingFrame.GetMethod().DeclaringType;
            }
            catch { }

            return t;
        }

        public static string TextIn<T>(this T item, string prefix = "") where T : class
        {
            try
            {
                var properties = typeof(T).GetProperties();
                //Enforce.That(properties.Length == 1);
                return string.Format("{0} {1}: {2}", prefix.ToEmpty(), properties[0].Name, item);
            }
            catch { return string.Format("{0} - {1}", prefix.ToEmpty(), item); }
        }

        public static IList<T> RemoveEx<T>(this IList<T> elements, IList<T> items)
        {
            if (items != null)
                foreach (T item in items) elements.Remove(item);
            
            return elements;
        }

        //public static bool AnyExt<T>(this T item, )

        // public static string UserField<T>(this T obj, bool isAdded = false, IList<string> propertiesToIgnore = null)
        // {
        //     var fields = (List<string>)obj.PropertyNames().RemoveEx<string>(propertiesToIgnore);
        //     string userField = string.Empty;

        //     bool inInserted = fields.Any(f => EntityConstants.Insert_UserFieldNames.Any(u => u.EqualsIgnoreCase(f)));
        //     bool inModified = fields.Any(f => EntityConstants.Update_UserFieldNames.Any(u => u.EqualsIgnoreCase(f)));


        //     fields.Find(f =>
        //     {
        //         if (isAdded && inInserted)
        //         {
        //             if ((userField = EntityConstants.Insert_UserFieldNames.Find(u => u.EqualsIgnoreCase(f))) == null)
        //                 userField = EntityConstants.Update_UserFieldNames.Find(u => u.EqualsIgnoreCase(f));
        //         }
        //         else
        //         {
        //             if ((userField = EntityConstants.Update_UserFieldNames.Find(u => u.EqualsIgnoreCase(f))) == null)
        //                 userField = EntityConstants.Insert_UserFieldNames.Find(u => u.EqualsIgnoreCase(f));
        //         }

        //         return !userField.Empty();
        //     });


        //     return userField;
        // }
        
        public static string JoinExt<T>(this IEnumerable<T> objs, string seprator = " ", bool donotPutEmptyElements = false)
        {
            string joinstr = "";
            if (!objs.Null())
                foreach (T obj in objs)
                    joinstr = string.Format("{0}{1}{2}", joinstr, (donotPutEmptyElements) ? (joinstr.Empty() ? "" : seprator) : seprator, obj);

            return joinstr.Trim().TrimEx(seprator);
        }

    }
}