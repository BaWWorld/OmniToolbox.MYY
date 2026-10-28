using Dalamud.Game;
using System.Collections.Generic;
using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using OmenTools;
using OmenTools.OmenService;
using OmniToolbox.Common.Module.Abstractions;
using OmniToolbox.Common.Module.Enums;
using OmniToolbox.Common.Module.Models;
using TreeTools.Lifecycle;
using OmniToolbox.UI;

namespace OmniToolbox.LocalModules;

[ModuleApiVersion(1)]
public sealed class ReplyChannelSwitch : OmniModuleBase
{
    public override ModuleInfo Info { get; } = new()
    {
        Category = ModuleCategory.Interface,
        Author = "以默＆MYY"
    };

    public override ModuleLocalization Localization { get; } = ModuleLocalization.Create(
        new Dictionary<string, string>
        {
            ["Title"] = "聊天回复选项",
            ["Description"] = "输入 /r，将当前聊天频道切换为悄悄话。"
        });

    private string searchString = string.Empty;

    protected override void OnStart(FeatureLifetime activation)
    {
        searchString = DService.Instance().ClientState.ClientLanguage switch
        {
            ClientLanguage.Japanese => @"1番目に文字列の指定がありません。： /r",
            ClientLanguage.English => @"“/r” requires a valid string.",
            ClientLanguage.German => @"Das Textkommando „/r“ erfordert den Unterbefehl [Name/Eingabe] an 1. Stelle.",
            ClientLanguage.French => @"L'argument “nom” est manquant (/r).",
            (ClientLanguage)4 => @"“/r”出现问题：1号指定的字串不存在。",
            _ => @"“/r” requires a valid string."
        };

        var chat = DService.Instance().Chat;
        activation.OnStopping(() => chat.CheckMessageHandled -= CheckMessage);
        chat.CheckMessageHandled += CheckMessage;
    }

    private void CheckMessage(IHandleableChatMessage chatMessage)
    {
        if (chatMessage.LogKind != XivChatType.ErrorMessage) return;
        if (chatMessage.Message.TextValue != searchString) return;
        ChatManager.Instance().SendMessage("/t <r>");
        chatMessage.PreventOriginal();
    }

}
