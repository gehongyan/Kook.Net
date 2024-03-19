var xmlFile = new FileInfo("sample-card.xml");

// 同步调用
var cards = CardMarkupSerializer.Deserialize(xmlFile);

// 同步调用，使用 Try... 方法
var canDeserialize = CardMarkupSerializer.TryDeserialize(xmlFile, out var cards);

// 异步调用
var cards = await CardMarkupSerializer.DeserializeAsync(xmlFile);

// 异步调用，传入 CancellationToken
var cts = new CancellationTokenSource();
var cards = await CardMarkupSerializer.DeserializeAsync(xmlFile, cts.Token);
