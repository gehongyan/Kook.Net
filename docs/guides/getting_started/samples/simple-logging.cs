// Log 事件，此处以直接输出到控制台为例
Task LogAsync(LogMessage log)
{
    Console.WriteLine(log.ToString());
    return Task.CompletedTask;
}
