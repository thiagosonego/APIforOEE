using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OEE.Application.Enums;
using OEE.Domain.Interfaces;
using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OEE.Application.Commands.Handlers
{
    public class LastOEEHandler : IRequestHandler<LastOEE, List<LastOEEModel>>
    {
        private readonly ILogger<ActualOEEHandler> logger;
        private readonly IOEERepository OEERepository;
        private readonly ICalendarsRepository calendarsRepository;
        public string Scheme;
        public List<DateStartAndEndForWeeksModel> dateStartAndEnd = new List<DateStartAndEndForWeeksModel>();
        public List<DateProcessAndTurnModel> dateProcessAndTurn = new List<DateProcessAndTurnModel>();

        public LastOEEHandler(ILogger<ActualOEEHandler> logger, IOEERepository actualOEERepository, ICalendarsRepository calendarsRepository)
        {
            this.logger = logger;
            this.OEERepository = actualOEERepository;
            this.calendarsRepository = calendarsRepository;
        }

        public async Task<List<LastOEEModel>> Handle(LastOEE LastOEE, CancellationToken cancellationToken)
        {
            logger.LogInformation($"- Handling Start Last OEE {LastOEE.LastOEERequest}");

            var LastOEERequest = LastOEE.LastOEERequest;

            string scheme;

            List<LastOEEModel> productionEntities = new List<LastOEEModel>();

            foreach (var stationRequest in LastOEERequest.StationRequest)
            {
                scheme = await calendarsRepository.GetScheme(stationRequest.Station, stationRequest.AssemblyLine);
                if (string.IsNullOrEmpty(this.Scheme) || this.Scheme != scheme)
                {
                    this.Scheme = scheme;

                    dateProcessAndTurn = new List<DateProcessAndTurnModel>();
                    dateProcessAndTurn.Add(await calendarsRepository.GetDateProcessAndTurn(this.Scheme));

                    if (!await SetDateTimeProcess(stationRequest, LastOEERequest.Period, scheme))
                        throw new Exception("There is a problem with the calendar, the API wasn't able to retrive it!");
                }

                LastOEEModel stationProduction = new LastOEEModel();
                stationProduction.StationName = stationRequest.Station;

                for (int indexer = 0; indexer < 3; indexer++)
                {
                    List<OEEDBModel> actualOEEs = new List<OEEDBModel>();
                    actualOEEs = await GetProductions(stationRequest, LastOEERequest.Period, indexer);
                    if(actualOEEs.Count != 0)
                        stationProduction.LastPeriods.Add(SetProductionByStation(actualOEEs, LastOEERequest.Period, indexer));
                }

                productionEntities.Add(stationProduction);
            }

            logger.LogInformation($"- Handling End Last OEE {LastOEE.LastOEERequest}");

            return productionEntities;
        }

        public async Task<List<OEEDBModel>> GetProductions(StationRequestModel station, string period, int indexer)
        {
            var Period = (PeriodEnum.Period)Enum.Parse(typeof(PeriodEnum.Period), period);
            switch (Period)
            {
                case PeriodEnum.Period.HORA:
                    return (await OEERepository.GetOEEHour(station.Station, station.AssemblyLine, DateTime.Now.AddHours(-indexer)));
                case PeriodEnum.Period.TURNO:
                    if (dateProcessAndTurn.Count > indexer)
                        return (await OEERepository.GetOEETurn(station.Station, station.AssemblyLine, dateProcessAndTurn[indexer].dateProcess, dateProcessAndTurn[indexer].turn));
                    break;
                case PeriodEnum.Period.DIA:
                    if (dateProcessAndTurn.Count > indexer)
                        return (await OEERepository.GetOEEDay(station.Station, station.AssemblyLine, dateProcessAndTurn[indexer].dateProcess));
                    break;
                case PeriodEnum.Period.SEMANA:
                    if (dateStartAndEnd.Count > indexer)
                        return (await OEERepository.GetOEEDays(station.Station, station.AssemblyLine, dateStartAndEnd[indexer].dateStart, dateStartAndEnd[indexer].dateEnd));
                    break;
                case PeriodEnum.Period.MES:
                    if (dateStartAndEnd.Count > indexer)
                        return (await OEERepository.GetOEEDays(station.Station, station.AssemblyLine, dateStartAndEnd[indexer].dateStart, dateStartAndEnd[indexer].dateEnd));
                    break;
            }
            return new List<OEEDBModel>();
        }

        public async Task<bool> SetDateTimeProcess(StationRequestModel station, string period, string esquema)
        {
            var Period = (PeriodEnum.Period)Enum.Parse(typeof(PeriodEnum.Period), period);
            switch (Period)
            {
                case PeriodEnum.Period.HORA:
                    return true;
                case PeriodEnum.Period.TURNO:
                    return await GetCalendarByTurn(esquema);
                case PeriodEnum.Period.DIA:
                    return await GetCalendarByDay(esquema);
                case PeriodEnum.Period.SEMANA:
                    return await GetCalendarByWeek(esquema);
                case PeriodEnum.Period.MES:
                    return await GetCalendarByMonth(esquema);
                default:
                    return false;
            }
        }

        public async Task<bool> GetCalendarByTurn(string esquema)
        {
            dateProcessAndTurn.AddRange(await calendarsRepository.GetCalendarByTurnForLastOEE(esquema));
            return true;
        }

        public async Task<bool> GetCalendarByDay(string esquema)
        {
            dateProcessAndTurn.AddRange(await calendarsRepository.GetCalendarByDayForLastOEE(esquema, dateProcessAndTurn.FirstOrDefault().dateProcess));
            return true;
        }

        public async Task<bool> GetCalendarByWeek(string esquema)
        {
            dateStartAndEnd = await calendarsRepository.GetCalendarByWeekForLastOEEAndDowntime(esquema, dateProcessAndTurn.FirstOrDefault().dateProcess);
            return true;
        }

        public async Task<bool> GetCalendarByMonth(string esquema)
        {
            var calendars = await calendarsRepository.GetCalendarByMonthForLastOEEAndDowntime(esquema, dateProcessAndTurn.FirstOrDefault().dateProcess);
            SetDatesForMonth(calendars);
            return true;
        }

        public void SetDatesForMonth(List<DateStartAndEndModel> calendars)
        {
            dateStartAndEnd = new List<DateStartAndEndForWeeksModel>();
            foreach (var calendar in calendars)
            {
                DateStartAndEndForWeeksModel dates = new DateStartAndEndForWeeksModel();
                dates.dateStart = calendar.dateStart;
                dates.dateEnd = calendar.dateEnd;
                dateStartAndEnd.Add(dates);
            }
        }

        public LastPeriodsModel SetProductionByStation(List<OEEDBModel> actualOEEs, string period, int indexer)
        {
            LastPeriodsModel production = new LastPeriodsModel();

            double PECAS_OK = actualOEEs.Sum(x => x.partsOk);
            double PECAS_REFUGO = actualOEEs.Sum(x => x.partsRefused);
            double PECAS_RETRABALHADAS = actualOEEs.Sum(x => x.partsReworked);
            double TEMPO_DISPONIVEL_TURNO = actualOEEs.Sum(x => x.timeAvailableTurn);
            double TEMPO_QUEBRADO = actualOEEs.Sum(x => x.timeBroken);
            double TEMPO_PARADA = actualOEEs.Sum(x => x.timeStop);
            double TEMPO_PARADA_SEM_APONTAMENTO = actualOEEs.Sum(x => x.timeStopWithoutDescription);
            double META_PRODUCAO = actualOEEs.Sum(x => x.goalProduction);

            double PecasProduzidas = PECAS_OK + PECAS_REFUGO + PECAS_RETRABALHADAS;
            double TempoPlanejado = TEMPO_DISPONIVEL_TURNO - TEMPO_QUEBRADO;
            double TempoParadas = TEMPO_PARADA_SEM_APONTAMENTO + TEMPO_PARADA;
            double TempoEfetivo = TempoPlanejado - TempoParadas;

            double quality = (PecasProduzidas > 0) ? PECAS_OK / PecasProduzidas : 0;

            double disponibility = (TempoPlanejado > 0) ? TempoEfetivo / TempoPlanejado : 0;

            double performance = (META_PRODUCAO > 0) ? PecasProduzidas / META_PRODUCAO : 0;

            production.Result = (int)((quality * disponibility * performance) * 100);
            production.Description = SetDescription(period, indexer);

            return production;
        }

        public string SetDescription(string period, int indexer)
        {
            var Period = (PeriodEnum.Period)Enum.Parse(typeof(PeriodEnum.Period), period);
            switch (Period)
            {
                case PeriodEnum.Period.HORA:
                    var dateHour = DateTime.Now.AddHours(-indexer);
                    return $"{dateHour.Hour:00}h {dateHour.Day:00}";
                case PeriodEnum.Period.TURNO:
                    return $"T: {dateProcessAndTurn[indexer].turn.Substring(6)} {dateProcessAndTurn[indexer].dateProcess.Day:00}";
                case PeriodEnum.Period.DIA:
                    return $"{dateProcessAndTurn[indexer].dateProcess.Day:00}.{dateProcessAndTurn[indexer].dateProcess.Month:00}";
                case PeriodEnum.Period.SEMANA:
                    return $"Semana {dateStartAndEnd[indexer].weeks}";
                case PeriodEnum.Period.MES:
                    return $"{dateStartAndEnd[indexer].dateStart.Month:00}.{dateStartAndEnd[indexer].dateStart.Year}";
                default:
                    return "";
            }
        }
    }
}