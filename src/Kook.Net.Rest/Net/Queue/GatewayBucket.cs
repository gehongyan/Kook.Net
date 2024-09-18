using System.Collections.Immutable;

namespace Kook.Net.Queue;

internal enum GatewayBucketType
{
    Unbucketed = 0
}

internal record struct GatewayBucket
{
    private static readonly ImmutableDictionary<GatewayBucketType, GatewayBucket> DefsByType;
    private static readonly ImmutableDictionary<BucketId, GatewayBucket> DefsById;

    static GatewayBucket()
    {
        GatewayBucket[] buckets =
        [
            // Limit is 120/60s, but 3 will be reserved for heartbeats (2 for possible heartbeats in the same timeframe and a possible failure)
            new GatewayBucket(GatewayBucketType.Unbucketed, BucketId.Create(null, "<gateway-unbucketed>", null), 117, 60)
        ];

        ImmutableDictionary<GatewayBucketType, GatewayBucket>.Builder typeBuilder =
            ImmutableDictionary.CreateBuilder<GatewayBucketType, GatewayBucket>();
        foreach (GatewayBucket bucket in buckets)
            typeBuilder.Add(bucket.Type, bucket);
        DefsByType = typeBuilder.ToImmutable();

        ImmutableDictionary<BucketId, GatewayBucket>.Builder idBuilder =
            ImmutableDictionary.CreateBuilder<BucketId, GatewayBucket>();
        foreach (GatewayBucket bucket in buckets)
            idBuilder.Add(bucket.Id, bucket);
        DefsById = idBuilder.ToImmutable();
    }

    public static GatewayBucket Get(GatewayBucketType type) => DefsByType[type];
    public static GatewayBucket Get(BucketId id) => DefsById[id];

    public GatewayBucketType Type { get; }
    public BucketId Id { get; }
    public int WindowCount { get; set; }
    public int WindowSeconds { get; set; }

    public GatewayBucket(GatewayBucketType type, BucketId id, int count, int seconds)
    {
        Type = type;
        Id = id;
        WindowCount = count;
        WindowSeconds = seconds;
    }
}
