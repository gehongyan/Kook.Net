namespace KaiHeiLa;

public static class MentionUtils
{
    internal static string MentionUser(string id) => $"(met){id}(met)";
    public static string MentionUser(uint id) => MentionUser(id.ToString());
    
    internal static string MentionChannel(string id) => $"(chn){id}(chn)";
    public static string MentionChannel(ulong id) => MentionChannel(id.ToString());
    
    internal static string MentionRole(string id) => $"(rol){id}(rol)";
    public static string MentionRole(ulong id) => MentionRole(id.ToString());

    
    
    


}