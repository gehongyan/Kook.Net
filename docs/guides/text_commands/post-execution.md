---
uid: Guides.TextCommands.PostExecution
title: 后处理程序
---

# 文本命令执行的后处理程序

在开发命令时，您可能想要在执行命令后执行一些操作，KaiHeiLa.Net
提供了一些命令执行后处理工作流来实现这个目的。

[文本命令框架入门] 中展示了下面的示例来执行并处理命令及其内的数据：

[!code[Command Handler](samples/intro/command_handler.cs)]

示例中，[ExecuteAsync] 方法的运行结果会返回一个 `Task<IResult>`
对象，要将结果打印到聊天中，可以采用以下的代码示例：

[!code[Basic Command Handler](samples/post-execution/post-execution-basic.cs)]

然而，这种处理方式中，命令执行后处理程序被嵌入到了基本的命令执行逻辑中，
这样会导致命令执行逻辑部分的代码更混乱，且可能违反了单一职责原则（SRP），不受推荐。

另一个主要的问题是，如果命令的处理程序被 `RunMode.Async` 标记为异步模式运行，[ExecuteAsync]
将**总是**返回包含执行成功信息的 [ExecuteResult]，而非真正的执行结果。

## CommandExecuted 事件

当命令执行完成时，无论其结果如何，都会触发 [CommandExecuted] 事件。
该事件可以简化前面示例中混乱的结构，且可以避免 `RunMode.Async` 模式的问题。

采用事件模式的代码示例如下：

[!code[CommandExecuted demo](samples/post-execution/command-executed-demo.cs)]

So now we have a streamlined post-execution pipeline, great! What's
next? We can take this further by using [RuntimeResult].

### RuntimeResult

命令执行完成后可以返回 `RuntimeResult` 对象，`RuntimeResult`
是一个结果类，可以用来表示命令执行结果的逻辑。

由于 [ExecuteAsync] 中异步执行的命令始终无法返回错误信息，[RuntimeResult]
往往不应与 [ExecuteResult] 共同使用，而应在 [CommandExecuted] 事件处理程序中使用。
在实践中，您可以基于 [RuntimeResult] 派生自定义的结果类，以记录更多的结果信息及逻辑。

下面的示例创建了一个 `RuntimeResult` 的最小派生类：

[!code[Base Use](samples/post-execution/customresult-base.cs)]

派生类中的信息及逻辑没有什么特殊的限制，您可以添加有关执行结果的任何其他信息。

例如，可能根据命令的执行结果添加更多的方法、字段或属性，
或是一些静态方法来更容易地创建该结果类。

[!code[Extended Use](samples/post-execution/customresult-extended.cs)]

要想在命令执行后返回该结果类，请将方法前面的返回类型更改为 `Task<RuntimeResult>`。

> [!NOTE]
> 方法签名中的返回类型必须是 `Task<RuntimeResult>` 而不应是
> `Task<MyCustomResult>`，后者无法被自动模块加载发现并加载。

下面的示例展示了如何使用该自定义结果类：

[!code[Usage](samples/post-execution/customresult-usage.cs)]

进而可以在 [CommandExecuted] 的事件处理程序中使用该结果类：

[!code[Usage](samples/post-execution/command-executed-adv-demo.cs)]

## CommandService.Log 事件

[CommandService.Log] 事件可以记录命令执行过程中发生的异常信息,
而这些信息由于业务逻辑抛出异常而中断执行，无法被传递到正常的命令执行后处理程序中。

所有命令执行过程中发生的异常信息都会被捕获并传入 [CommandException] 类内的
[LogMessage.Exception] 属性中，[CommandException] 类记录了抛出的异常及命令执行的上下文。

[!code[Logger Sample](samples/post-execution/command-exception-log.cs)]

[CommandException]: xref:KaiHeiLa.Commands.CommandException
[LogMessage.Exception]: xref:KaiHeiLa.LogMessage.Exception
[CommandService.Log]: xref:Discord.KaiHeiLa.CommandService.Log
[RuntimeResult]: xref:KaiHeiLa.Commands.RuntimeResult
[CommandExecuted]: xref:KaiHeiLa.Commands.CommandService.CommandExecuted
[ExecuteAsync]: xref:KaiHeiLa.Commands.CommandService.ExecuteAsync*
[ExecuteResult]: xref:KaiHeiLa.Commands.ExecuteResult
[文本命令框架入门]: xref:Guides.TextCommands.Intro