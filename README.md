# AutoValidation

一个基于 `ASP.NET Core` + `FluentValidation` 的自动参数校验示例项目。

项目通过自定义过滤器，在 `Controller` Action 执行前自动识别带有 `[AutoValidation]` 特性的模型，并调用对应的 `IValidator<T>` 进行校验。

---

## 项目目标

- 减少在每个 Action 中手动调用校验逻辑的重复代码
- 基于约定（模型打标记）实现自动校验
- 演示两种性能思路：
  - 反射 + `MethodInfo` 缓存（当前启用）
  - 表达式树编译委托 + 缓存（备选实现）

---

## 支持版本

- `.NET 8~.NET 10`

主要依赖（见 `AutoValidation.csproj`）：

- `FluentValidation.DependencyInjectionExtensions`

---

## 目录结构

- `Program.cs`：应用启动与 DI 注册
- `AutoValidationAttribute.cs`：自动校验标记特性
- `AutoValidationFilter.cs`：自动校验过滤器（当前启用）
- `AutoValidationWithDelegateFilter.cs`：表达式树委托版本过滤器（示例）
- `LoginRequest.cs`：请求模型 + `LoginRequestValidator`
- `Controllers/DemoController.cs`：演示接口

---

## 说明

- 当前生效的是 `AutoValidationFilter`（反射 + 缓存版）
- `AutoValidationDelegateFilter` 是另一种实现思路，可用于对比或替换
- 如果希望把校验异常统一转换为标准 HTTP 错误响应，可在项目中增加全局异常处理中间件或异常过滤器
