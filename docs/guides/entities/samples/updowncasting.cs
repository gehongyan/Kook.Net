IUser user;

// 这里使用了内联的向下类型转换来一次性获取字段数据
// 需要注意的是，如果类型转换的结果实体为 null，则会抛出 NullReferenceException 异常
Console.WriteLine(((IGuildUser)user).Nickname);

// 如果可以保证转换是合法且结果非空的，则可以为类型转换结果赋值到另一个变量中
IGuildUser guildUser = (IGuildUser)user;