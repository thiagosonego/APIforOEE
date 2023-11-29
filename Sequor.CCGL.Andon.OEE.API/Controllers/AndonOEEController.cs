using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OEE.Application.Commands;
using OEE.Application.Validators;
using OEE.Domain.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace OEE.API.Controllers
{
    [ApiController]
    [Route("api/andon-oee/")]
    public class AndonOEEController : ControllerBase
    {
        private readonly IMediator mediator;

        public AndonOEEController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("ActualOEE")]
        public async Task<IActionResult> GetActualOEE([FromQuery]string json)
        {
            OEERequestModel model = JsonSerializer.Deserialize<OEERequestModel>(json);
            var actualOEEsRequestValidator = new OEERequestValidator();
            ValidationResult validationResult = actualOEEsRequestValidator.Validate(model);
            if (validationResult.IsValid)
            {
                List<ActualOEEModel> productionEntities = await mediator.Send(new ActualOEE(model));
                return base.Ok(
                        productionEntities
                    );
            }
            return base.BadRequest(
                new ActualOEEModel()
                {
                    StationName = $"NOK"
                }
            );
        }

        [HttpGet("Downtime")]
        public async Task<IActionResult> GetDowntime([FromQuery] string json)
        {
            DowntimeRequestModel model = JsonSerializer.Deserialize<DowntimeRequestModel>(json);
            var actualOEEsRequestValidator = new DowntimeRequestValidator();
            ValidationResult validationResult = actualOEEsRequestValidator.Validate(model);
            if (validationResult.IsValid)
            {
                List<DowntimeModel> productionEntities = await mediator.Send(new Downtime(model));
                return base.Ok(
                        productionEntities
                    );
            }
            return base.BadRequest(
                new DowntimeModel()
                {
                    StationName = $"NOK"
                }
            );
        }

        [HttpGet("LastOEE")]
        public async Task<IActionResult> GetLastOEE([FromQuery] string json)
        {
            OEERequestModel model = JsonSerializer.Deserialize<OEERequestModel>(json);
            var actualOEEsRequestValidator = new OEERequestValidator();
            ValidationResult validationResult = actualOEEsRequestValidator.Validate(model);
            if (validationResult.IsValid)
            {
                List<LastOEEModel> productionEntities = await mediator.Send(new LastOEE(model));
                return base.Ok(
                        productionEntities
                    );
            }
            return base.BadRequest(
                new LastOEEModel()
                {
                    StationName = $"NOK"
                }
            );
        }
    }
}