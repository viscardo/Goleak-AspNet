using System;
using System.Configuration;
using NHibernate.Cfg;

namespace Goleak.Infra.Banco
{
    public static class BancoConfiguracaoFactory
    {
        public static BancoConfiguracao CriarBancoConfiguracao()
        {
            var connectionStringSetting = ConfigurationManager.ConnectionStrings["BD"];
            var schemaAction = ConfigurationManager.AppSettings["schemaAction"];

            var schemaAutoAction = string.IsNullOrEmpty(schemaAction)
                                                    ? null
                                                    : SchemaAutoActionFactory.Criar(schemaAction);

            if (connectionStringSetting.ProviderName == "NHibernate.Connection.MySqlDataDriver")
                return new MySQLBancoConfiguracao(connectionStringSetting.ConnectionString, schemaAutoAction);
            else if (connectionStringSetting.ProviderName == "NHibernate.Connection.SqlClientDriver")
                return new SqlServerBancoConfiguracao(connectionStringSetting.ConnectionString, schemaAutoAction);

            throw new NotSupportedException(string.Format("O provider \"{0}\" não é suportado.",
                                                          connectionStringSetting.ProviderName));
        }

        private static class SchemaAutoActionFactory
        {
            public static SchemaAutoAction Criar(string valor)
            {
                switch (valor)
                {
                    case "criar":
                        return SchemaAutoAction.Create;
                    case "recriar":
                        return SchemaAutoAction.Recreate;
                    case "atualizar":
                        return SchemaAutoAction.Update;
                    case "validar":
                        return SchemaAutoAction.Validate;
                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}