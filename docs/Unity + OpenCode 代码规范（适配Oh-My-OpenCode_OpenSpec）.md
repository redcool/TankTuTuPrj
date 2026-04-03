# Unity + OpenCode 代码规范（适配Oh-My-OpenCode/OpenSpec）

适用场景：使用 OpenCode、OpenSpec、Oh-My-OpenCode 插件的 Unity 游戏开发，轻量化、易执行、可被插件识别校验，核心提升代码可读性与团队协作一致性。

## 一、基础规范（Oh-My-OpenCode 自动格式化适配）

### 1.1 文件命名

- 编码文件：统一使用 **PascalCase（大驼峰）**，无空格、中文、特殊字符

- 强制要求：脚本文件名 = 类名（完全一致）

- ✅ 正确示例：PlayerController.cs、UIManager.cs、GameConfig.cs

- ❌ 错误示例：playerController.cs、玩家控制.cs、Player_Controller.cs

### 1.2 编码格式

- 编码格式：UTF-8（统一标准，避免乱码）

- 缩进：4个空格（禁用Tab，Oh-My-OpenCode可设置自动替换）

- 行宽：单行文不超过120字符（避免横向滚动）

- 文件结尾：保留1行空行（插件格式化默认要求）

### 1.3 代码存放规范（符合Unity官方标准）

- 核心规则：所有业务逻辑运行时脚本，必须存放于 **unity_prj/Assets/Scripts** 目录下，禁止散放在Assets根目录、Resources等非专用脚本目录（Editor编辑器扩展、第三方插件脚本除外）。

- 补充要求：脚本目录需按业务模块划分（如Player、UI、Utils），命名空间与目录结构对应，方便插件识别与团队协作。

## 二、命名规范（OpenSpec 自动校验核心）

遵循 C# + Unity 通用规范，OpenSpec 可配置自动校验，禁止违规命名

|元素类型|命名风格|示例|
|---|---|---|
|类、结构体、枚举|PascalCase|PlayerHealth、GameState、ItemData|
|接口|I + PascalCase|IDamageable、ISaveable、IInteractable|
|方法|PascalCase|TakeDamage()、LoadGame()、InitPlayer()|
|公有字段/属性|PascalCase|public int MaxHealth;、public bool IsAlive { get; set; }|
|私有字段|camelCase + 前缀_|private float _moveSpeed;、private Rigidbody2D _rb;|
|常量|全大写 + 下划线|const float PLAYER_SPEED = 5f;、const string GROUND_TAG = "Ground";|
|局部变量|camelCase|int currentHp = 100;、bool isGrounded = false;|
|枚举值|PascalCase|Idle、Run、Jump、Attack|
### 2.1 禁止命名

- 无意义命名：a、temp、data、test（无法被插件识别为有效命名）

- 缩写滥用：仅通用缩写（HP、UI、ID、FPS）可使用，禁止自定义缩写（如PlrCtrl替代PlayerController）

## 三、代码结构规范（OpenCode 代码片段适配）

### 3.1 固定脚本结构（OpenCode可配置自动生成模板）

```csharp
using UnityEngine; // 系统引用在上，自定义引用在下
using System;
using Game.Player; // 自定义命名空间

/// <summary>
/// 类注释：简述脚本功能（OpenSpec 强制要求）
/// 作者：XXX
/// 最后修改时间：XXXX-XX-XX
/// </summary>
namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        // 1. 常量/静态字段（优先定义）
        private const float MAX_SPEED = 10f;
        
        // 2. 序列化私有字段（Inspector可见），添加Header/Tooltip
        [Header("移动设置")]
        [Tooltip("玩家移动速度")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpForce = 7f;
        
        // 3. 私有字段（非序列化，仅脚本内部使用）
        private Rigidbody2D _rb;
        private bool _isGrounded;
        
        // 4. 公有属性（对外暴露，禁止直接暴露公有字段）
        public int CurrentHp { get; private set; }
        
        // 5. 生命周期函数（按执行顺序排列，Oh-My-OpenCode可自动排序）
        private void Awake() { InitComponent(); }
        private void Start() { InitData(); }
        private void Update() { CheckInput(); }
        private void FixedUpdate() { MovePlayer(); }
        
        // 6. 公有方法（对外提供调用）
        public void TakeDamage(int damage) { /* 逻辑 */ }
        
        // 7. 私有方法（脚本内部调用，按功能拆分）
        private void InitComponent() { _rb = GetComponent<Rigidbody2D>(); }
        private void MovePlayer() { /* 移动逻辑 */ }
    }
}

```

### 3.2 生命周期函数规范

- 仅保留实际使用的生命周期函数，删除空函数（插件可检测并提示）

