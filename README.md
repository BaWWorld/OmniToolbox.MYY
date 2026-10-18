# OmniToolbox.MYY

MYY 的 Omni Toolbox（树树妙妙屋）在线模块仓库。

## 模块列表

- **隐藏同好菜单**（HideCommunityButtons）— 隐藏中文端社交菜单中的“同好招募”和“同好会”。

- **显示长婚礼长纱**（LongWeddingVeil）— 将婚礼头纱模型替换为长款头纱。

- **聊天回复选项**（ReplyChannelSwitch）— 输入 /r，将当前聊天频道切换为悄悄话。作者：以默＆MYY。

## 在 Omni 中添加

在「插件设置 → 在线模块」中填写清单地址。

地址：

```text
https://github.com/BaWWorld/OmniToolbox.MYY
```

链接：

```text
https://raw.githubusercontent.com/BaWWorld/OmniToolbox.MYY/main/TreeHouseModules.json
```

首次安装后模块默认关闭，在「树树妙妙屋 → 在线」中手动启用。

## 仓库结构

```text
OmniToolbox.MYY/
  Modules/
    HideCommunityButtons.cs
    LongWeddingVeil.cs
    ReplyChannelSwitch.cs
  README.md
  TreeHouseModules.json
```

各模块作者见模块列表及清单。模块文件由 Omni 在线模块加载器下载，并依据
`TreeHouseModules.json` 中的 SHA256 校验值进行校验。
