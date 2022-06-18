
namespace Orders.Api.Controllers.ViewModel
{
    public class CreateOrderApiRequest
    {
        public Guid Id { get; set; }

        public IDictionary<string, decimal> Items { get; set; }
    }
}
