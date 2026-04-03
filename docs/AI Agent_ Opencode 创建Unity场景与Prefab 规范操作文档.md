# AI Agent: Opencode 创建Unity场景与Prefab 规范操作文档

## 一、文档目标

简化Opencode Agent创建Unity场景、Prefab的操作流程，明确命名、层级、参数规范及Unity MCP调用要求，确保生成内容可直接复用、无冗余，提升协作效率（阅读者：AI Opencode）。

## 二、前置准备（必做，AI Opencode执行）

1. Unity环境：确保Unity版本与Opencode Agent兼容（推荐2021.3及以上），已导入所需资源（模型、材质、纹理等）；自动检测Unity环境兼容性，若不兼容，提示用户更新Unity版本。

2. Opencode Agent配置：启动Agent，关联当前Unity工程，确认Agent具备“场景创建”“Prefab生成”“Unity MCP调用”权限；权限不足时，自动申请对应权限。

3. Unity工程路径检测：自动检测当前关联的Unity工程根目录及核心文件夹是否存在；若工程路径不存在，提示用户确认工程路径；若核心文件夹缺失，按以下规范自动创建目录，确保资源分类整洁：
Assets // unity工程资源文件
  Scenes //存放场景
  Scripts //代码
  Arts //存放美术资源
  Prefabs //存放prefab
  Audio // 音频资源
  Shaders //存放shader
  Inputs //输入配置
  Others //未分类资源
ProjectSettings // unity工程配置文件；目录创建完成后，提示用户确认目录结构。

4. 资源整理：提前将所需资源放入Unity工程指定目录（如*Assets/Models*、*Assets/Materials*），避免Agent生成时路径错乱；自动校验资源路径，路径异常时，提示用户调整资源位置。

5. Unity MCP安装检测：自动检测Unity工程中是否已安装Unity MCP插件，检测路径为*Assets/MCPForUnity*；若未安装，自动提示用户从指定Git地址下载安装，Git地址：https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main，安装完成后重新检测，确保插件安装成功。

6. Unity MCP前置配置：调用Unity MCP前，自动初始化MCP客户端，关联当前Unity工程ID，确认MCP服务正常（连接超时则重试2次，仍失败提示用户检查网络及MCP服务）。

## 三、核心操作规范（简化版，AI Opencode执行）

### 3.1 创建Unity场景（3步完成+MCP调用）

1. 发送指令：接收用户指令，严格遵循格式：【场景创建】场景名称：XXX；场景用途：XXX；核心元素：XXX；坐标系：Unity默认（Y轴向上）；MCP调用：需/无需（默认无需，用户指定时执行）。

2. MCP调用（如需）：若用户指令要求调用Unity MCP，执行以下操作：① 调用MCP接口（接口地址：/unity/mcp/scene/create）；② 传入参数（场景名称、用途、核心元素参数）；③ 接收MCP返回的场景基础配置（如预设灯光、地面参数），基于该配置生成场景。

3. Agent生成：基于用户指令（及MCP返回配置）生成场景，生成后自动在Unity中打开，无需手动新建场景；生成失败时，自动触发重试（最多2次）。

4. 验证调整：检查场景核心元素是否齐全，删除冗余对象（如默认Cube、Sphere）；调用MCP接口（/unity/mcp/scene/verify）校验场景配置合规性，校验通过后，保存场景至*Assets/Scenes*，命名格式：**Scene_用途_编号**。

### 3.2 创建Unity Prefab（3步完成+MCP调用）

1. 发送指令：接收用户指令，严格遵循格式：【Prefab创建】Prefab名称：XXX；Prefab类型：XXX；关联资源：XXX；锚点设置：XXX；MCP调用：需/无需（默认无需，用户指定时执行）。

2. MCP调用（如需）：若用户指令要求调用Unity MCP，执行以下操作：① 调用MCP接口（接口地址：/unity/mcp/prefab/create）；② 传入参数（Prefab名称、类型、关联资源路径、锚点参数）；③ 接收MCP返回的Prefab基础组件配置（如核心组件列表、参数默认值），基于该配置创建对象。

