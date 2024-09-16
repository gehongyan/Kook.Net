namespace Kook;

/// <summary>
///     表示在转换标签时应进行的操作类型。
/// </summary>
/// <seealso cref="Kook.IUserMessage.Resolve(Kook.TagHandling,Kook.TagHandling,Kook.TagHandling,Kook.TagHandling,Kook.TagHandling)"/>
public enum TagHandling
{
    /// <summary>
    ///     不进行任何处理。
    /// </summary>
    /// <remarks>
    ///     例如：
    ///     <list type="table">
    ///         <listheader> <term> 原始值 </term> <description> 转换结果 </description> </listheader>
    ///         <item> <term> <c>(met)2810246202(met)</c> </term> <description> <c>(met)2810246202(met)</c> </description> </item>
    ///         <item> <term> <c>(chn)5017360261312802(chn)</c> </term> <description> <c>(chn)5017360261312802(chn)</c> </description> </item>
    ///         <item> <term> <c>(rol)6790760(rol)</c> </term> <description> <c>(rol)6790760(rol)</c> </description> </item>
    ///         <item> <term> <c>(met)all(met)</c> </term> <description> <c>(met)all(met)</c> </description> </item>
    ///         <item> <term> <c>(met)here(met)</c> </term> <description> <c>(met)here(met)</c> </description> </item>
    ///         <item> <term> <c>(emj)kooknet-logo(emj)[1591057729615250/E8fy7hpRFU1jk1jk]</c> </term> <description> <c>(emj)kooknet-logo(emj)[1591057729615250/E8fy7hpRFU1jk1jk]</c> </description> </item>
    ///     </list>
    /// </remarks>
    Ignore = 0,

    /// <summary>
    ///     移除标签。
    /// </summary>
    /// <remarks>
    ///     例如：
    ///     <list type="table">
    ///         <listheader> <term> 原始值 </term> <description> 转换结果 </description> </listheader>
    ///         <item> <term> <c>(met)2810246202(met)</c> </term> <description> <c></c> </description> </item>
    ///         <item> <term> <c>(chn)5017360261312802(chn)</c> </term> <description> <c></c> </description> </item>
    ///         <item> <term> <c>(rol)6790760(rol)</c> </term> <description> <c></c> </description> </item>
    ///         <item> <term> <c>(met)all(met)</c> </term> <description> <c></c> </description> </item>
    ///         <item> <term> <c>(met)here(met)</c> </term> <description> <c></c> </description> </item>
    ///         <item> <term> <c>(emj)kooknet-logo(emj)[1591057729615250/E8fy7hpRFU1jk1jk]</c> </term> <description> <c></c> </description> </item>
    ///     </list>
    /// </remarks>
    Remove,

    /// <summary>
    ///     转换为名称，包含提及前缀。
    /// </summary>
    /// <remarks>
    ///     例如：
    ///     <list type="table">
    ///         <listheader> <term> 原始值 </term> <description> 转换结果 </description> </listheader>
    ///         <item> <term> <c>(met)2810246202(met)</c> </term> <description> <c>@用户名</c> </description> </item>
    ///         <item> <term> <c>(chn)5017360261312802(chn)</c> </term> <description> <c>#频道名称</c> </description> </item>
    ///         <item> <term> <c>(rol)6790760(rol)</c> </term> <description> <c>@角色名称</c> </description> </item>
    ///         <item> <term> <c>(met)all(met)</c> </term> <description> <c>@全体成员</c> </description> </item>
    ///         <item> <term> <c>(met)here(met)</c> </term> <description> <c>@在线成员</c> </description> </item>
    ///         <item> <term> <c>(emj)kooknet-logo(emj)[1591057729615250/E8fy7hpRFU1jk1jk]</c> </term> <description> <c>:kooknet-logo:</c> </description> </item>
    ///     </list>
    /// </remarks>
    Name,

    /// <summary>
    ///     转换为名称，但不包含提及前缀。
    /// </summary>
    /// <remarks>
    ///     例如：
    ///     <list type="table">
    ///         <listheader> <term> 原始值 </term> <description> 转换结果 </description> </listheader>
    ///         <item> <term> <c>(met)2810246202(met)</c> </term> <description> <c>用户名</c> </description> </item>
    ///         <item> <term> <c>(chn)5017360261312802(chn)</c> </term> <description> <c>频道名称</c> </description> </item>
    ///         <item> <term> <c>(rol)6790760(rol)</c> </term> <description> <c>角色名称</c> </description> </item>
    ///         <item> <term> <c>(met)all(met)</c> </term> <description> <c>全体成员</c> </description> </item>
    ///         <item> <term> <c>(met)here(met)</c> </term> <description> <c>在线成员</c> </description> </item>
    ///         <item> <term> <c>(emj)kooknet-logo(emj)[1591057729615250/E8fy7hpRFU1jk1jk]</c> </term> <description> <c>kooknet-logo</c> </description> </item>
    ///     </list>
    /// </remarks>
    NameNoPrefix,

