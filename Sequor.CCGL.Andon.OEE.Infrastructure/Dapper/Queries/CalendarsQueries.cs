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
    public class CalendarsQueries : ICalendarsQueries
    {
        private readonly IConnectionFactory connectionFactory;

        public CalendarsQueries(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<string> GetEsquema(string station, string assemblyLine)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT                                 ")
                .AppendLine("	ESQUEMA_CALENDARIO                  ")
                .AppendLine("FROM                                   ")
                .AppendLine("	WSQOPCP2PARESTACAO WITH(NOLOCK)     ")
                .AppendLine("WHERE                                  ")
                .AppendLine("	ESTACAO = @ESTACAO AND              ")
                .AppendLine("	LINHA_MONTAGEM = @LINHA_MONTAGEM    ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@ESTACAO", station);
            parameters.Add("@LINHA_MONTAGEM", assemblyLine);

            var result = (await connection.QueryAsync<SchemeDBModel>(query.ToString(), parameters)).FirstOrDefault();

            connection.Close();

            return result.scheme;
        }

        public async Task<DateProcessAndTurnModel> GetDateProcessAndTurn(string scheme)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT                                                     ")
                .AppendLine("	DATA_PROCESSO                                           ")
                .AppendLine("	,TURNO                                                  ")
                .AppendLine("FROM                                                       ")
                .AppendLine("	WSQOCADDIASCALENDARIO WITH(NOLOCK)                      ")
                .AppendLine("WHERE                                                      ")
                .AppendLine("	ESQUEMA = @ESQUEMA AND                                  ")
                .AppendLine("	GETDATE() between DATA_INICIAL and DATA_FINAL           ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@ESQUEMA", scheme);

            var result = (await connection.QueryAsync<DateProcessAndTurnModel>(query.ToString(), parameters)).FirstOrDefault();

            connection.Close();

            return result;
        }

        public async Task<List<DateProcessAndTurnModel>> GetCalendarByWeekForActualOEE(string esquema, DateTime dateProcess)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT                                                         ")
                .AppendLine("	DATA_PROCESSO                                               ")
                .AppendLine("	,TURNO                                                      ")
                .AppendLine("FROM                                                           ")
                .AppendLine("	WSQOCADDIASCALENDARIO WITH(NOLOCK)                          ")
                .AppendLine("WHERE                                                          ")
                .AppendLine("	ESQUEMA = @ESQUEMA AND                                      ")
                .AppendLine("	DATA_PROCESSO between                                       ")
                .AppendLine("	dateadd(week, datediff(week, 0, @DATA_PROCESSO -1), 0) and  ")
                .AppendLine("	dateadd(week, datediff(week, 0, @DATA_PROCESSO -1), 6)      ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@ESQUEMA", esquema);
            parameters.Add("@DATA_PROCESSO", dateProcess);

            List<DateProcessAndTurnModel> result = (await connection.QueryAsync<DateProcessAndTurnModel>(query.ToString(), parameters)).ToList();

            connection.Close();

            return result;
        }

        public async Task<List<DateProcessAndTurnModel>> GetCalendarByMonthForActualOEE(string esquema, DateTime dateProcess)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT                                                         ")
                .AppendLine("	DATA_PROCESSO                                               ")
                .AppendLine("	,TURNO                                                      ")
                .AppendLine("FROM                                                           ")
                .AppendLine("	WSQOCADDIASCALENDARIO WITH(NOLOCK)                          ")
                .AppendLine("WHERE                                                          ")
                .AppendLine("	ESQUEMA = @ESQUEMA AND                                      ")
                .AppendLine("	DATA_PROCESSO between                                       ")
                .AppendLine("	dateadd(MONTH, datediff(MONTH, 0, @DATA_PROCESSO), 0) and   ")
                .AppendLine("	EOMONTH(@DATA_PROCESSO,0)                                   ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@ESQUEMA", esquema);
            parameters.Add("@DATA_PROCESSO", dateProcess);

            List<DateProcessAndTurnModel> result = (await connection.QueryAsync<DateProcessAndTurnModel>(query.ToString(), parameters)).ToList();

            connection.Close();

            return result;
        }

        public async Task<List<DateProcessAndTurnModel>> GetCalendarByTurnForLastOEE(string esquema)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT TOP 2                           ")
                .AppendLine("	DATA_PROCESSO                       ")
                .AppendLine("	,TURNO                              ")
                .AppendLine("	,MAX(DATA_FINAL) AS DATA_FINAL      ")
                .AppendLine("FROM                                   ")
                .AppendLine("	WSQOCADDIASCALENDARIO WITH(NOLOCK)  ")
                .AppendLine("WHERE                                  ")
                .AppendLine("	ESQUEMA = @ESQUEMA AND              ")
                .AppendLine("	DATA_FINAL < GETDATE()              ")
                .AppendLine("	GROUP BY TURNO, DATA_PROCESSO       ")
                .AppendLine("	ORDER BY DATA_FINAL ASC             ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@ESQUEMA", esquema);

            List<DateProcessAndTurnForLastTurnsModel> results = (await connection.QueryAsync<DateProcessAndTurnForLastTurnsModel>(query.ToString(), parameters))
                .OrderBy(x => x.dateEnd).ToList();

            connection.Close();

            List<DateProcessAndTurnModel> dateProcessAndTurnList = new List<DateProcessAndTurnModel>();
            foreach (var result in results)
            {
                DateProcessAndTurnModel dateProcessAndTurn = new DateProcessAndTurnModel();
                dateProcessAndTurn.dateProcess = result.dateProcess;
                dateProcessAndTurn.turn = result.turn;
                dateProcessAndTurnList.Add(dateProcessAndTurn);
            }

            return dateProcessAndTurnList;
        }

        public async Task<List<DateProcessAndTurnModel>> GetCalendarByDayForLastOEE(string esquema, DateTime dateProcess)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT TOP 2                           ")
                .AppendLine("	DATA_PROCESSO                       ")
                .AppendLine("	,MAX(TURNO) AS TURNO                ")
                .AppendLine("FROM                                   ")
                .AppendLine("	WSQOCADDIASCALENDARIO WITH(NOLOCK)  ")
                .AppendLine("WHERE                                  ")
                .AppendLine("	ESQUEMA = @ESQUEMA AND              ")
                .AppendLine("	DATA_PROCESSO < @DATA_PROCESSO      ")
                .AppendLine("	GROUP BY DATA_PROCESSO              ")
                .AppendLine("	ORDER BY MAX(DATA_FINAL) DESC       ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@ESQUEMA", esquema);
            parameters.Add("@DATA_PROCESSO", dateProcess);

            List<DateProcessAndTurnModel> results = (await connection.QueryAsync<DateProcessAndTurnModel>(query.ToString(), parameters))
                .OrderByDescending(x => x.dateProcess).ToList();

            connection.Close();

            List<DateProcessAndTurnModel> dateProcessAndTurnList = new List<DateProcessAndTurnModel>();
            foreach (var result in results)
            {
                DateProcessAndTurnModel dateProcessAndTurn = new DateProcessAndTurnModel();
                dateProcessAndTurn.dateProcess = result.dateProcess;
                dateProcessAndTurn.turn = result.turn;
                dateProcessAndTurnList.Add(dateProcessAndTurn);
            }

            return dateProcessAndTurnList;
        }

        public async Task<List<DateStartAndEndModel>> GetCalendarByTurnsForDowntime(string esquema, int Quantity)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT TOP (@QUANTITY)                 ")
                .AppendLine("	MIN(DATA_INICIAL) AS dateStart      ")
                .AppendLine("	,MAX(DATA_FINAL) AS dateEnd         ")
                .AppendLine("FROM                                   ")
                .AppendLine("	WSQOCADDIASCALENDARIO WITH(NOLOCK)  ")
                .AppendLine("WHERE                                  ")
                .AppendLine("	ESQUEMA = @ESQUEMA AND              ")
                .AppendLine("	DATA_INICIAL < GETDATE()            ")
                .AppendLine("	GROUP BY TURNO, DATA_PROCESSO       ")
                .AppendLine("	ORDER BY dateEnd ASC                ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@QUANTITY", Quantity);
            parameters.Add("@ESQUEMA", esquema);

            List<DateStartAndEndModel> results = (await connection.QueryAsync<DateStartAndEndModel>(query.ToString(), parameters)).OrderBy(x => x.dateStart).ToList();

            connection.Close();

            return results;
        }

        public async Task<List<DateStartAndEndModel>> GetCalendarByDayForDowntime(string esquema, DateTime dateProcess, int Quantity)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT TOP (@QUANTITY)                 ")
                .AppendLine("	MIN(DATA_INICIAL) AS dateStart      ")
                .AppendLine("	,MAX(DATA_FINAL) AS dateEnd         ")
                .AppendLine("FROM                                   ")
                .AppendLine("	WSQOCADDIASCALENDARIO WITH(NOLOCK)  ")
                .AppendLine("WHERE                                  ")
                .AppendLine("	ESQUEMA = @ESQUEMA AND              ")
                .AppendLine("	DATA_PROCESSO <= @DATA_PROCESSO     ")
                .AppendLine("	GROUP BY DATA_PROCESSO              ")
                .AppendLine("	ORDER BY dateEnd ASC                ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@QUANTITY", Quantity);
            parameters.Add("@ESQUEMA", esquema);
            parameters.Add("@DATA_PROCESSO", dateProcess);

            List<DateStartAndEndModel> results = (await connection.QueryAsync<DateStartAndEndModel>(query.ToString(), parameters)).OrderBy(x => x.dateStart).ToList();

            connection.Close();

            return results;
        }

        public async Task<List<DateStartAndEndForWeeksModel>> GetCalendarByWeekForLastOEEAndDowntime(string esquema, DateTime dateProcess, int Quantity = 2)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT                                                                     ")
                .AppendLine("	MIN(DATA_PROCESSO) AS dateStart                                         ")
                .AppendLine("	,MAX(DATA_PROCESSO) AS dateEnd                                          ")
                .AppendLine("	,DATEPART(week, MAX(DATA_PROCESSO)-1) AS weeks                          ")
                .AppendLine("FROM                                                                       ")
                .AppendLine("	WSQOCADDIASCALENDARIO WITH(NOLOCK)                                      ")
                .AppendLine("WHERE                                                                      ")
                .AppendLine("	ESQUEMA = @ESQUEMA AND                                                  ")
                .AppendLine("	DATA_PROCESSO between                                                   ")
                .AppendLine("	dateadd(week, datediff(week, 0, @DATA_PROCESSO - 1)-@QUANTITY, 0) and   ")
                .AppendLine("	dateadd(week, datediff(week, 0, @DATA_PROCESSO - 1), 6)                 ")
                .AppendLine("	GROUP BY dateadd(week, datediff(week, 0, DATA_PROCESSO - 1), 0)         ")
                .AppendLine("	ORDER BY dateEnd DESC                                                   ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@ESQUEMA", esquema);
            parameters.Add("@DATA_PROCESSO", dateProcess);
            parameters.Add("@QUANTITY", Quantity);

            List<DateStartAndEndForWeeksModel> results = (await connection.QueryAsync<DateStartAndEndForWeeksModel>(query.ToString(), parameters)).OrderBy(x => x.dateStart).ToList();

            connection.Close();

            return results;
        }

        public async Task<List<DateStartAndEndModel>> GetCalendarByMonthForLastOEEAndDowntime(string esquema, DateTime dateProcess, int Quantity = 2)
        {
            var query = new StringBuilder()
                .AppendLine("SELECT                                                                     ")
                .AppendLine("	MIN(DATA_PROCESSO) AS dateStart                                         ")
                .AppendLine("	,MAX(DATA_PROCESSO) AS dateEnd                                          ")
                .AppendLine("FROM                                                                       ")
                .AppendLine("	WSQOCADDIASCALENDARIO WITH(NOLOCK)                                      ")
                .AppendLine("WHERE                                                                      ")
                .AppendLine("	ESQUEMA = @ESQUEMA AND                                                  ")
                .AppendLine("	DATA_PROCESSO between                                                   ")
                .AppendLine("	dateadd(MONTH, datediff(MONTH, 0, @DATA_PROCESSO)-@QUANTITY, 0) and     ")
                .AppendLine("	EOMONTH(@DATA_PROCESSO, 0)                                              ")
                .AppendLine("	GROUP BY dateadd(MONTH, datediff(MONTH, 0, DATA_PROCESSO), 0)           ")
                .AppendLine("	ORDER BY dateEnd ASC                                                    ");

            using var connection = this.connectionFactory.GetConnection();

            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@ESQUEMA", esquema);
            parameters.Add("@DATA_PROCESSO", dateProcess);
            parameters.Add("@QUANTITY", Quantity);

            List<DateStartAndEndModel> results = (await connection.QueryAsync<DateStartAndEndModel>(query.ToString(), parameters)).OrderBy(x => x.dateStart).ToList();

            connection.Close();

            return results;
        }
    }
}
