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
    /// <summary>
    ///     Deserialize a Card Message XML markup file to a Card list
    /// </summary>
    /// <param name="file">UTF-8 encoded XML file</param>
    /// <returns></returns>
    public static async Task<IEnumerable<ICard>> DeserializeAsync(FileInfo file)
    {
        using var fs = file.OpenRead();
        return await DeserializeAsync(fs);
    }

    /// <summary>
    ///     Deserialize a Card Message XML markup text to a Card list
    /// </summary>
    /// <param name="xmlText">UTF-8 encoded XML text</param>
    /// <returns></returns>
    public static async Task<IEnumerable<ICard>> DeserializeAsync(string xmlText)
    {
        using var xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlText));
        return await DeserializeAsync(xmlStream);
    }

    /// <summary>
    ///     Deserialize a Card Message XML stream to a Card list
    /// </summary>
    /// <param name="xmlStream">UTF-8 encoded XML stream</param>
    /// <returns></returns>
    public static async Task<IEnumerable<ICard>> DeserializeAsync(Stream xmlStream)
    {
        using var xmlReader = XmlReader.Create(xmlStream, new XmlReaderSettings
        {
            Async = true,
            IgnoreWhitespace = true,
            IgnoreComments = true
        });

        MarkupElement markupElement = null;
        var stack = new Stack<MarkupElement>();

        while (await xmlReader.ReadAsync())
        {
            var nodeType = xmlReader.NodeType;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (nodeType)
            {
                case XmlNodeType.Element:
                    var attributes = new Dictionary<string, string>();
                    if (xmlReader.HasAttributes)
                    {
                        for (int i = 0; i < xmlReader.AttributeCount; i++)
                        {
                            xmlReader.MoveToAttribute(i);
                            attributes.Add(xmlReader.Name, xmlReader.Value);
                        }

                        xmlReader.MoveToElement();
                    }

                    var e = new MarkupElement
                    {
                        Name = xmlReader.Name,
                        Attributes = attributes,
                        Children = []
                    };

                    if (xmlReader.IsEmptyElement)
                    {
                        stack.Peek().Children.Add(e);
                    }
                    else
                    {
                        stack.Push(e);
                    }
                    break;
                case XmlNodeType.EndElement:
                    var element = stack.Pop();
                    if (stack.Count == 0)
                    {
                        markupElement = element;
                    }
                    else
                    {
                        stack.Peek().Children.Add(element);
                    }
                    break;

                case XmlNodeType.Text:
                    stack.Peek().Text = xmlReader.Value.Trim();
                    break;
            }
        }

        return markupElement.ToCards();
    }
}
