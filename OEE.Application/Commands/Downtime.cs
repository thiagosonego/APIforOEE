using MediatR;
using OEE.Domain.Models;
using System.Collections.Generic;

namespace OEE.Application.Commands
{
    public class Downtime : IRequest<List<DowntimeModel>>
    {
        public DowntimeRequestModel DowntimeRequest { get; set; }

        public Downtime(DowntimeRequestModel downtimeRequest)
        {
            DowntimeRequest = downtimeRequest;
        }
    }
}
