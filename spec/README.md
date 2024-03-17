# 卡片消息 XML

## 命名空间

XML 命名空间为 `https://kooknet.dev`, 如果你使用了 XML 编辑器，可以在根节点中添加命名空间声明：

```xml
<?xml version="1.0" encoding="UTF-8"?>
<card-message xmlns="https://kooknet.dev">
    <!-- Something else -->
</card-message>
```

像上面的写法，你需要将 `card-message.xsd` 放置在项目文件中，这样编辑器才能自动检查 XML 的正确性。

或者，你可以将 `XSD` 文件导入到所使用的编辑器的命名空间列表中，例如在 JetBrains Rider 中（以及其他 JetBrains IDE）,
打开 `Settings` -> `Languages & Frameworks` -> `Schemas and DTDs` -> `Add` -> `From File`，然后输入
`card-message.xsd` 的路径即可。

或者，你可以在根节点中添加命名空间位置的声明：

```xml
<?xml version="1.0" encoding="UTF-8"?>
<card-message xmlns="https://kooknet.dev"
              xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
              xsi:schemaLocation="https://kooknet.dev https://raw.githubusercontent.com/gehongyan/Kook.Net/master/spec/card-message.xsd">
    <!-- Something else -->
</card-message>
```

```xml
<?xml version="1.0" encoding="UTF-8"?>
<card-message xmlns="https://kooknet.dev"
              xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
              xsi:schemaLocation="https://kooknet.dev file:///path/to/card-message.xsd">
    <!-- Something else -->
</card-message>
```
