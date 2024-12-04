using EStore.Models.Order;
using System.Text.Json.Serialization;

namespace EStore.DTOs;

public class UpdateOrderStatusDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Status Status { get; set; }
}
