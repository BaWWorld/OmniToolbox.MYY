using System.Collections.Frozen;
using System;
using System.Collections.Generic;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using OmenTools;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;
using OmniToolbox.Common.Module.Abstractions;
using OmniToolbox.Common.Module.Enums;
using OmniToolbox.Common.Module.Models;
using OmniToolbox.Host;
using TreeTools.Lifecycle;
using OmniToolbox.UI;

namespace OmniToolbox.TreeHouseOnline;

[ModuleApiVersion(1)]
public sealed unsafe class HideCommunityButtons : OmniModuleBase
{
    private const int StartIndex = 5;
    private const int MaxEntries = 20;
    private Hook<AgentHUD.Delegates.OpenSystemMenu>? openHook;
    private Hook<GenerateSystemMenuDelegate>? generateHook;

    private static readonly CompSig GenerateSig = new(
        "40 53 55 56 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 49 8B D9");
    private delegate byte GenerateSystemMenuDelegate(AgentHUD* agent, AtkValue* values, int count, ulong eventKind);

    public override ModuleInfo Info { get; } = new()
    {
        Category = ModuleCategory.Interface,
        Author = "MYY"
    };

    public override ModuleLocalization Localization { get; } = ModuleLocalization.Create(
        new Dictionary<string, string>
        {
            ["Title"] = "隐藏同好菜单",
            ["Description"] = "隐藏中文端社交菜单中的“同好招募”和“同好会”。"
        });

    protected override void OnStart(FeatureLifetime activation)
    {
        openHook = DService.Instance().Hook.HookFromMemberFunction(
            typeof(AgentHUD.MemberFunctionPointers), "OpenSystemMenu",
            (AgentHUD.Delegates.OpenSystemMenu)OpenDetour);
        activation.Add(openHook);
        generateHook = GenerateSig.GetHook<GenerateSystemMenuDelegate>(GenerateDetour);
        activation.Add(generateHook);
        openHook.Enable();
        generateHook.Enable();
    }

    private void OpenDetour(AgentHUD* agent, AtkValue* values, uint menuSize)
    {
        if (values == null || menuSize > MaxEntries)
        {
            openHook!.Original(agent, values, menuSize);
            return;
        }

        using var filtered = new RentedAtkValues(StartIndex + MaxEntries * 2);
        for (var i = 0; i < StartIndex; i++) filtered[i].Copy(&values[i]);
        var newSize = 0;
        for (var i = 0; i < menuSize; i++)
        {
            var source = StartIndex + (int)i;
            var command = (uint)values[source].Int;
            if (BlockedCommandIds.Contains(command)) continue;
            var target = StartIndex + newSize++;
            filtered[target].Copy(&values[source]);
            filtered[target + MaxEntries].Copy(&values[source + MaxEntries]);
        }
        filtered[3].SetInt(newSize);
        openHook!.Original(agent, filtered, (uint)newSize);
    }

    private byte GenerateDetour(AgentHUD* agent, AtkValue* values, int count, ulong eventKind)
    {
        if (eventKind == 102 || values == null || values[0].Int != 1 || (values[4].Int & 256) != 0)
            return generateHook!.Original(agent, values, count, eventKind);

        var commands = new List<int>();
        for (var i = StartIndex; i < count; i++)
        {
            if (values[i].Type == AtkValueType.Undefined) break;
            var command = (uint)values[i].Int;
            if (!BlockedCommandIds.Contains(command)) commands.Add(values[i].Int);
        }

        using var filtered = new RentedAtkValues(count);
        for (var i = 0; i < StartIndex; i++) filtered[i].Copy(&values[i]);
        for (var i = 0; i < commands.Count; i++) filtered[i + StartIndex].SetInt(commands[i]);
        filtered[4].SetInt(commands.Count);
        return generateHook!.Original(agent, filtered, count, eventKind);
    }

    private static readonly FrozenSet<uint> BlockedCommandIds =
    [
        85, // 同好会
        86  // 同好招募
    ];
}
