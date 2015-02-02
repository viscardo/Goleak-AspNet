using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Goleak.Infra.TipoGenerico
{
    public class SimNaoType : IUserType
    {
        public bool IsMutable
        {
            get { return false; }
        }

        public Type ReturnedType
        {
            get { return typeof(Boolean); }
        }

        public SqlType[] SqlTypes
        {
            get { return new[] { NHibernateUtil.String.SqlType }; }
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var obj = NHibernateUtil.String.NullSafeGet(rs, names[0]);

            if (obj == null) return null;

            var yesNo = (string)obj;

            if (yesNo != "S" && yesNo != "N")
                throw new Exception(string.Format(
                    "Expected data to be 'S' or 'N' but was '{0}'.", yesNo));

            return yesNo == "S";
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null)
            {
                ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
            }
            else
            {
                var yes = Convert.ToBoolean(value);

                ((IDataParameter)cmd.Parameters[index]).Value = yes ? "S" : "N";
            }
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public new bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y)) return true;

            if (x == null || y == null) return false;

            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            return x == null ? typeof(bool).GetHashCode() + 473 : x.GetHashCode();
        }
    }
}
