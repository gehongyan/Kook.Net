IUser user;

// 这里检查了该用户实体是否为一个 IGuildUser 实体
// 如果检查不通过，条件判断语句可以绕过可能会导致 null 对象访问的代码
if (user is IGuildUser)
{
    Console.WriteLine("This user is in a guild!");
}
else
{
    // 检查不通过
}