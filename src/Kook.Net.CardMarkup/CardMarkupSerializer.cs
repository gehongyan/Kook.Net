using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using Kook.CardMarkup.Extensions;
using Kook.CardMarkup.Models;

namespace Kook.CardMarkup;

/// <summary>
///     提供用于从 XML 标记语言创建卡片消息的序列化器。
/// </summary>
public static class CardMarkupSerializer
{
    #region Async

    /// <summary>
    ///     将卡片消息 XML 标记文件反序列化为 <see cref="ICard"/> 列表，每个 XML 文件包含一个 <c>card-message</c>
    ///     元素，可以包含多个 <c>card</c> 元素。
    /// </summary>
    /// <param name="file"> UTF-8 编码的 XML 文件 </param>
    /// <param name="token"> 取消令牌。 </param>
    /// <returns> 一个表示异步解析操作的任务。任务的结果包含一个可用于枚举 <see cref="ICard"/> 成员的 <see cref="System.Collections.Generic.IEnumerable{T}"/>。 </returns>
    public static async Task<IEnumerable<ICard>> DeserializeAsync(FileInfo file, CancellationToken token = default)
    {
#if NETSTANDARD2_0 || NET462
        using FileStream fs = file.OpenRead();
#else
        await using FileStream fs = file.OpenRead();
#endif
        return await DeserializeAsync(fs, token);
    }

    /// <summary>
    ///     将卡片消息 XML 标记文本反序列化为 <see cref="ICard"/> 列表，每个 XML 文件包含一个 <c>card-message</c>
    ///     元素，可以包含多个 <c>card</c> 元素。
    /// </summary>
    /// <param name="xmlText"> UTF-8 编码的 XML 文本。 </param>
    /// <param name="token"> 取消令牌。 </param>
    /// <returns> 一个表示异步解析操作的任务。任务的结果包含一个可用于枚举 <see cref="ICard"/> 成员的 <see cref="System.Collections.Generic.IEnumerable{T}"/>。 </returns>
    public static async Task<IEnumerable<ICard>> DeserializeAsync(string xmlText, CancellationToken token = default)
    {
        using MemoryStream xmlStream = new(Encoding.UTF8.GetBytes(xmlText));
        return await DeserializeAsync(xmlStream, token);
    }

    /// <summary>
    ///     将卡片消息 XML 标记文本流反序列化为 <see cref="ICard"/> 列表，每个 XML 文件包含一个 <c>card-message</c>
    ///     元素，可以包含多个 <c>card</c> 元素。
    /// </summary>
    /// <param name="xmlStream"> UTF-8 编码的 XML 流。 </param>
    /// <param name="token"> 取消令牌。 </param>
    /// <returns> 一个表示异步解析操作的任务。任务的结果包含一个可用于枚举 <see cref="ICard"/> 成员的 <see cref="System.Collections.Generic.IEnumerable{T}"/>。 </returns>
    public static async Task<IEnumerable<ICard>> DeserializeAsync(Stream xmlStream, CancellationToken token = default)
    {
        using XmlReader xmlReader = XmlReader.Create(xmlStream, new XmlReaderSettings
        {
            Async = true,
            IgnoreWhitespace = true,
            IgnoreComments = true
        });

        MarkupElement? markupElement = null;
        Stack<MarkupElement> stack = [];

        while (await xmlReader.ReadAsync())
        {
            XmlNodeType nodeType = xmlReader.NodeType;

            if (token.IsCancellationRequested)
                throw new TaskCanceledException("The operation was canceled.");

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (nodeType)
            {
                case XmlNodeType.Element:
                    Dictionary<string, string> attributes = new();
                    if (xmlReader.HasAttributes)
                    {
                        for (int i = 0; i < xmlReader.AttributeCount; i++)
                        {
                            xmlReader.MoveToAttribute(i);
                            attributes.Add(xmlReader.Name, xmlReader.Value);
                        }

                        xmlReader.MoveToElement();
                    }

                    MarkupElement e = new()
                    {
                        Name = xmlReader.Name,
                        Attributes = attributes,
                        Children = []
                    };

                    if (xmlReader.IsEmptyElement)
                        stack.Peek().Children.Add(e);
                    else
                        stack.Push(e);
                    break;
                case XmlNodeType.EndElement:
                    MarkupElement element = stack.Pop();
                    if (stack.Count == 0)
                        markupElement = element;
                    else
                        stack.Peek().Children.Add(element);
                    break;
                case XmlNodeType.Text:
                    stack.Peek().Text = xmlReader.Value;
                    break;
            }
        }

        return markupElement?.ToCards() ?? [];
    }

    #endregion

    #region Sync

    /// <summary>
    ///     将卡片消息 XML 标记文件反序列化为 <see cref="ICard"/> 列表，每个 XML 文件包含一个 <c>card-message</c>
    ///     元素，可以包含多个 <c>card</c> 元素。
    /// </summary>
    /// <param name="file"> UTF-8 编码的 XML 文件 </param>
    /// <returns> 一个可用于枚举 <see cref="ICard"/> 成员的 <see cref="System.Collections.Generic.IEnumerable{T}"/>。 </returns>
    public static IEnumerable<ICard> Deserialize(FileInfo file)
    {
        using FileStream fs = file.OpenRead();
        return Deserialize(fs);
    }

