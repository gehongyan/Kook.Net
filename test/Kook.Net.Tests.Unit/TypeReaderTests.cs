using Kook.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Kook;

public sealed class TypeReaderTests
{
    [Fact]
    public async Task TestNamedArgumentReader()
    {
        using CommandService commands = new();
        ModuleInfo module = await commands.AddModuleAsync<TestModule>(null);

        Assert.NotNull(module);
        Assert.NotEmpty(module.Commands);

        CommandInfo cmd = module.Commands[0];
        Assert.NotNull(cmd);
        Assert.NotEmpty(cmd.Parameters);

        ParameterInfo param = cmd.Parameters[0];
        Assert.NotNull(param);
        Assert.True(param.IsRemainder);

        TypeReaderResult result = await param.ParseAsync(null, "bar: hello foo: 42");
        Assert.True(result.IsSuccess);

        ArgumentType m = result.BestMatch as ArgumentType;
        Assert.NotNull(m);
        Assert.Equal(42, m.Foo);
        Assert.Equal("hello", m.Bar);
    }

    [Fact]
    public async Task TestQuotedArgumentValue()
    {
        using CommandService commands = new();
        ModuleInfo module = await commands.AddModuleAsync<TestModule>(null);

        Assert.NotNull(module);
        Assert.NotEmpty(module.Commands);

        CommandInfo cmd = module.Commands[0];
        Assert.NotNull(cmd);
        Assert.NotEmpty(cmd.Parameters);

        ParameterInfo param = cmd.Parameters[0];
        Assert.NotNull(param);
        Assert.True(param.IsRemainder);

        TypeReaderResult result = await param.ParseAsync(null, "foo: 42 bar: 《hello》");
        Assert.True(result.IsSuccess);

        ArgumentType m = result.BestMatch as ArgumentType;
        Assert.NotNull(m);
        Assert.Equal(42, m.Foo);
        Assert.Equal("hello", m.Bar);
    }

    [Fact]
    public async Task TestNonPatternInput()
    {
        using CommandService commands = new();
        ModuleInfo module = await commands.AddModuleAsync<TestModule>(null);

        Assert.NotNull(module);
        Assert.NotEmpty(module.Commands);

        CommandInfo cmd = module.Commands[0];
        Assert.NotNull(cmd);
        Assert.NotEmpty(cmd.Parameters);

        ParameterInfo param = cmd.Parameters[0];
        Assert.NotNull(param);
        Assert.True(param.IsRemainder);

        TypeReaderResult result = await param.ParseAsync(null, "foobar");
        Assert.False(result.IsSuccess);
        Assert.Equal(CommandError.Exception, result.Error);
    }

    [Fact]
    public async Task TestMultiple()
    {
        using CommandService commands = new();
        ModuleInfo module = await commands.AddModuleAsync<TestModule>(null);

        Assert.NotNull(module);
        Assert.NotEmpty(module.Commands);

        CommandInfo cmd = module.Commands[0];
        Assert.NotNull(cmd);
        Assert.NotEmpty(cmd.Parameters);

        ParameterInfo param = cmd.Parameters[0];
        Assert.NotNull(param);
        Assert.True(param.IsRemainder);

        TypeReaderResult result = await param.ParseAsync(null, "manyints: \"1, 2, 3, 4, 5, 6, 7\"");
        Assert.True(result.IsSuccess);

        ArgumentType m = result.BestMatch as ArgumentType;
        Assert.NotNull(m);
        Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7 }, m.ManyInts);
    }
}

[NamedArgumentType]
public sealed class ArgumentType
{
    public int Foo { get; set; }

    [OverrideTypeReader(typeof(CustomTypeReader))]
    public string Bar { get; set; }

    public IEnumerable<int> ManyInts { get; set; }
}

public sealed class CustomTypeReader : TypeReader
{
    public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        => Task.FromResult(TypeReaderResult.FromSuccess(input));
}

public sealed class TestModule : ModuleBase
{
    [Command("test")]
    public Task TestCommand(ArgumentType arg) => Task.Delay(0);
}
