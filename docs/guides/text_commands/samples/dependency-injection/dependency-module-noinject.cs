// 通过依赖注入服务注入可公共写属性有时是预期之外的

// 可通过两种方式显式告知 Kook.Net 不要为属性进行注入：
// 限制访问修饰符 / 标记 DontInjectAttribute 特性标签

// 限制属性的访问修饰符
public class ImageModule : ModuleBase<SocketCommandContext>
{
    public ImageService ImageService { get; }
    public ImageModule()
    {
        ImageService = new ImageService();
    }
}

// 标记 DontInjectAttribute 特性标签
public class ImageModule : ModuleBase<SocketCommandContext>
{
    [DontInject]
    public ImageService ImageService { get; set; }
    public ImageModule()
    {
        ImageService = new ImageService();
    }
}