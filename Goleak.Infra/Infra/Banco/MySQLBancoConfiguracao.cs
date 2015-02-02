using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using System.IO;

namespace Goleak.Infra.Banco
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ce")]
    public class MySQLBancoConfiguracao : BancoConfiguracao
    {
        public MySQLBancoConfiguracao(string connectionString, SchemaAutoAction schemaAction)
            : base(connectionString, schemaAction)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected override void ConfigurarBanco(IDbIntegrationConfigurationProperties db)
        {
            db.Dialect<MySQL5Dialect>();
            db.Driver<MySqlDataDriver>();
        }
    }
}