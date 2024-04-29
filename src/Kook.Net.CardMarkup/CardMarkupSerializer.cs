using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using Kook.CardMarkup.Extensions;
using Kook.CardMarkup.Models;

namespace Kook.CardMarkup;

/// <summary>
///     Serializer for Card Message XML markup
/// </summary>
public static class CardMarkupSerializer
{
    #region Async

    /// <summary>
    ///     Deserialize a Card Message XML markup file to a <see cref="ICard"/> list.
    ///     One XML markup file has one card-message element, which can
    ///     contain multiple card elements.
    /// </summary>
    /// <param name="file">UTF-8 encoded XML file</param>
    /// <param name="token">Cancellation token</param>
    /// <returns><see cref="ICard"/> enumerable</returns>
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
    ///     Deserialize a Card Message XML markup text to a <see cref="ICard"/> list.
    ///     One XML markup file has one card-message element, which can
    ///     contain multiple card elements.
    /// </summary>
    /// <param name="xmlText">UTF-8 encoded XML text</param>
    /// <param name="token">Cancellation token</param>
    /// <returns><see cref="ICard"/> enumerable</returns>
    public static async Task<IEnumerable<ICard>> DeserializeAsync(string xmlText, CancellationToken token = default)
    {
        using MemoryStream xmlStream = new(Encoding.UTF8.GetBytes(xmlText));
        return await DeserializeAsync(xmlStream, token);
    }

    /// <summary>
    ///     Deserialize a Card Message XML stream to a <see cref="ICard"/> list.
    ///     One XML markup file has one card-message element, which can
    ///     contain multiple card elements.
    /// </summary>
    /// <param name="xmlStream">UTF-8 encoded XML stream</param>
    /// <param name="token">Cancellation token</param>
    /// <returns><see cref="ICard"/> enumerable</returns>
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
    ///     Deserialize a Card Message XML markup file to a <see cref="ICard"/> list.
    ///     One XML markup file has one card-message element, which can
    ///     contain multiple card elements.
    /// </summary>
    /// <param name="file">UTF-8 encoded XML file</param>
    /// <returns><see cref="ICard"/> enumerable</returns>
    public static IEnumerable<ICard> Deserialize(FileInfo file)
    {
        using FileStream fs = file.OpenRead();
        return Deserialize(fs);
    }

    /// <summary>
    ///     Deserialize a Card Message XML markup text to a <see cref="ICard"/> list.
    ///     One XML markup file has one card-message element, which can
    ///     contain multiple card elements.
    /// </summary>
    /// <param name="xmlText">UTF-8 encoded XML text</param>
    /// <returns><see cref="ICard"/> enumerable</returns>
    public static IEnumerable<ICard> Deserialize(string xmlText)
    {
        using MemoryStream xmlStream = new(Encoding.UTF8.GetBytes(xmlText));
        return Deserialize(xmlStream);
    }

    /// <summary>
    ///     Deserialize a Card Message XML markup file to a <see cref="ICard"/> list.
    ///     One XML markup file has one card-message element, which can
    ///     contain multiple card elements.
    /// </summary>
    /// <param name="xmlStream">UTF-8 encoded XML stream</param>
    /// <returns><see cref="ICard"/> enumerable</returns>
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
    ///     Try to deserialize a Card Message XML markup file to a <see cref="ICard"/> list.
    /// </summary>
    /// <param name="file">UTF-8 encoded XML file</param>
    /// <param name="cards"><see cref="ICard"/> enumerable, will be null if return value is false</param>
    /// <returns>True if deserialization is successful, otherwise false</returns>
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
    ///     Try to deserialize a Card Message XML markup file to a <see cref="ICard"/> list.
    /// </summary>
    /// <param name="xmlText">UTF-8 encoded XML text</param>
    /// <param name="cards"><see cref="ICard"/> enumerable, will be null if return value is false</param>
    /// <returns>True if deserialization is successful, otherwise false</returns>
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
    ///     Try to deserialize a Card Message XML markup file to a <see cref="ICard"/> list.
    /// </summary>
    /// <param name="xmlStream">UTF-8 encoded XML stream</param>
    /// <param name="cards"><see cref="ICard"/> enumerable, will be null if return value is false</param>
    /// <returns>True if deserialization is successful, otherwise false</returns>
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
