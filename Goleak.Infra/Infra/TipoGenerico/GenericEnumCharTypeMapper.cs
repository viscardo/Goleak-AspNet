using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Goleak.Infra.TipoGenerico
{
    public class GenericEnumCharTypeMapper<TEnum> : EnumCharType
    {
        public GenericEnumCharTypeMapper()
            : base(typeof(TEnum))
        {
        }
    }
}
