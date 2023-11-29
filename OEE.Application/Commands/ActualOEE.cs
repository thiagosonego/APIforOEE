using MediatR;
using OEE.Domain.Models;
using System.Collections.Generic;

namespace OEE.Application.Commands
{
    public class ActualOEE : IRequest<List<ActualOEEModel>>
    {
        public OEERequestModel OEERequest { get; set; }

        public ActualOEE(OEERequestModel actualOEEsRequest)
        {
            OEERequest = actualOEEsRequest;
        }
    }
}
