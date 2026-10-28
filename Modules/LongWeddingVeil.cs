using Dalamud.Hooking;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using OmenTools;
using OmenTools.Interop.Game.Models;
using OmniToolbox.Common.Module.Abstractions;
using OmniToolbox.Common.Module.Enums;
using OmniToolbox.Common.Module.Models;
using TreeTools.Lifecycle;
using OmniToolbox.UI;

namespace OmniToolbox.TreeHouseOnline;

/// <summary>
/// 行为参考 SimpleTweaksPlugin 的 LongVeil。
/// 本文件为独立重写；SimpleTweaksPlugin 使用 AGPL-3.0-or-later。
/// </summary>
[ModuleApiVersion(1)]
public sealed unsafe class LongWeddingVeil : OmniModuleBase
{
    private const uint LongVeilModelId = 208;
    private const uint ShortVeilModelId = 199;
    private static readonly CompSig SetEquipmentSlotModelSignature = new(
        "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 8B DA 49 8B F0 48 8B F9");

    private Hook<Human.Delegates.SetEquipmentSlotModel>? hook;

    public override ModuleInfo Info { get; } = new()
    {
        Category = ModuleCategory.Interface,
        Author = "MYY"
    };

    public override ModuleLocalization Localization { get; } = ModuleLocalization.Create(
        new Dictionary<string, string>
        {
            ["Title"] = "显示长婚礼长纱",
            ["Description"] = "将婚礼头纱模型替换为长款头纱。"
        });

    protected override void OnStart(FeatureLifetime activation)
    {
        hook = SetEquipmentSlotModelSignature.GetHook<Human.Delegates.SetEquipmentSlotModel>(Detour);
        activation.Add(hook);
        hook.Enable();
    }

    private bool Detour(Human* human, uint slot, EquipmentModelId* model)
    {
        if (slot == 0 && model != null && model->Id == LongVeilModelId)
        {
            model->Id = (ushort)ShortVeilModelId;
        }

        return hook!.Original(human, slot, model);
    }
}
