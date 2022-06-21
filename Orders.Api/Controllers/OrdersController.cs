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
        private IMediator _mediator { get; }

        public OrdersController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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

            try
            {
                var result = await _mediator.Send<bool>(command);

                if (!result)
                {
                    return BadRequest();
                }

                return this.StatusCode(201);
            }
            catch (Exception)
            {
                return this.UnprocessableEntity();
            }
        }
    }
}

