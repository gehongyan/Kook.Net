// 本类库已对多数基本类型的类型解析进行了实现
// 本示例仅用来展示如何创建一个简单的自定义类型解析
using KaiHeiLa;
using KaiHeiLa.Commands;

public class BooleanTypeReader : TypeReader
{
    public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        bool result;
        if (bool.TryParse(input, out result))
            return Task.FromResult(TypeReaderResult.FromSuccess(result));

        return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as a boolean."));
    }
}