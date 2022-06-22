using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orders.Api.Controllers.ViewModel;
using Orders.Domain.Commands;
using System.Net;

namespace Orders.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(CreateOrderApiRequest request)
        {
            CreateOrderCommand command = new CreateOrderCommand
            {
                Id = request.Id,
                Items = request.Items
            };

            var result = await _mediator.Send(command);

            if (!result)
            {
                return BadRequest();
            }

            return this.StatusCode(201);

            //note: if exception was throwing the exception middleware returns the correct Status Code (see ExceptionMiddleware class)
        }
    }
}

