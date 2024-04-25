using NpgsqlTypes;

namespace RecordStore.Api.Entities;

public enum EventType
{
    [PgName("Sale")]
    Sale,
    [PgName("Purchase")]
    Purchase,
    [PgName("Cancel")]
    Cancel
}