using EStore.Data;
using EStore.DTOs;
using EStore.Entities;
using EStore.Interfaces;
using EStore.Models;
using EStore.Models.Basket;
using EStore.Models.Order;
using EStore.Models.User;
using LinqToDB;
using LinqToDB.Data;

namespace EStore.Services;

public class OrderRepository : IOrderRepository
{
    private readonly AltDataContext _dataContext;
    private readonly ILogRepository _logger;
    public OrderRepository(AltDataContext dataContext, ILogRepository logger)
    {
        _dataContext = dataContext;
        _logger = logger;
    }
    public async Task<Response> CreateOrderAsync(OrderCreateDto orderDto)
    {
        using var transaction = _dataContext.BeginTransaction();
        var response = new Response();
        try
        {
            if (orderDto == null || orderDto.CartItems == null || !orderDto.CartItems.Any())
            {
                throw new ArgumentException("Order data is invalid or cart is empty");
            }

            // Save address 
            var addressEntity = new AddressEntity
            {
                City = orderDto.Address.City,
                ZipCode = orderDto.Address.ZipCode,
                StreetAddress = orderDto.Address.StreetAddress,
                FirstName = orderDto.Address.FirstName,
                LastName = orderDto.Address.LastName,
                PhoneNumber = orderDto.Address.PhoneNumber,
            };

            var addressId = await _dataContext.InsertWithInt32IdentityAsync(addressEntity);

            // Save order with address Id
            var orderEntity = new OrderEntity
            {
                AddressId = addressId,
                Created = DateTime.UtcNow,
                Total = orderDto.Total,
                Status = "pending",
                PaymentMethod = orderDto.PaymentMethod ?? "cod",
                UserId = orderDto.UserId ?? string.Empty
            };

            // Depending on your implementation, InsertAsync might return the generated id.
            await _dataContext.InsertAsync(orderEntity);

            var selectedVariants = new List<SelectedVariantEntity>();

            foreach (var ci in orderDto.CartItems)
            {
                // Check if the product exists
                var product = await _dataContext.Products.FirstOrDefaultAsync(m => m.Id == ci.ProductId);
                if (product == null )
                {
                    response.Error = "Product doesn't exist or was removed";
                    response.Success = false;
                    return response;
                }
                if(product.Stock < 1)
                {
                    response.Error = "Product is out of stock";
                    response.Success = false;
                    return response;
                }

                // Save cart item with OrderId
                var cartItemId = await _dataContext.InsertWithInt32IdentityAsync(new CartItemEntity
                {
                    OrderEntityId = orderEntity.Id,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    SubTotal = ci.Price * ci.Quantity,
                });
                var validStock = product.Stock - ci.Quantity > -1;
                if (!validStock)
                {
                    response.Error = "Not enough stock available, please reduce quantity and try again";
                    response.Success = false;
                    return response;
                }
                product.Stock = product.Stock - ci.Quantity;
                _dataContext.Update(product);

                // Process selected variants if any (ci.SelectedVariants is assumed to be a Dictionary<int, int>
                if (ci.SelectedVariants != null)
                {
                    foreach (var variant in ci.SelectedVariants)
                    {
                        // variant.Key is the VariantEntityId, variant.Value is the OptionEntityId
                        selectedVariants.Add(new SelectedVariantEntity
                        {
                            CartItemEntityId = cartItemId,
                            VariantEntityId = variant.Key,
                            OptionEntityId = variant.Value,
                        });
                    }
                }
            }

            if (selectedVariants.Any())
            {
                await _dataContext.BulkCopyAsync(selectedVariants);
            }
            
            await transaction.CommitAsync();

            response.Success = true;
            response.Data = orderEntity;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await _logger.LogAsync(ex.Message);
            response.Success = false;
            response.Error = $"Failed to create order: {ex.Message}";
        }

        return response;
    }