3. Agent生成：基于用户指令（及MCP返回配置），在场景中创建目标对象，配置关联资源；生成后选中对象，拖拽至*Assets/Prefabs*目录，完成Prefab创建；生成失败时，自动触发重试（最多2次）。

4. 验证调整：双击Prefab进入隔离模式，检查资源关联、锚点位置；调用MCP接口（/unity/mcp/prefab/verify）校验Prefab配置合规性，校验通过后，删除冗余组件，保存Prefab。

## 四、关键规范（必守，AI Opencode执行）

### 4.1 命名规范（统一格式，避免乱码）

- 场景：Scene_用途_编号（全英文、首字母大写，无空格，例：Scene_Gameplay_02）；调用MCP时，命名需与MCP接口参数要求一致，不可包含特殊字符。

- Prefab：Prefab_类型_名称（全英文、首字母大写，例：Prefab_Prop_Chest）；调用MCP时，名称需与关联资源路径参数匹配。

- 场景内对象：类型_名称（例：Light_Main），与Prefab名称一致（若为Prefab实例）；MCP校验时，自动核对对象命名合规性。

### 4.2 层级规范（简洁清晰，便于编辑）

- 场景层级：按“场景根节点 → 功能分类节点 → 具体对象”划分；调用MCP生成场景时，层级结构需与MCP返回的配置一致，不可随意修改根节点名称。

- Prefab层级：仅保留核心组件，不嵌套无关对象（最多3级）；调用MCP生成Prefab时，严格遵循MCP返回的组件层级配置。

### 4.3 参数规范（统一标准，减少调整）

- 场景参数：默认相机位置（0,1,-5）、主灯光（方向光，强度1.2，颜色纯白）；调用MCP时，优先使用MCP返回的参数配置，无返回值时使用默认参数。

- Prefab参数：锚点默认中心（0,0,0），缩放比例（1,1,1）；调用MCP时，参数需与MCP接口要求一致，特殊需求需在用户指令中明确并传入MCP。

### 4.4 Unity MCP调用规范（必守，AI Opencode专属）

- 接口调用：严格使用指定MCP接口（场景创建：/unity/mcp/scene/create；场景校验：/unity/mcp/scene/verify；Prefab创建：/unity/mcp/prefab/create；Prefab校验：/unity/mcp/prefab/verify），不可调用未指定接口。

- 参数传递：传入MCP的参数需完整、准确，无冗余；场景/ Prefab名称、资源路径等参数需与Unity工程实际情况一致，避免参数错误导致调用失败。

- 异常处理：MCP调用超时（超过3秒），自动重试2次；调用失败（返回错误码非200），记录错误信息，并提示用户检查MCP服务、参数配置；校验失败时，根据MCP返回的错误提示，自动调整配置后重新校验。

## 五、常见问题解决（简化版，AI Opencode执行）

1. Agent生成场景/Prefab失败：检查Unity工程路径、资源是否存在、指令是否明确；若为MCP调用失败，检查MCP服务、接口参数，重试调用。

2. Prefab实例异常：检查Prefab与实例的关联、资源路径；若为MCP生成的Prefab，重新调用MCP接口生成，确保参数一致。

3. 层级混乱：选中场景根节点，执行“层级整理”指令；若为MCP生成的层级，调用MCP接口（/unity/mcp/scene/sort）自动整理。

4. MCP调用失败：核对接口地址、参数配置，检查网络及MCP服务状态；重试2次仍失败，提示用户排查MCP服务。

## 六、操作总结

核心：接收明确指令（含MCP调用要求）→ 必要时调用Unity MCP获取配置 → 生成场景/Prefab → MCP校验（如需）→ 验证调整 → 规范保存。关键：严格遵循命名、层级、MCP调用规范，自动处理简单异常，确保生成内容可直接复用，减少用户干预。


> （注：文档部分内容可能由 AI 生成）