    /// <summary>
    ///     将卡片消息 XML 标记文本反序列化为 <see cref="ICard"/> 列表，每个 XML 文件包含一个 <c>card-message</c>
    ///     元素，可以包含多个 <c>card</c> 元素。
    /// </summary>
    /// <param name="xmlText"> UTF-8 编码的 XML 文本。 </param>
    /// <returns> 一个可用于枚举 <see cref="ICard"/> 成员的 <see cref="System.Collections.Generic.IEnumerable{T}"/>。 </returns>
    public static IEnumerable<ICard> Deserialize(string xmlText)
    {
        using MemoryStream xmlStream = new(Encoding.UTF8.GetBytes(xmlText));
        return Deserialize(xmlStream);
    }

    /// <summary>
    ///     将卡片消息 XML 标记文本流反序列化为 <see cref="ICard"/> 列表，每个 XML 文件包含一个 <c>card-message</c>
    ///     元素，可以包含多个 <c>card</c> 元素。
    /// </summary>
    /// <param name="xmlStream"> UTF-8 编码的 XML 流。 </param>
    /// <returns> 一个可用于枚举 <see cref="ICard"/> 成员的 <see cref="System.Collections.Generic.IEnumerable{T}"/>。 </returns>
    public static IEnumerable<ICard> Deserialize(Stream xmlStream)
    {
        using XmlReader xmlReader = XmlReader.Create(xmlStream, new XmlReaderSettings
        {
            Async = false,
            IgnoreWhitespace = true,
            IgnoreComments = true
        });

        MarkupElement? markupElement = null;
        Stack<MarkupElement> stack = [];

        while (xmlReader.Read())
        {
            XmlNodeType nodeType = xmlReader.NodeType;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (nodeType)
            {
                case XmlNodeType.Element:
                    Dictionary<string, string> attributes = new();
                    if (xmlReader.HasAttributes)
                    {
                        for (int i = 0; i < xmlReader.AttributeCount; i++)
                        {
                            xmlReader.MoveToAttribute(i);
                            attributes.Add(xmlReader.Name, xmlReader.Value);
                        }

                        xmlReader.MoveToElement();
                    }

                    MarkupElement e = new()
                    {
                        Name = xmlReader.Name,
                        Attributes = attributes,
                        Children = []
                    };

                    if (xmlReader.IsEmptyElement)
                        stack.Peek().Children.Add(e);
                    else
                        stack.Push(e);
                    break;
                case XmlNodeType.EndElement:
                    MarkupElement element = stack.Pop();
                    if (stack.Count == 0)
                        markupElement = element;
                    else
                        stack.Peek().Children.Add(element);
                    break;
                case XmlNodeType.Text:
                    stack.Peek().Text = xmlReader.Value;
                    break;
            }
        }

        return markupElement?.ToCards() ?? [];
    }

    #endregion

    #region Try

    #nullable enable

    /// <summary>
    ///     尝试将卡片消息 XML 标记文件反序列化为 <see cref="ICard"/> 列表。
    /// </summary>
    /// <param name="file"> UTF-8 编码的 XML 文件。 </param>
    /// <param name="cards">
    ///     如果反序列化操作成功，则为一个可用于枚举 <see cref="ICard"/> 成员的
    ///     <see cref="System.Collections.Generic.IEnumerable{T}"/>；否则为 <c>null</c>。
    /// </param>
    /// <returns> 如果反序列化操作成功，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool TryDeserialize(FileInfo file, [NotNullWhen(true)] out IEnumerable<ICard>? cards)
    {
        try
        {
            cards = Deserialize(file);
            return true;
        }
        catch
        {
            cards = null;
            return false;
        }
    }

    /// <summary>
    ///     尝试将卡片消息 XML 标记文本反序列化为 <see cref="ICard"/> 列表。
    /// </summary>
    /// <param name="xmlText"> UTF-8 编码的 XML 文本。 </param>
    /// <param name="cards">
    ///     如果反序列化操作成功，则为一个可用于枚举 <see cref="ICard"/> 成员的
    ///     <see cref="System.Collections.Generic.IEnumerable{T}"/>；否则为 <c>null</c>。
    /// </param>
    /// <returns> 如果反序列化操作成功，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool TryDeserialize(string xmlText, [NotNullWhen(true)] out IEnumerable<ICard>? cards)
    {
        try
        {
            cards = Deserialize(xmlText);
            return true;
        }
        catch
        {
            cards = null;
            return false;
        }
    }

    /// <summary>
    ///     尝试将卡片消息 XML 标记文本流反序列化为 <see cref="ICard"/> 列表。
    /// </summary>
    /// <param name="xmlStream"> UTF-8 编码的 XML 文本流。 </param>
    /// <param name="cards">
    ///     如果反序列化操作成功，则为一个可用于枚举 <see cref="ICard"/> 成员的
    ///     <see cref="System.Collections.Generic.IEnumerable{T}"/>；否则为 <c>null</c>。
    /// </param>
    /// <returns> 如果反序列化操作成功，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool TryDeserialize(Stream xmlStream, [NotNullWhen(true)] out IEnumerable<ICard>? cards)
    {
        try
        {
            cards = Deserialize(xmlStream);
            return true;
        }
        catch
        {
            cards = null;
            return false;
        }
    }

    #endregion
}