- 禁止在 Update() 中写复杂逻辑，拆分到独立方法（提升性能与可读性）

- FixedUpdate() 仅用于物理相关逻辑，Update() 用于输入、UI更新等

### 3.3 方法单一职责规范

- 核心规则：**每个方法只做一件事**，单方法有效代码行数（不含注释、空行、大括号）严格控制在50行以内，Oh-My-OpenCode可配置行数检测告警。

- 具体要求：方法仅完成单一语义功能，不可混合多个独立逻辑；方法名需清晰体现其唯一职责，禁止模糊命名（如DoSomething()）；超出50行需按功能拆分为多个子方法。

- 示例：
                  `// 正确：拆分单一职责方法
private void Update()
{
    if (!_isAlive) return;
    CheckInput();
    UpdateAnimation();
}
// 仅处理输入检测
private void CheckInput()
{
    _moveInput = Input.GetAxisRaw("Horizontal");
    if (Input.GetButtonDown("Jump")) Jump();
}
// 仅处理跳跃逻辑
private void Jump()
{
    _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
` `}`

### 3.4 MonoBehaviour 调度规范

- 核心规则：MonoBehaviour类型脚本，需遵循Unity生命周期执行顺序，建立清晰、可控的调度逻辑，禁止无序调用。

- 具体要求：
                  Awake()：仅用于组件缓存、单例赋值，禁止调用其他脚本业务方法；

- Start()：仅用于数据初始化、事件注册，可调用Awake中初始化的引用；

- Update()：仅用于输入检测、状态判断等非物理逻辑；

- FixedUpdate()：仅用于物理相关逻辑（刚体移动、碰撞检测）；

- 跨脚本调度优先使用事件/接口，禁止直接调用其他MonoBehaviour的生命周期函数。

## 四、注释规范（OpenSpec 强制校验）

### 4.1 必须添加注释的场景

- 类/接口：标注用途、作者、最后修改时间

- 公共方法：标注功能、参数、返回值（无返回值可省略）、异常（如有）

- 复杂逻辑块：标注代码作用（避免后续维护误解）

- 魔法数值：解释数值含义（如 7f 标注为“跳跃力度，适配当前物理参数”）

### 4.2 注释格式（插件可识别）

```csharp
/// <summary>
/// 玩家受伤处理方法（核心功能：扣除血量，触发死亡逻辑）
/// </summary>
/// <param name="damage">受到的伤害值（必须>0）</param>
public void TakeDamage(int damage)
{
    if (damage <= 0) return; // 异常值过滤（注释：避免负伤害导致血量增加）
    
    // 防止血量低于0，确保逻辑合理性
    CurrentHp = Mathf.Max(0, CurrentHp - damage);
    
    // 血量为0时，触发死亡回调
    if (CurrentHp <= 0)
    {
        OnPlayerDeath();
    }
}

```

## 五、编码层次规范（贴近MVC模式，数据与代码分层）

### 5.1 核心分层原则

- 严格遵循“数据与代码分离”原则，贴近MVC模式架构，明确各层次职责，降低耦合，提升代码可维护性与可测试性，适配OpenCode插件的模块化管理。

- 分层核心要求：数据层（模型）仅负责数据存储与基础校验，业务层（控制器）仅负责逻辑调度，视图层（UI）仅负责显示与交互，禁止跨层次混淆职责。

### 5.2 ValueObject 数据类规范（优先定义）

- 核心规则：**所有业务相关数据，必须先定义 ValueObject（值对象）数据类**，作为数据层核心，统一管理数据结构，禁止在业务逻辑代码中直接使用零散变量存储业务数据。

- ValueObject 定义要求：
                  命名规范：采用 PascalCase，后缀建议为 Value（如 PlayerDataValue、ItemValue），明确标识数据类身份。

- 职责范围：仅包含数据字段、get/set 属性、基础数据校验（如数值范围校验），禁止包含任何业务逻辑、生命周期方法、Unity组件引用。

- 存放位置：统一放在 unity_prj/Assets/Scripts/Runtime/ValueObject 目录下（无该目录需新建），与业务逻辑代码分离。

- 示例：
                          `// 正确：ValueObject 仅负责数据存储与基础校验
public class PlayerDataValue
{
    // 数据字段（私有，通过属性暴露）
    private int _maxHp;
    private float _moveSpeed;
    
    // 基础数据校验（仅校验数据合法性，无业务逻辑）
    public int MaxHp
    {
        get => _maxHp;
        set => _maxHp = Mathf.Max(0, value); // 确保血量不小于0
    }
    
    public float MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = Mathf.Max(1f, value); // 确保移动速度合法
    }
    
    // 构造方法（用于初始化数据）
    public PlayerDataValue(int maxHp, float moveSpeed)
    {
        MaxHp = maxHp;
        MoveSpeed = moveSpeed;
    }
}
`

