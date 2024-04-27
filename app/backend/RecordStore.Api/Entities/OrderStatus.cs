using NpgsqlTypes;

namespace RecordStore.Api.Entities;

public enum OrderStatus
{
    [PgName("Pending")]
    Pending,
    [PgName("Paid")]
    Paid,
    [PgName("Processing")]
    Processing,
    [PgName("Shipped")]
    Shipped,
    [PgName("Delivered")]
    Delivered,
    [PgName("Canceled")]
    Canceled
}