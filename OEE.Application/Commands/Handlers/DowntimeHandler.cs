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
    public class DowntimeHandler : IRequestHandler<Downtime, List<DowntimeModel>>
    {
        private readonly ILogger<ActualOEEHandler> logger;
        private readonly IDowntimeRepository DowntimeRepository;
        private readonly ICalendarsRepository CalendarsRepository;
        public DateTime dateHour;
        public string Scheme;
        public DateProcessAndTurnModel DateProcessAndTurnEntity = new DateProcessAndTurnModel();

        public DowntimeHandler(ILogger<ActualOEEHandler> logger, IDowntimeRepository downtimeRepository, ICalendarsRepository calendarsRepository)
        {
            this.logger = logger;
            this.DowntimeRepository = downtimeRepository;
            this.CalendarsRepository = calendarsRepository;
        }

        public async Task<List<DowntimeModel>> Handle(Downtime downtime, CancellationToken cancellationToken)
        {
            logger.LogInformation($"- Handling Start Downtimes {downtime.DowntimeRequest}");

            var downtimeRequest = downtime.DowntimeRequest;

            string scheme;

            List<DowntimeModel> downtimeEntities = new List<DowntimeModel>();

            foreach (var stationRequest in downtimeRequest.StationRequest)
            {
                scheme = await CalendarsRepository.GetScheme(stationRequest.Station, stationRequest.AssemblyLine);
                if (this.Scheme != scheme)
                {
                    this.Scheme = scheme;

                    DateProcessAndTurnEntity = new DateProcessAndTurnModel();
                    DateProcessAndTurnEntity = await CalendarsRepository.GetDateProcessAndTurn(this.Scheme);

                    if (!await SetDateTimeProcess(downtimeRequest.Period, scheme, downtime.DowntimeRequest.Quantity))
                        throw new Exception("There is a problem with the calendar, the API wasn't able to retrive it!");
                }
                List<DowntimeDBModel> downtimeResults = await DowntimeRepository.GetTop3Downtime(stationRequest.Station, stationRequest.AssemblyLine, dateHour);
                
                if (downtimeResults.Count != 0)
                    downtimeEntities.Add(SetDowntimeByStation(downtimeResults, stationRequest.Station));
            }

            logger.LogInformation($"- Handling End Downtimes {downtime.DowntimeRequest}");

            return downtimeEntities;
        }

        public async Task<bool> SetDateTimeProcess(string period, string esquema, int quantity)
        {
            var Period = (PeriodEnum.Period)Enum.Parse(typeof(PeriodEnum.Period), period);
            switch (Period)
            {
                case PeriodEnum.Period.HORA:
                    return GetCalendarByHour(quantity);
                case PeriodEnum.Period.TURNO:
                    return await GetCalendarByTurn(esquema, quantity);
                case PeriodEnum.Period.DIA:
                    return await GetCalendarByDay(esquema, quantity);
                case PeriodEnum.Period.SEMANA:
                    return await GetCalendarByWeek(esquema, quantity);
                case PeriodEnum.Period.MES:
                    return await GetCalendarByMonth(esquema, quantity);
                default:
                    return false;
            }
        }

        public bool GetCalendarByHour(int quantity)
        {
            DateTime dataProcesso = DateTime.Now;
            dataProcesso = dataProcesso.AddHours(-quantity);
            dateHour = dataProcesso;
            return true;
        }

        public async Task<bool> GetCalendarByTurn(string esquema, int quantity)
        {
            List<DateStartAndEndModel> calendars = await CalendarsRepository.GetCalendarByTurnsForDowntime(esquema, quantity);
            dateHour = calendars.Min(x => x.dateStart);
            return true;
        }

        public async Task<bool> GetCalendarByDay(string esquema, int quantity)
        {
            List<DateStartAndEndModel> calendars = await CalendarsRepository.GetCalendarByDayForDowntime(esquema, DateProcessAndTurnEntity.dateProcess, quantity);
            dateHour = calendars.Min(x => x.dateStart);
            return true;
        }

        public async Task<bool> GetCalendarByWeek(string esquema, int quantity)
        {
            List<DateStartAndEndForWeeksModel> calendars = await CalendarsRepository.GetCalendarByWeekForLastOEEAndDowntime(esquema, DateProcessAndTurnEntity.dateProcess, quantity);
            dateHour = calendars.Min(x => x.dateStart);
            return true;
        }

        public async Task<bool> GetCalendarByMonth(string esquema, int quantity)
        {
            List<DateStartAndEndModel> calendars = await CalendarsRepository.GetCalendarByMonthForLastOEEAndDowntime(esquema, DateProcessAndTurnEntity.dateProcess, quantity);
            dateHour = calendars.Min(x => x.dateStart);
            return true;
        }

        public DowntimeModel SetDowntimeByStation(List<DowntimeDBModel> downtimesResults, string station)
        {
            DowntimeModel downtimes = new DowntimeModel();

            downtimes.StationName = station;
            foreach (var downtime in downtimesResults)
            {
                DowntimeStopsModel stop = new DowntimeStopsModel();
                stop.Reason = SetDescriptionOfReason(downtime);
                if (downtime.stopTime > 60)
                    stop.Time = downtime.stopTime / 60;

                downtimes.Stops.Add(stop);
            }

            return downtimes;
        }

        public string SetDescriptionOfReason(DowntimeDBModel downtime)
        {
            var reason = "";
            if (HasValue(downtime.level1))
                reason += $"{downtime.level1} ";
            if (HasValue(downtime.level1) && HasValue(downtime.level2))
                reason += " > ";
            if (HasValue(downtime.level2))
                reason += $"{downtime.level2} ";
            if ((HasValue(downtime.level1) && HasValue(downtime.justification)) || (HasValue(downtime.level2) && HasValue(downtime.justification)))
                reason += " > ";
            if (HasValue(downtime.justification))
                reason += downtime.justification;
            if(!HasValue(reason))
                reason = "Motivo não declarado";
            return reason;
        }

        public bool HasValue(string value)
        {
            return !string.IsNullOrEmpty(value);
        }
    }
}
