﻿using System.ComponentModel.DataAnnotations;

namespace TheChienHouse.Models
{
    public class MenuItemDTO
    {
        /* I would probably make multiple records for each type of request and response, i.e. Product
         * CreateRequest, ProductCreateResponse, ProductGetRequest, ProductGetResponse, etc. 
         * But for the scope of this assignment, I've just kept it as one for simplicity. 
         */
        public record MenuItemCreateRequest(
            string Name,
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
            decimal Price,
            string DishType
        );

        public record MenuItemGetRequest(
            long? Id
        );

        public record MenuItemResponse(
            string Id,
            string Name,
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
            decimal Price,
            string DishType,
            DateTime CreatedAt,
            DateTime? UpdatedAt,
            long? Count = null // Count is optional, used for aggregating dishes with the same name
        );

        public record MenuItemUpdateRequest(
            string OldName,
            string OldType,
            decimal? NewPrice = null,
            string? NewType = null,
            string? NewName = null
        );
    }
}