    public async Task<(List<UserOrder>, int)> GetAllAsync(Status status = Status.All, int page = 1, int size = 5, string userId = "")
    {
        try
        {
            // Retrieve orders with an optional status filter.
            var orderQuery = _dataContext.Orders.AsQueryable();
            if (status != Status.All)
            {
                orderQuery = orderQuery.Where(o => o.Status == status.ToString().ToLower());
            }
            if (!string.IsNullOrEmpty(userId))
            {
                orderQuery = orderQuery.Where(o => o.UserId == userId);
            }
            orderQuery = orderQuery.OrderByDescending(o => o.Created);
            var totalCount = await orderQuery.CountAsync();
            var orderEntities = await orderQuery
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            // Collect IDs needed for batch lookups.
            var addressIds = orderEntities.Select(o => o.AddressId).Distinct().ToList();
            var orderIds = orderEntities.Select(o => o.Id).Distinct().ToList();
            var userIds = orderEntities.Where(o => !string.IsNullOrEmpty(o.UserId))
                                       .Select(o => o.UserId)
                                       .Distinct()
                                       .ToList();

            // Batch fetch addresses, users, and cart items.
            var addresses = await _dataContext.Addresses
                .Where(a => addressIds.Contains(a.Id))
                .ToListAsync();

            var users = await _dataContext.Users
                .Where(u => userIds.Contains(u.Id.ToString()))
                .ToListAsync();

            var cartItemEntities = await _dataContext.CartItems
                .Where(ci => orderIds.Contains(ci.OrderEntityId))
                .ToListAsync();

            // Map cart items with an optimized helper (see below).
            var cartItemsGrouped = await MapCartItems(cartItemEntities);

            var userOrders = new List<UserOrder>();
            foreach (var orderEntity in orderEntities)
            {
                var userOrder = new UserOrder
                {
                    Id = orderEntity.Id.ToString(),
                    Total = orderEntity.Total,
                    Created = orderEntity.Created,
                    Status = Enum.Parse<Status>(orderEntity.Status, ignoreCase: true),
                    Address = MapAddress(addresses.FirstOrDefault(a => a.Id == orderEntity.AddressId)),
                    User = users.FirstOrDefault(u => u.Id.ToString() == orderEntity.UserId) is UserEntity ue ? MapUser(ue) : null,
                    CartItems = cartItemsGrouped.ContainsKey(orderEntity.Id)
                                ? cartItemsGrouped[orderEntity.Id]
                                : new List<CartItem>()
                };

                userOrders.Add(userOrder);
            }

            return (userOrders, totalCount);
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Error fetching orders: {ex.Message}");
            throw;
        }
    }


