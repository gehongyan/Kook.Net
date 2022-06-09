IUser user;

// 假设这里的 user 变量内事实上存储的是一个 IGuildUser 对象
// 那么在类型检查通过后，此处的代码可以直接将 user 转换为 IGuildUser
// 并赋值到 guildUser 变量中
// 这样就不用再在后面的代码中再次进行类型转换了
if (user is IGuildUser guildUser)
{
    Console.WriteLine(guildUser.JoinedAt);
}
else
{
    // 检查不通过
}