PYunAPI SDK C#
---



### 版本更新

##### [v1.2.0.2] - 2022/04/29

- 支持输出认证请求并优化打包。

##### [v1.2.0.0] - 2020/04/18

- 支持拦截请求用于自定义解析, 重写`AbstractServiceAPI->HookRequest`方法。

##### [v1.1.0.6] - 2019/10/04

- 支持设置厂商信息;
- 支持手动/自动设置当前主机名称及IP。

##### [v1.1.0.4] - 2019/06/08

- 修复极端情况无法重链的问题。



### 开发环境

| 属性    | 值        |
|:----- |:-------- |
| IDE   | VS2010   |
| 平台工具集 | .NET 2.0 |

### 文件清单

* 4pyun-api.dll

## 用法

### 1. 实现AbstractServiceAPI

在抽象类`AbstractServiceAPI`中定义类目前所有接口中定义方法的回调函数, 具体接口参数可参考接口文档定义。

### 2. 实现ChannelEventHandler

当底层TCP连接建立、授权结果反馈时该事件会被触发, 事件类型有:

| 项定义                              | 说明      |
| -------------------------------- | ------- |
| `ChannelEventType.AccessGranted` | API授权成功 |
| `ChannelEventType.AccessDenied`  | API授权失败 |
| `ChannelEventType.ChannelError`  | TCP连接异常 |
| `ChannelEventType.ChannelClosed` | TCP链接关闭 |

### 3. 创建PYunAPI对象

```csharp
// PYun服务器地址, 根据开通参数设置
string host = "sandbox.gate.4pyun.com";
// PYun服务器端口, 根据开通参数设置
int port = 8661;

PYunAPI instance = new PYunAPI(host, port);
// 设置终端类型
instance.Type = "public:parking:agent";
// 设置通信事件处理回调处理类
instance.ChannelEventHandler += new EventHandler<ChannelEventArgs>(ChannelEventHandler);
// 设置业务接口处理回调类
instance.ServiceHandler = new PYunServiceAPIImpl();
```

### 4. 启动服务

```csharp
// PYun分配当客户端ID, 根据开通参数设置
string uuid = "foo";
// PYun分配当客户端通信密钥, 根据开通参数设置
string mac = "mac";
// 启动服务
instance.Startup(uuid, mac);
```

通过调用`Startup`方法底层SDK会开启线程和PYun服务器保持一个TCP长连接, 并支持会自动重连; 连接的状态通过`ChannelEventHandler`中监听处理。

### 5. 停止服务

```csharp
instance.Shutdown();
```

通过调用`Shutdown`方法会关闭底层TCP连接线程。

### 6. 重启服务

```
instance.Restart();
```

通过调用`Restart`方法会关闭底层TCP连接线程, 然后重新建立连接, 连接的状态通过`ChannelEventHandler`中监听处理。

### 附录

#### A. 多终端设置

默认P云一个项目只允许保持一个有效TCP连接用于数据通信, 为兼容部分系统厂商暂时提供多终端的解决方案, 该方案允许各个终端设置自己的`device`用于区分不同的终端。

在停车场景, `device`需设置为当前通道口ID, 设置方法如下:

```csharp
// [可选]设置设备ID, 仅在多终端模式下设置
instance.Device = "98K";
```
