using System.Collections.Immutable;

namespace Kook.Net.Queue;

internal enum ClientBucketType
{
    Unbucketed = 0,
    SendEdit = 1
}

internal struct ClientBucket
{
    private static readonly ImmutableDictionary<ClientBucketType, ClientBucket> DefsByType;
    private static readonly ImmutableDictionary<BucketId, ClientBucket> DefsById;

    static ClientBucket()
    {
        ClientBucket[] buckets =
        [
            new ClientBucket(ClientBucketType.Unbucketed, BucketId.Create(null, "<unbucketed>", null), 10, 10),
            new ClientBucket(ClientBucketType.SendEdit, BucketId.Create(null, "<send_edit>", null), 10, 10)
        ];

        ImmutableDictionary<ClientBucketType, ClientBucket>.Builder typeBuilder =
            ImmutableDictionary.CreateBuilder<ClientBucketType, ClientBucket>();
        foreach (ClientBucket bucket in buckets)
            typeBuilder.Add(bucket.Type, bucket);
        DefsByType = typeBuilder.ToImmutable();

        ImmutableDictionary<BucketId, ClientBucket>.Builder idBuilder =
            ImmutableDictionary.CreateBuilder<BucketId, ClientBucket>();
        foreach (ClientBucket bucket in buckets) idBuilder.Add(bucket.Id, bucket);
        DefsById = idBuilder.ToImmutable();
    }

    public static ClientBucket Get(ClientBucketType type) => DefsByType[type];
    public static ClientBucket Get(BucketId id) => DefsById[id];

    public ClientBucketType Type { get; }
    public BucketId Id { get; }
    public int WindowCount { get; }
    public int WindowSeconds { get; }

    public ClientBucket(ClientBucketType type, BucketId id, int count, int seconds)
    {
        Type = type;
        Id = id;
        WindowCount = count;
        WindowSeconds = seconds;
    }
}
