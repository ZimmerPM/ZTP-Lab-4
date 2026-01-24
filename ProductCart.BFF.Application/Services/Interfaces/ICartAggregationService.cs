using ProductCart.BFF.Application.DTOs;

namespace ProductCart.BFF.Application.Services.Interfaces;

public interface ICartAggregationService
{
    Task<CartDto?> GetEnrichedCartAsync(string userId);
}
