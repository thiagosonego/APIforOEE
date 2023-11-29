using Dapper;
using OEE.Domain.Interfaces;
using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEE.Infrastructure.Dapper.Queries
{
    public class DowntimeQueries : IDowntimeQueries
    {
        private readonly IConnectionFactory connectionFactory;

        public DowntimeQueries(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<List<DowntimeDBModel>> GetTop3Downtime(string station, string assemblyLine, DateTime dateHour)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT TOP 3                                                   ")
                .AppendLine("	JUSTIFICATIVAS.JUSTIFICATIVA                                ")
                .AppendLine("	,SUM(PARADAS.TEMPO_PARADA) AS TEMPO_PARADA                  ")
                .AppendLine("	,JUSTIFICATIVAS.NIVEL_1                                     ")
                .AppendLine("	,JUSTIFICATIVAS.NIVEL_2                                     ")
                .AppendLine("	FROM WSQOWCMICROPARADASJUSTIFICATIVA AS JUSTIFICATIVAS      ")
                .AppendLine("INNER JOIN WSQOWCMICROPARADAS AS PARADAS                       ")
                .AppendLine("ON JUSTIFICATIVAS.ID_MICROPARADAS = PARADAS.ID                 ")
                .AppendLine("WHERE JUSTIFICATIVAS.ESTACAO = @ESTACAO AND                    ")
                .AppendLine("	JUSTIFICATIVAS.LINHA_MONTAGEM = @LINHA_MONTAGEM AND         ")
                .AppendLine("	(PARADAS.DATA_OCORRENCIA_FINAL >= @DATA_HORA OR             ")
                .AppendLine("	PARADAS.DATA_OCORRENCIA_FINAL != NULL)                      ")
                .AppendLine("	GROUP BY JUSTIFICATIVAS.JUSTIFICATIVA                       ")
                .AppendLine("	,JUSTIFICATIVAS.NIVEL_1                                     ")
                .AppendLine("	,JUSTIFICATIVAS.NIVEL_2                                     ")
                .AppendLine("	ORDER BY TEMPO_PARADA DESC                                  ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@ESTACAO", station);
            parameters.Add("@LINHA_MONTAGEM", assemblyLine);
            parameters.Add("@DATA_HORA", dateHour);

            var result = (await connection.QueryAsync<DowntimeDBModel>(query.ToString(), parameters)).ToList();

            connection.Close();

            return result;
        }
    }
}
