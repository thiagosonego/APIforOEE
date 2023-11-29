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
    public class OEEQueries : IOEEQueries
    {
        private readonly IConnectionFactory connectionFactory;

        public OEEQueries(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<List<OEEDBModel>> GetActualOEEsByHour(string station, string assemblyLine, DateTime dateHour)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT 	                                ")
                .AppendLine("	ESTACAO                                 ")
                .AppendLine("	,PECAS_OK                               ")
                .AppendLine("	,PECAS_REFUGADAS                        ")
                .AppendLine("	,TEMPO_DISPONIVEL_TURNO                 ")
                .AppendLine("	,TEMPO_PARADA_SEM_APONTAMENTO           ")
                .AppendLine("	,TEMPO_PARADA                           ")
                .AppendLine("	,TEMPO_QUEBRADO                         ")
                .AppendLine("	,META_PRODUCAO                          ")
                .AppendLine("FROM	                                    ")
                .AppendLine("	WSQOWCPRODUCAO WITH(NOLOCK)             ")
                .AppendLine("WHERE                                      ")
                .AppendLine("	ESTACAO = @ESTACAO AND                  ")
                .AppendLine("	LINHA_MONTAGEM = @LINHA_MONTAGEM AND    ")
                .AppendLine("	@DATA_HORA BETWEEN                      ")
                .AppendLine("	DATA_HORA_INICIAL AND DATA_HORA_FINAL   ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@ESTACAO", station);
            parameters.Add("@LINHA_MONTAGEM", assemblyLine);
            parameters.Add("@DATA_HORA", dateHour);

            var result = await connection.QueryAsync<OEEDBModel>(query.ToString(), parameters);

            connection.Close();

            return result.ToList();
        }

        public async Task<List<OEEDBModel>> GetActualOEEsByTurns(string station, string assemblyLine, DateTime dateProcess, string turn)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT 	                                ")
                .AppendLine("	ESTACAO                                 ")
                .AppendLine("	,PECAS_OK                               ")
                .AppendLine("	,PECAS_REFUGADAS                        ")
                .AppendLine("	,TEMPO_DISPONIVEL_TURNO                 ")
                .AppendLine("	,TEMPO_PARADA_SEM_APONTAMENTO           ")
                .AppendLine("	,TEMPO_PARADA                           ")
                .AppendLine("	,TEMPO_QUEBRADO                         ")
                .AppendLine("	,META_PRODUCAO                          ")
                .AppendLine("FROM	                                    ")
                .AppendLine("	WSQOWCPRODUCAO WITH(NOLOCK)             ")
                .AppendLine("WHERE                                      ")
                .AppendLine("	ESTACAO = @ESTACAO AND                  ")
                .AppendLine("	LINHA_MONTAGEM = @LINHA_MONTAGEM AND    ")
                .AppendLine("	DATA_PROCESSO = @DATA_PROCESSO AND      ")
                .AppendLine("	TURNO = @TURNO                          ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@ESTACAO", station);
            parameters.Add("@LINHA_MONTAGEM", assemblyLine);
            parameters.Add("@DATA_PROCESSO", dateProcess);
            parameters.Add("@TURNO", turn);

            var result = await connection.QueryAsync<OEEDBModel>(query.ToString(), parameters);

            connection.Close();

            return result.ToList();
        }

        public async Task<List<OEEDBModel>> GetActualOEEsByDay(string station, string assemblyLine, DateTime dateProcess)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT 	                                ")
                .AppendLine("	ESTACAO                                 ")
                .AppendLine("	,PECAS_OK                               ")
                .AppendLine("	,PECAS_REFUGADAS                        ")
                .AppendLine("	,TEMPO_DISPONIVEL_TURNO                 ")
                .AppendLine("	,TEMPO_PARADA_SEM_APONTAMENTO           ")
                .AppendLine("	,TEMPO_PARADA                           ")
                .AppendLine("	,TEMPO_QUEBRADO                         ")
                .AppendLine("	,META_PRODUCAO                          ")
                .AppendLine("FROM	                                    ")
                .AppendLine("	WSQOWCPRODUCAO WITH(NOLOCK)             ")
                .AppendLine("WHERE                                      ")
                .AppendLine("	ESTACAO = @ESTACAO AND                  ")
                .AppendLine("	LINHA_MONTAGEM = @LINHA_MONTAGEM AND    ")
                .AppendLine("	DATA_PROCESSO = @DATA_PROCESSO          ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@ESTACAO", station);
            parameters.Add("@LINHA_MONTAGEM", assemblyLine);
            parameters.Add("@DATA_PROCESSO", dateProcess);

            var result = await connection.QueryAsync<OEEDBModel>(query.ToString(), parameters);

            connection.Close();

            return result.ToList();
        }

        public async Task<List<OEEDBModel>> GetActualOEEsByDays(string station, string assemblyLine, DateTime dateStart, DateTime dateEnd)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT 	                                ")
                .AppendLine("	ESTACAO                                 ")
                .AppendLine("	,PECAS_OK                               ")
                .AppendLine("	,PECAS_REFUGADAS                        ")
                .AppendLine("	,TEMPO_DISPONIVEL_TURNO                 ")
                .AppendLine("	,TEMPO_PARADA_SEM_APONTAMENTO           ")
                .AppendLine("	,TEMPO_PARADA                           ")
                .AppendLine("	,TEMPO_QUEBRADO                         ")
                .AppendLine("	,META_PRODUCAO                          ")
                .AppendLine("FROM	                                    ")
                .AppendLine("	WSQOWCPRODUCAO WITH(NOLOCK)             ")
                .AppendLine("WHERE                                      ")
                .AppendLine("	ESTACAO = @ESTACAO AND                  ")
                .AppendLine("	LINHA_MONTAGEM = @LINHA_MONTAGEM        ")
                .AppendLine("	AND DATA_PROCESSO BETWEEN               ")
                .AppendLine("	@DATA_INICIAL AND                       ")
                .AppendLine("	@DATA_FINAL                             ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@ESTACAO", station);
            parameters.Add("@LINHA_MONTAGEM", assemblyLine);
            parameters.Add("@DATA_INICIAL", dateStart);
            parameters.Add("@DATA_FINAL", dateEnd);

            var result = await connection.QueryAsync<OEEDBModel>(query.ToString(), parameters);

            connection.Close();

            return result.ToList();
        }
    }
}
