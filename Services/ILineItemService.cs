using TheChienHouse.Models;
using static LineItemDTO;

namespace TheChienHouse.Services
{
    public interface ILineItemService
    {
        Task<LineItem> CreateLineItemAsync(Guid menuItem, int quantity, decimal discount);
        Task<List<LineItem>> CreateLineItemsAsync(Dictionary<Guid, Tuple<int, decimal>> dictItems);
        Task<List<LineItem>> SetLineItemSaleId(List<LineItem> lineItems, Guid saleId);
    }
}
