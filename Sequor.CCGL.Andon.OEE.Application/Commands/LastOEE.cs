using MediatR;
using OEE.Domain.Models;
using System.Collections.Generic;

namespace OEE.Application.Commands
{
    public class LastOEE : IRequest<List<LastOEEModel>>
    {
        public OEERequestModel LastOEERequest { get; set; }

        public LastOEE(OEERequestModel actualOEEsRequest)
        {
            LastOEERequest = actualOEEsRequest;
        }
    }
}
