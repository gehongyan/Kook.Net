using System;
using System.Collections.Generic;
using System.Linq;
using Kook.CardMarkup;
using Xunit;

namespace Kook;

[Trait("Category", "Unit")]
public class CardXmlTests
{
    private const string CardXml =
        """
        <?xml version="1.0" encoding="UTF-8" ?>

        <card-message xmlns="https://kooknet.dev"
                      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                      xsi:schemaLocation="https://kooknet.dev https://kooknet.dev/card-message.xsd">
          <card theme="success"
                color="#ABCDEF"
                size="large">
            <modules>
              <header>
                <plain>SECTION_HEADER</plain>
              </header>
              <section mode="right">
                <text>
                  <plain emoji="false">SECTION_PLAIN</plain>
                </text>
                <accessory>
                  <button theme="secondary"
                          click="return-val"
                          value="SECTION_ACCESSORY_BUTTON_VALUE">
                    <plain>SECTION_ACCESSORY_BUTTON</plain>
                  </button>
                </accessory>
              </section>
              <section>
                <text>
                  <kmarkdown>SECTION_KMARKDOWN</kmarkdown>
                </text>
                <accessory>
                  <image src="https://SECTION_MOCK/IMAGE.jpg"
                         size="small"
                         alt="IMAGE_ALT"
                         circle="true"/>
                </accessory>
              </section>
              <images>
                <image src="https://IMAGE_MOCK/IMAGE_1.jpg"/>
                <image src="https://IMAGE_MOCK/IMAGE_2.jpg"/>
                <image src="https://IMAGE_MOCK/IMAGE_3.jpg"/>
              </images>
              <container>
                <image src="https://CONTAINER_MOCK/IMAGE_1.jpg"/>
                <image src="https://CONTAINER_MOCK/IMAGE_2.jpg"/>
                <image src="https://CONTAINER_MOCK/IMAGE_3.jpg"/>
              </container>
              <actions>
                <button>
                  <plain>ACTIONS_BUTTON_1</plain>
                </button>
                <button>
                  <plain>ACTIONS_BUTTON_2</plain>
                </button>
                <button>
                  <plain>ACTIONS_BUTTON_3</plain>
                </button>
              </actions>
              <context>
                <image src="https://CONTEXT_MOCK/IMAGE.jpg"/>
                <plain>CONTEXT_PLAIN</plain>
                <kmarkdown>CONTEXT_KMARKDOWN</kmarkdown>
              </context>
              <divider/>
              <file src="https://FILE_MOCK/FILE.zip"
                    title="FILE_TITLE"/>
              <video src="https://VIDEO_MOCK/VIDEO.mp4"
                     title="VIDEO_TITLE"/>
              <audio src="https://AUDIO_MOCK/AUDIO.aac"
                     title="AUDIO_TITLE"
                     cover="https://AUDIO_MOCK/AUDIO.jpg"/>
              <countdown end="4080251224000"
                         start="4077572824000"
                         mode="second"/>
              <invite code="EvxnOb"/>
            </modules>
          </card>
          <card>
            <modules>
              <divider/>
            </modules>
          </card>
        </card-message>
        """;

