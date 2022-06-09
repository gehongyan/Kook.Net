private void MyFunction(IMessage message)
{
    // 这里的类型检查与逻辑模式中的 not 相结合
    // 当 message 不为 IUserMessage 时，方法会直接返回
    if (message is not IUserMessage userMessage)
        return;

    // 由于以上代码进行的类型检查是内联的
    // 类型转换结果 userMessage 变量在判断语句外也可以访问
    Console.WriteLine(userMessage.Author);
}