使用要求：业务逻辑脚本（如MonoBehaviour控制器）通过引用 ValueObject 类获取/修改数据，禁止直接定义与业务相关的零散数据字段，确保数据统一管理。

### 5.3 MVC 分层适配要求

- 模型层（Model）：以 ValueObject 为核心，补充数据持久化、数据更新逻辑（如Save/Load），禁止依赖Unity组件。

- 控制器层（Controller）：对应MonoBehaviour脚本（如PlayerController），负责调用模型层数据、处理业务逻辑、调度视图层更新，禁止直接操作视图组件或零散数据。

- 视图层（View）：仅负责UI显示、用户交互事件触发，通过事件通知控制器层处理逻辑，禁止包含业务逻辑与数据处理。

## 六、Unity 专属规范

### 5.1 组件获取规范

- 禁止频繁使用 GetComponent()，优先在 Awake() 中缓存（提升性能）

- ✅ 正确示例：
        `private Rigidbody2D _rb;
private void Awake()
{
    _rb = GetComponent<Rigidbody2D>();
}`

- ❌ 错误示例：在 Update() 中直接调用 GetComponent<Rigidbody2D>()

### 5.2 SerializeField 规范

- 私有字段需在 Inspector 可见时，使用 [SerializeField]，禁止用公有字段暴露

- 关键字段添加 [Header]、[Tooltip]，方便编辑器操作（插件可识别，不影响代码运行）

### 5.3 字符串规范

- 禁止字符串硬编码（标签、层级、动画参数、场景名等），统一用常量定义

- 示例：
        `private const string GROUND_TAG = "Ground";
private const string ANIM_JUMP = "Jump";
private const string SCENE_MAIN = "MainScene";`

### 5.4 类开发与测试规范

- 核心规则：完成一个类的开发（含逻辑实现、注释、规范校验）后，**必须进行测试并验证通过，方可进入后续编码工作**。

- 测试要求：
                  测试需覆盖类的所有公有方法、核心逻辑、边界条件及异常处理；

- 工具类需编写EditMode单元测试，MonoBehaviour类需完成PlayMode功能测试；

- 测试不通过需修改完善类逻辑，直至验证通过，禁止未测代码进入后续开发。

## 六、插件配合规范（核心适配）

- OpenSpec：开启命名、注释、格式校验，提交代码前必须无警告（避免违规代码提交）

- Oh-My-OpenCode：
        

    - 配置自动格式化：设置 Ctrl+S 保存时自动格式化代码

    - 开启生命周期排序、空行规范、缩进校正功能

- OpenCode：统一配置代码片段模板（类、方法、注释），团队共用同一模板

## 七、禁止行为（插件可检测/提示）

1. 禁止直接修改静态变量、单例成员（避免全局状态混乱）

2. 禁止在场景中挂载未使用的脚本（插件可检测冗余脚本）

3. 禁止滥用 Invoke() / Coroutine，优先使用对象池、事件机制

4. 禁止跨脚本直接获取组件，使用接口、事件解耦（提升代码可维护性）

5. 禁止未使用的变量、多余空行、多余空格（Oh-My-OpenCode 可自动清理）

6. 禁止单个方法超过50行，超过则拆分（提升可读性，插件可提示）

7. 禁止MonoBehaviour脚本无序调度，禁止跨脚本直接调用生命周期函数

8. 禁止未完成测试、未验证通过的类进入后续编码环节

9. 禁止数据与代码混淆，禁止未定义ValueObject直接使用零散变量存储业务数据

10. 禁止跨MVC层次混淆职责（如模型层包含业务逻辑、视图层处理数据）

## 八、总结

1. 命名、格式、注释严格遵循本规范，依赖 OpenSpec 校验、Oh-My-OpenCode 格式化，减少人工校验成本；

2. 脚本结构统一，使用 OpenCode 代码片段自动生成，提升开发效率；

3. 遵循 Unity 开发最佳实践，兼顾性能与可维护性，适配插件识别规则；

4. 团队成员统一遵循本规范，确保代码风格一致，降低协作成本；

5. 严格遵循代码存放规范、方法单一职责及MonoBehaviour调度要求，配合插件完成校验；

6. 执行“开发-测试-通过”的流程，确保每一个类的功能稳定可靠；

7. 遵循编码层次规范，按MVC模式实现数据与代码分层，优先定义ValueObject数据类，确保数据统一管理。
> （注：文档部分内容可能由 AI 生成）