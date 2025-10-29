using TheChienHouse.Models;
using static LineItemDTO;

namespace TheChienHouse.Services
{
    public interface ILineItemService
    {
        Task<LineItem> CreateLineItemAsync(LineItemCreateRequest lineItemRequest);
        Task<List<LineItem>> CreateLineItemsAsync(List<LineItemCreateRequest> lineItemRequests);
        Task<List<LineItem>> SetLineItemSaleId(List<LineItem> lineItems, Guid saleId);
    }
}