    public async Task<List<UserOrder>> GetOrderByParamsAsync(string param)
    {
        if (string.IsNullOrEmpty(param))
        {
            return new List<UserOrder>();
        }

        try
        {
            var orderEntity = new List<OrderEntity>();
            if (IsPhone(param))
            {
                var addressIds = await _dataContext.Addresses.Where(m=>m.PhoneNumber == param).Select(m=>m.Id).ToListAsync();
                orderEntity = await _dataContext.Orders.Where(m => addressIds.Contains(m.AddressId)).ToListAsync();
            }
            else
            {
                orderEntity = await _dataContext.Orders.Where(m => m.Id == param).ToListAsync();
            }
            
            if (orderEntity == null)
            {
                throw new Exception($"Order with ID/Phone {param} not found");
            }

            var userOrders = new List<UserOrder>();

            foreach (var order in orderEntity)
            {
            var addressEntity = await _dataContext.Addresses.FirstOrDefaultAsync(m => m.Id == order.AddressId);
            var cartItemEntities = await _dataContext.CartItems.Where(x => x.OrderEntityId == order.Id).ToListAsync();
            var userEntity = !string.IsNullOrEmpty(order.UserId)
                ? await _dataContext.Users.FirstOrDefaultAsync(m => m.Id.ToString() == order.UserId)
                : null;

            var cartItemsGrouped = await MapCartItems(cartItemEntities);

            var userOrder = new UserOrder
            {
                Id = order.Id.ToString(),
                Total = order.Total,
                Created = order.Created,
                Status = Enum.Parse<Status>(order.Status, ignoreCase: true),
                Address = MapAddress(addressEntity),
                User = userEntity != null ? MapUser(userEntity) : null,
                CartItems = cartItemsGrouped.ContainsKey(order.Id)
                            ? cartItemsGrouped[order.Id]
                            : new List<CartItem>()
            };

                userOrders.Add(userOrder);
            }


            return userOrders;
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Error fetching order: {ex.Message}");
            return new List<UserOrder>();
        }
     
    }
     bool IsPhone(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false; // Or true, depending on your requirement for empty strings
        }
        return input.All(char.IsDigit);
    }
    private Address MapAddress(AddressEntity addressEntity)
    {
        return new Address
        {
            Id = addressEntity.Id,
            FirstName = addressEntity.FirstName,
            LastName = addressEntity.LastName,
            City = addressEntity.City,
            ZipCode = addressEntity.ZipCode,
            StreetAddress = addressEntity.StreetAddress,
            PhoneNumber = addressEntity.PhoneNumber
        };
    }

    private AppUser MapUser(UserEntity userEntity)
    {
        return new AppUser
        {
            Id = userEntity.Id.ToString(),
            UserName = userEntity.UserName,
            // Map other user properties as needed
        };
    }

    private async Task<Dictionary<string, List<CartItem>>> MapCartItems(IEnumerable<CartItemEntity> cartItemEntities)
    {
        var cartItemList = cartItemEntities.ToList();
        if (!cartItemList.Any())
            return new Dictionary<string, List<CartItem>>();

        // Get distinct product IDs and cart item IDs
        var productIds = cartItemList.Select(ci => ci.ProductId).Distinct().ToList();
        // Get the distinct cart item IDs from the cart items we already fetched
        var cartItemIds = cartItemList.Select(ci => ci.Id).Distinct().ToList();

        var variantData = await (
            from sv in _dataContext.SelectedVariants
            where cartItemIds.Contains(sv.CartItemEntityId)
            join v in _dataContext.Variants
                 on sv.VariantEntityId equals v.Id into vGroup
            from v in vGroup.DefaultIfEmpty() // If this is null, then the join failed
            join vo in _dataContext.VariantOptions
                 on sv.OptionEntityId equals vo.Id into voGroup
            from vo in voGroup.DefaultIfEmpty()
            select new
            {
                CartItemEntityId = sv.CartItemEntityId,
                Id = sv.Id,
                VariantName = v != null ? v.Name : "[No Name]",
                OptionValue = vo != null ? vo.Value : string.Empty,
                PriceAdjustment = vo != null ? vo.PriceAdjustment : 0m,
                
            }
        ).ToListAsync();
        

        // Batch fetch products
        var products = await _dataContext.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();


        var variantGroups = variantData
     .GroupBy(x => x.CartItemEntityId)
     .ToDictionary(
          g => g.Key,
          g => g.Select(x => new SelectedVariant
          {
              Id = x.Id,
              VariantName = x.VariantName,
              OptionValue = x.OptionValue,
              PriceAdjustment = x.PriceAdjustment
          }).ToList()
     );


        var cartItems = new List<CartItem>();
        foreach (var ci in cartItemList)
        {
            var product = products.FirstOrDefault(p => p.Id == ci.ProductId);
            var cartItem = new CartItem
            {
                Id = ci.Id,
                Product = MapProduct(product),  // Assumes you have a MapProduct method
                Quantity = ci.Quantity,
                SubTotal = ci.SubTotal,
                SelectedVariants = variantGroups.ContainsKey(ci.Id) ? variantGroups[ci.Id] : new List<SelectedVariant>(),
                
            };
            cartItems.Add(cartItem);
        }


        // Group cart items by OrderEntityId to ease mapping in GetAllAsync/GetOrderByIdAsync.
        return cartItems.GroupBy(ci => cartItemEntities.First(c => c.Id == ci.Id).OrderEntityId)
                        .ToDictionary(g => g.Key, g => g.ToList());
    }

    private Product MapProduct(ProductEntity productEntity)
    {
        if (productEntity != null)
        {
        return new Product
        {
            Id = productEntity.Id,
            Name = productEntity.Name,
            Price = productEntity.Price,
            SKU = productEntity.SKU
            // Map other product properties as needed
        };

        }
        return new Product
        {
            Id = 0,
            Name = "Product does not exist, It was most likely deleted",

        };
    }
    public async Task<bool> UpdateOrderStatus(string orderId, Status status)
    {
        try
        {
            var orderEntity = await _dataContext.Orders.FirstOrDefaultAsync(o => o.Id.ToString() == orderId);

            if (orderEntity == null)
            {
                throw new Exception($"Order with ID {orderId} not found");
            }

            orderEntity.Status = status.ToString().ToLower();
            await _dataContext.UpdateAsync(orderEntity);

            return true;
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Error updating order status: {ex.Message}");
            return false;
        }
    }

    public async Task<List<UserOrder>> GetUserOrders(string userId)
    {
        try
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(m => m.Id.ToString() == userId);
            if (user == null)
            {
                await _logger.LogAsync($"User with ID {userId} not found");
                return new List<UserOrder>();
            }

            var (userOrders,totalCount) = await GetAllAsync(userId:userId);

            return userOrders;
        }
        catch (Exception ex)
        {
            await _logger.LogAsync( $"Error fetching orders for user {userId} :{ex.Message}");
            throw;
        }
    }


}