    /// <summary>
    ///     转换为名称，包含提及前缀。如果提及标签为用户提及，则还会包含用户的识别号。
    /// </summary>
    /// <remarks>
    ///     例如：
    ///     <list type="table">
    ///         <listheader> <term> 原始值 </term> <description> 转换结果 </description> </listheader>
    ///         <item> <term> <c>(met)2810246202(met)</c> </term> <description> <c>@用户名#0001</c> </description> </item>
    ///         <item> <term> <c>(chn)5017360261312802(chn)</c> </term> <description> <c>#频道名称</c> </description> </item>
    ///         <item> <term> <c>(rol)6790760(rol)</c> </term> <description> <c>@角色名称</c> </description> </item>
    ///         <item> <term> <c>(met)all(met)</c> </term> <description> <c>@全体成员</c> </description> </item>
    ///         <item> <term> <c>(met)here(met)</c> </term> <description> <c>@在线成员</c> </description> </item>
    ///         <item> <term> <c>(emj)kooknet-logo(emj)[1591057729615250/E8fy7hpRFU1jk1jk]</c> </term> <description> <c>:kooknet-logo:</c> </description> </item>
    ///     </list>
    /// </remarks>
    FullName,

    /// <summary>
    ///     转换为名称，但不包含提及前缀。如果提及标签为用户提及，则还会包含用户的识别号。
    /// </summary>
    /// <remarks>
    ///     例如：
    ///     <list type="table">
    ///         <listheader> <term> 原始值 </term> <description> 转换结果 </description> </listheader>
    ///         <item> <term> <c>(met)2810246202(met)</c> </term> <description> <c>用户名#0001</c> </description> </item>
    ///         <item> <term> <c>(chn)5017360261312802(chn)</c> </term> <description> <c>频道名称</c> </description> </item>
    ///         <item> <term> <c>(rol)6790760(rol)</c> </term> <description> <c>角色名称</c> </description> </item>
    ///         <item> <term> <c>(met)all(met)</c> </term> <description> <c>全体成员</c> </description> </item>
    ///         <item> <term> <c>(met)here(met)</c> </term> <description> <c>在线成员</c> </description> </item>
    ///         <item> <term> <c>(emj)kooknet-logo(emj)[1591057729615250/E8fy7hpRFU1jk1jk]</c> </term> <description> <c>kooknet-logo</c> </description> </item>
    ///     </list>
    /// </remarks>
    FullNameNoPrefix,

    /// <summary>
    ///     使用 <c>U+200B</c> 零宽空格字符转义标签。
    /// </summary>
    /// <remarks>
    ///     例如：
    ///     <list type="table">
    ///         <listheader> <term> 原始值 </term> <description> 转换结果 </description> </listheader>
    ///         <item> <term> <c>(met)2810246202(met)</c> </term> <description> <c>(met)[ZWSP]2810246202(met)</c> </description> </item>
    ///         <item> <term> <c>(chn)5017360261312802(chn)</c> </term> <description> <c>(chn)[ZWSP]5017360261312802(chn)</c> </description> </item>
    ///         <item> <term> <c>(rol)6790760(rol)</c> </term> <description> <c>(rol)[ZWSP]6790760(rol)</c> </description> </item>
    ///         <item> <term> <c>(met)all(met)</c> </term> <description> <c>(met)[ZWSP]all(met)</c> </description> </item>
    ///         <item> <term> <c>(met)here(met)</c> </term> <description> <c>(met)[ZWSP]here(met)</c> </description> </item>
    ///         <item> <term> <c>(emj)kooknet-logo(emj)[1591057729615250/E8fy7hpRFU1jk1jk]</c> </term> <description> <c>(emj)[ZWSP]kooknet-logo(emj)[[ZWSP]1591057729615250/E8fy7hpRFU1jk1jk]</c> </description> </item>
    ///     </list>
    ///     其中，<c>[ZWSP]</c> 代表 <c>U+200B</c> 零宽空格字符。
    /// </remarks>
    Sanitize
}