    [Fact]
    public void XmlParse()
    {
        List<ICard> cards = CardMarkupSerializer.Deserialize(CardXml).ToList();
        if (cards is not [Card full, Card simple])
        {
            Assert.Fail("Expected two cards");
            return;
        }

        Assert.Equal(CardTheme.Success, full.Theme);
        Assert.Equal(new Color(0xABCDEF), full.Color);
        Assert.Equal(CardSize.Large, full.Size);
        Assert.Equal(13, full.Modules.Length);

        Assert.IsType<HeaderModule>(full.Modules[0]);
        Assert.Equal("SECTION_HEADER", ((HeaderModule)full.Modules[0]).Text.Content);

        Assert.IsType<SectionModule>(full.Modules[1]);
        Assert.Equal(SectionAccessoryMode.Right, ((SectionModule)full.Modules[1])?.Mode);
        Assert.False((((SectionModule)full.Modules[1]).Text as PlainTextElement)?.Emoji);
        Assert.Equal("SECTION_PLAIN", (((SectionModule)full.Modules[1]).Text as PlainTextElement)?.Content);
        Assert.Equal(ButtonTheme.Secondary, (((SectionModule)full.Modules[1]).Accessory as ButtonElement)?.Theme);
        Assert.Equal(ButtonClickEventType.ReturnValue, ((ButtonElement)((SectionModule)full.Modules[1]).Accessory).Click);
        Assert.Equal("SECTION_ACCESSORY_BUTTON_VALUE", ((ButtonElement)((SectionModule)full.Modules[1]).Accessory).Value);
        Assert.Equal("SECTION_ACCESSORY_BUTTON", (((ButtonElement)((SectionModule)full.Modules[1]).Accessory)?.Text as PlainTextElement)?.Content);

        Assert.IsType<SectionModule>(full.Modules[2]);
        Assert.Equal(null, ((SectionModule)full.Modules[2])?.Mode);
        Assert.Equal("SECTION_KMARKDOWN", (((SectionModule)full.Modules[2]).Text as KMarkdownElement)?.Content);
        Assert.Equal("https://SECTION_MOCK/IMAGE.jpg", (((SectionModule)full.Modules[2]).Accessory as ImageElement)?.Source);
        Assert.Equal(ImageSize.Small, ((ImageElement)((SectionModule)full.Modules[2]).Accessory).Size);
        Assert.Equal("IMAGE_ALT", ((ImageElement)((SectionModule)full.Modules[2]).Accessory).Alternative);
        Assert.True(((ImageElement)((SectionModule)full.Modules[2]).Accessory).Circle);

        Assert.IsType<ImageGroupModule>(full.Modules[3]);
        Assert.Equal(3, ((ImageGroupModule)full.Modules[3])?.Elements.Length);
        Assert.Equal("https://IMAGE_MOCK/IMAGE_1.jpg", ((ImageGroupModule)full.Modules[3]).Elements[0].Source);
        Assert.Equal("https://IMAGE_MOCK/IMAGE_2.jpg", ((ImageGroupModule)full.Modules[3]).Elements[1].Source);
        Assert.Equal("https://IMAGE_MOCK/IMAGE_3.jpg", ((ImageGroupModule)full.Modules[3]).Elements[2].Source);

        Assert.IsType<ContainerModule>(full.Modules[4]);
        Assert.Equal(3, ((ContainerModule)full.Modules[4])?.Elements.Length);
        Assert.Equal("https://CONTAINER_MOCK/IMAGE_1.jpg", ((ContainerModule)full.Modules[4]).Elements[0].Source);
        Assert.Equal("https://CONTAINER_MOCK/IMAGE_2.jpg", ((ContainerModule)full.Modules[4]).Elements[1].Source);
        Assert.Equal("https://CONTAINER_MOCK/IMAGE_3.jpg", ((ContainerModule)full.Modules[4]).Elements[2].Source);

        Assert.IsType<ActionGroupModule>(full.Modules[5]);
        Assert.Equal(3, ((ActionGroupModule)full.Modules[5])?.Elements.Length);
        Assert.Equal("ACTIONS_BUTTON_1", (((ActionGroupModule)full.Modules[5]).Elements[0].Text as PlainTextElement)?.Content);
        Assert.Equal("ACTIONS_BUTTON_2", (((ActionGroupModule)full.Modules[5]).Elements[1].Text as PlainTextElement)?.Content);
        Assert.Equal("ACTIONS_BUTTON_3", (((ActionGroupModule)full.Modules[5]).Elements[2].Text as PlainTextElement)?.Content);

        Assert.IsType<ContextModule>(full.Modules[6]);
        Assert.Equal(3, ((ContextModule)full.Modules[6])?.Elements.Length);
        Assert.Equal("https://CONTEXT_MOCK/IMAGE.jpg", (((ContextModule)full.Modules[6]).Elements[0] as ImageElement)?.Source);
        Assert.Equal("CONTEXT_PLAIN", (((ContextModule)full.Modules[6]).Elements[1] as PlainTextElement)?.Content);
        Assert.Equal("CONTEXT_KMARKDOWN", (((ContextModule)full.Modules[6]).Elements[2] as KMarkdownElement)?.Content);

        Assert.IsType<DividerModule>(full.Modules[7]);

        Assert.IsType<FileModule>(full.Modules[8]);
        Assert.Equal("https://FILE_MOCK/FILE.zip", ((FileModule)full.Modules[8]).Source);
        Assert.Equal("FILE_TITLE", ((FileModule)full.Modules[8]).Title);

        Assert.IsType<VideoModule>(full.Modules[9]);
        Assert.Equal("https://VIDEO_MOCK/VIDEO.mp4", ((VideoModule)full.Modules[9]).Source);
        Assert.Equal("VIDEO_TITLE", ((VideoModule)full.Modules[9]).Title);

        Assert.IsType<AudioModule>(full.Modules[10]);
        Assert.Equal("https://AUDIO_MOCK/AUDIO.aac", ((AudioModule)full.Modules[10]).Source);
        Assert.Equal("AUDIO_TITLE", ((AudioModule)full.Modules[10]).Title);
        Assert.Equal("https://AUDIO_MOCK/AUDIO.jpg", ((AudioModule)full.Modules[10]).Cover);

        Assert.IsType<CountdownModule>(full.Modules[11]);
        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(4080251224000), ((CountdownModule)full.Modules[11]).EndTime);
        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(4077572824000), ((CountdownModule)full.Modules[11]).StartTime);
        Assert.Equal(CountdownMode.Second, ((CountdownModule)full.Modules[11]).Mode);

        Assert.IsType<InviteModule>(full.Modules[12]);
        Assert.Equal("EvxnOb", ((InviteModule)full.Modules[12]).Code);

        Assert.Single(simple.Modules);
        Assert.IsType<DividerModule>(simple.Modules[0]);
    }
}
