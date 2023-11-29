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
    public class ActualOEEHandler : IRequestHandler<ActualOEE, List<ActualOEEModel>>
    {
        private readonly ILogger<ActualOEEHandler> logger;
        private readonly IOEERepository OEERepository;
        private readonly ICalendarsRepository calendarsRepository;
        public DateStartAndEndModel dateStartAndEnd;
        public string Scheme;
        public DateProcessAndTurnModel DateProcessAndTurnEntity = new DateProcessAndTurnModel();

        public ActualOEEHandler(ILogger<ActualOEEHandler> logger, IOEERepository actualOEERepository, ICalendarsRepository calendarsRepository)
        {
            this.logger = logger;
            this.OEERepository = actualOEERepository;
            this.calendarsRepository = calendarsRepository;
        }

        public async Task<List<ActualOEEModel>> Handle(ActualOEE actualOEE, CancellationToken cancellationToken)
        {
            logger.LogInformation($"- Handling Start Actual OEE {actualOEE.OEERequest}");

            var ActualOEERequest = actualOEE.OEERequest;

            string scheme;

            List<ActualOEEModel> productionEntities = new List<ActualOEEModel>();

            foreach (var stationRequest in ActualOEERequest.StationRequest)
            {
                scheme = await calendarsRepository.GetScheme(stationRequest.Station, stationRequest.AssemblyLine);
                if (this.Scheme != scheme)
                {
                    this.Scheme = scheme;

                    DateProcessAndTurnEntity = await calendarsRepository.GetDateProcessAndTurn(scheme);

                    if (!await SetDateTimeProcess(stationRequest, ActualOEERequest.Period, scheme))
                        throw new Exception("There is a problem with the calendar, the API wasn't able to retrive it!");
                }
                List<OEEDBModel> actualOEEs = await GetActualOEE(stationRequest.Station, stationRequest.AssemblyLine, ActualOEERequest.Period, scheme);
                if (actualOEEs.Count > 0)
                    productionEntities.Add(SetProductionByStation(actualOEEs));
            }

            logger.LogInformation($"- Handling End Actual OEE {actualOEE.OEERequest}");

            return productionEntities;
        }

        public async Task<List<OEEDBModel>> GetActualOEE(string station, string assemblyLine, string period, string esquema)
        {
            var Period = (PeriodEnum.Period)Enum.Parse(typeof(PeriodEnum.Period), period);
            switch (Period)
            {
                case PeriodEnum.Period.HORA:
                    return (await OEERepository.GetOEEHour(station, assemblyLine, DateTime.Now));
                case PeriodEnum.Period.TURNO:
                    if(DateProcessAndTurnEntity.dateProcess != null)
                        return (await OEERepository.GetOEETurn(station, assemblyLine, DateProcessAndTurnEntity.dateProcess, DateProcessAndTurnEntity.turn));
                    break;
                case PeriodEnum.Period.DIA:
                    if(DateProcessAndTurnEntity.dateProcess != null)
                        return (await OEERepository.GetOEEDay(station, assemblyLine, DateProcessAndTurnEntity.dateProcess));
                    break;
                case PeriodEnum.Period.SEMANA:
                    if(dateStartAndEnd.dateStart != null)
                        return (await OEERepository.GetOEEDays(station, assemblyLine, dateStartAndEnd.dateStart, dateStartAndEnd.dateEnd));
                    break;
                case PeriodEnum.Period.MES:
                    if(dateStartAndEnd.dateStart != null)
                        return (await OEERepository.GetOEEDays(station, assemblyLine, dateStartAndEnd.dateStart, dateStartAndEnd.dateEnd));
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
                    return true;
                case PeriodEnum.Period.DIA:
                    return true;
                case PeriodEnum.Period.SEMANA:
                    dateStartAndEnd = await calendarsRepository.GetCalendarByWeekForActualOEE(esquema, DateProcessAndTurnEntity.dateProcess);
                    return true;
                case PeriodEnum.Period.MES:
                    dateStartAndEnd = await calendarsRepository.GetCalendarByMonthForActualOEE(esquema, DateProcessAndTurnEntity.dateProcess);
                    return true;
                default:
                    return false;
            }
        }

        public ActualOEEModel SetProductionByStation(List<OEEDBModel> actualOEEs)
        {
            ActualOEEModel production = new ActualOEEModel();

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

            double quality = (PecasProduzidas > 0) ? PECAS_OK / PecasProduzidas : 0 ;

            double disponibility = (TempoPlanejado > 0) ? TempoEfetivo / TempoPlanejado : 0 ;

            double performance = (META_PRODUCAO > 0) ? PecasProduzidas / META_PRODUCAO : 0 ;

            production.OEE = (int)((quality * disponibility * performance)*100);
            production.Quality = (int)(quality * 100);
            production.Disponibility = (int)(disponibility * 100);
            production.Performance = (int)(performance * 100);
            production.StationName = actualOEEs.FirstOrDefault().station;

            return production;
        }
    }
}