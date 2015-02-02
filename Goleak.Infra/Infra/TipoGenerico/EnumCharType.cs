using System;
using System.Data;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;


namespace Goleak.Infra.TipoGenerico
{
    [Serializable]
    public abstract class EnumCharType : ImmutableType, IDiscriminatorType
    {
        private readonly Type enumClass;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected EnumCharType(Type enumClass)
            : base(new StringFixedLengthSqlType(1))
        {
            if (enumClass.IsEnum)
            {
                this.enumClass = enumClass;
            }
            else
            {
                throw new MappingException(enumClass.Name + " did not inherit from System.Enum");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public virtual object GetInstance(object code)
        {
            if (code is String)
            {
                return GetInstanceFromString((String)code);
            }
            if (code is Char)
            {
                return GetInstanceFromChar((Char)code);
            }
            throw new HibernateException(string.Format("Can't Parse {0} as {1}", code, enumClass.Name));
        }

        private object GetInstanceFromString(String s)
        {
            if (s.Length == 0) throw new HibernateException(string.Format("Can't Parse empty string as {0}", enumClass.Name));

            if (s.Length == 1)
            {
                //String representation of underlying char value e.g. "R"
                return GetInstanceFromChar(s[0]);
            }
            //Name of enum value e.g. "Red"
            try
            {
                return Enum.Parse(enumClass, s, false);
            }
            catch (ArgumentException)
            {
                try
                {
                    return Enum.Parse(enumClass, s, true);
                }
                catch (ArgumentException ae)
                {
                    throw new HibernateException(string.Format("Can't Parse {0} as {1}", s, enumClass.Name), ae);
                }
            }
        }

        private object GetInstanceFromChar(Char c)
        {
            object instance = Enum.ToObject(enumClass, c);
            if (Enum.IsDefined(enumClass, instance)) return instance;

            instance = Enum.ToObject(enumClass, Alternate(c));
            if (Enum.IsDefined(enumClass, instance)) return instance;

            throw new HibernateException(string.Format("Can't Parse {0} as {1}", c, enumClass.Name));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.Char.ToUpper(System.Char)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.Char.ToLower(System.Char)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private Char Alternate(Char c)
        {
            return Char.IsUpper(c) ? Char.ToLower(c) : Char.ToUpper(c);
        }

        /// <summary>
        /// Converts the given enum instance into a basic type.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual object GetValue(object instance)
        {
            if (instance == null)
            {
                return null;
            }

            if (instance.GetType() == typeof(string))
            {
                return ((char)(int)Enum.Parse(enumClass, (string)instance, true));
            }
            return (Char)(Int32)instance;
        }

        public override Type ReturnedClass
        {
            get { return enumClass; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public override void Set(IDbCommand cmd, object value, int index)
        {
            IDataParameter par = (IDataParameter)cmd.Parameters[index];
            if (value == null)
            {
                par.Value = DBNull.Value;
            }
            else
            {
                if (value.GetType() == typeof(string))
                {
                    par.Value = ((char)(int)Enum.Parse(enumClass, (string)value, true));
                }
                else
                {
                    par.Value = ((Char)(Int32)(value)).ToString();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public override object Get(IDataReader rs, int index)
        {
            object code = rs[index];
            if (code == DBNull.Value || code == null)
            {
                return null;
            }
            return GetInstance(code);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public override object Get(IDataReader rs, string name)
        {
            return Get(rs, rs.GetOrdinal(name));
        }

        public override string Name
        {
            get { return "enumchar - " + enumClass.Name; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
        public override string ToString(object value)
        {
            return (value == null) ? null : GetValue(value).ToString();
        }

        public override object Assemble(object cached, ISessionImplementor session, object owner)
        {
            if (cached == null)
            {
                return null;
            }
            return GetInstance(cached);
        }

        public override object Disassemble(object value, ISessionImplementor session, object owner)
        {
            return (value == null) ? null : GetValue(value);
        }

        public virtual object StringToObject(string xml)
        {
            return (string.IsNullOrEmpty(xml)) ? null : FromStringValue(xml);
        }

        public override object FromStringValue(string xml)
        {
            return GetInstance(xml);
        }

        public string ObjectToSQLString(object value, Dialect dialect)
        {
            return '\'' + GetValue(value).ToString() + '\'';
        }
    }
}
