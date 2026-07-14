namespace PROJ.Communication.Network;

public sealed class ClientCommandMessage
{
    public string Type { get; set; } = "command";
    public string Key { get; set; } = string.Empty;
}
