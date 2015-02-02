using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Connection;
using NHibernate.Mapping.ByCode;

namespace Goleak.Infra.Banco
{
    public abstract class BancoConfiguracao
    {
        protected BancoConfiguracao(string connectionString, SchemaAutoAction schemaAction)
        {
            ConnectionString = connectionString;
            SchemaAction = schemaAction;
        }

        public string ConnectionString { get; private set; }
        public SchemaAutoAction SchemaAction { get; private set; }

        public ISessionFactory ConstruirSessionFactory()
        {
            var cfg = new Configuration();

            cfg.DataBaseIntegration(db =>
                                        {
                                            db.ConnectionProvider<DriverConnectionProvider>();
                                            db.ConnectionString = ConnectionString;

                                            db.LogFormattedSql = true;

                                            ConfigurarBanco(db);
                                        });

            cfg.AddMapping(CompilarMapeamentos());

            return cfg.BuildSessionFactory();
        }

        protected static HbmMapping CompilarMapeamentos()
        {
            var mapper = new ModelMapper();

            mapper.AddMappings(typeof (BancoConfiguracao).Assembly.GetExportedTypes());

            return mapper.CompileMappingForAllExplicitlyAddedEntities();
        }

        protected abstract void ConfigurarBanco(IDbIntegrationConfigurationProperties db);
    }
}