IChannel channel;

// 如果要将通用频道接口 IChannel 转换为服务器文字频道接口 ITextChannel
// 来访问 ITextChannel 中存在而 IChannel 中不存在的属性和方法
// 则可进行如下的转换
ITextChannel textChannel = channel as ITextChannel;

await textChannel.DoSomethingICantWithIChannelAsync();