using System.Collections.Immutable;

namespace Kook.Net.Queue;

internal enum GatewayBucketType
{
    Unbucketed = 0
}

internal struct GatewayBucket
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

        ImmutableDictionary<GatewayBucketType, GatewayBucket>.Builder builder = ImmutableDictionary.CreateBuilder<GatewayBucketType, GatewayBucket>();
        foreach (GatewayBucket bucket in buckets) builder.Add(bucket.Type, bucket);

        DefsByType = builder.ToImmutable();

        ImmutableDictionary<BucketId, GatewayBucket>.Builder builder2 = ImmutableDictionary.CreateBuilder<BucketId, GatewayBucket>();
        foreach (GatewayBucket bucket in buckets) builder2.Add(bucket.Id, bucket);

        DefsById = builder2.ToImmutable();
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
