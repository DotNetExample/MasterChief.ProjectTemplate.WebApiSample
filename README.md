# WebApi 快速构建示例
> 1. 实现代码：[**MasterChief.DotNet.ProjectTemplate.WebApi**](https://github.com/YanZhiwei/MasterChief/tree/master/MasterChief.DotNet.ProjectTemplate.WebApi)
> 2. Nuget : [![Travis](https://img.shields.io/badge/MasterChief.DotNet.ProjectTemplate.WebApi-1.0.0.5-blue.svg)](https://www.nuget.org/packages/MasterChief.DotNet.ProjectTemplate.WebApi/)
> 3. 实现WebApi开发中诸如授权验证，缓存，参数验证，异常处理等，方便快速构建项目而无需过多关心技术细节；
> 4. 欢迎Star，欢迎PR；



目录
=================

* [授权](#%E6%8E%88%E6%9D%83)
* [鉴权](#%E9%89%B4%E6%9D%83)
* [授权与鉴权使用](#%E6%8E%88%E6%9D%83%E4%B8%8E%E9%89%B4%E6%9D%83%E4%BD%BF%E7%94%A8)
* [基于请求缓存处理](#%E5%9F%BA%E4%BA%8E%E8%AF%B7%E6%B1%82%E7%BC%93%E5%AD%98%E5%A4%84%E7%90%86)
* [异常处理](#%E5%BC%82%E5%B8%B8%E5%A4%84%E7%90%86)
* [参数验证](#%E5%8F%82%E6%95%B0%E9%AA%8C%E8%AF%81)

Created by [gh-md-toc](https://github.com/ekalinin/github-markdown-toc.go)

#### 授权

1. 授权接口，通过该接口自定义授权实现，项目默认实现基于Jwt授权

   ```c#
   /// <summary>
   ///     WebApi 授权接口
   /// </summary>
   public interface IApiAuthorize
   {
       /// <summary>
       ///     检查请求签名合法性
       /// </summary>
       /// <param name="signature">加密签名字符串</param>
       /// <param name="timestamp">时间戳</param>
       /// <param name="nonce">随机数</param>
       /// <param name="appConfig">应用接入配置信息</param>
       /// <returns>CheckResult</returns>
       CheckResult CheckRequestSignature(string signature, string timestamp, string nonce, AppConfig appConfig);
    
    
       /// <summary>
       ///     创建合法用户获取访问令牌接口数据
       /// </summary>
       /// <param name="identityUser">IdentityUser</param>
       /// <param name="appConfig">AppConfig</param>
       /// <returns>IdentityToken</returns>
       ApiResult<IdentityToken> CreateIdentityToken(IdentityUser identityUser, AppConfig appConfig);
   }
   ```

2. 基于Jwt授权实现

   ```c#
   /// <summary>
   ///     基于Jwt 授权实现
   /// </summary>
   public sealed class JwtApiAuthorize : IApiAuthorize
   {
       /// <summary>
       ///     检查请求签名合法性
       /// </summary>
       /// <param name="signature">加密签名字符串</param>
       /// <param name="timestamp">时间戳</param>
       /// <param name="nonce">随机数</param>
       /// <param name="appConfig">应用接入配置信息</param>
       /// <returns>CheckResult</returns>
       public CheckResult CheckRequestSignature(string signature, string timestamp, string nonce, AppConfig appConfig)
       {
           ValidateOperator.Begin()
               .NotNullOrEmpty(signature, "加密签名字符串")
               .NotNullOrEmpty(timestamp, "时间戳")
               .NotNullOrEmpty(nonce, "随机数")
               .NotNull(appConfig, "AppConfig");
           var appSecret = appConfig.AppSecret;
           var signatureExpired = appConfig.SignatureExpiredMinutes;
           string[] data = {appSecret, timestamp, nonce};
           Array.Sort(data);
           var signatureText = string.Join("", data);
           signatureText = Md5Encryptor.Encrypt(signatureText);
    
           if (!signature.CompareIgnoreCase(signatureText) && CheckHelper.IsNumber(timestamp))
               return CheckResult.Success();
           var timestampMillis =
               UnixEpochHelper.DateTimeFromUnixTimestampMillis(timestamp.ToDoubleOrDefault());
           var minutes = DateTime.UtcNow.Subtract(timestampMillis).TotalMinutes;
    
           return minutes > signatureExpired ? CheckResult.Fail("签名时间戳失效") : CheckResult.Success();
       }
    
       /// <summary>
       ///     创建合法用户获取访问令牌接口数据
       /// </summary>
       /// <param name="identityUser">IdentityUser</param>
       /// <param name="appConfig">AppConfig</param>
       /// <returns>IdentityToken</returns>
       public ApiResult<IdentityToken> CreateIdentityToken(IdentityUser identityUser, AppConfig appConfig)
       {
           ValidateOperator.Begin()
               .NotNull(identityUser, "IdentityUser")
               .NotNull(appConfig, "AppConfig");
           var payload = new Dictionary<string, object>
           {
               {"iss", identityUser.UserId},
               {"iat", UnixEpochHelper.GetCurrentUnixTimestamp().TotalSeconds}
           };
           var identityToken = new IdentityToken
           {
               AccessToken = CreateIdentityToken(appConfig.SharedKey, payload),
               ExpiresIn = appConfig.TokenExpiredDay * 24 * 3600
           };
           return ApiResult<IdentityToken>.Success(identityToken);
       }
    
       /// <summary>
       ///     创建Token
       /// </summary>
       /// <param name="secret">密钥</param>
       /// <param name="payload">负载数据</param>
       /// <returns>Token令牌</returns>
       public static string CreateIdentityToken(string secret, Dictionary<string, object> payload)
       {
           ValidateOperator.Begin().NotNull(payload, "负载数据").NotNullOrEmpty(secret, "密钥");
           IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
           IJsonSerializer serializer = new JsonNetSerializer();
           IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
           IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
           return encoder.Encode(payload, secret);
       }
   }
   ```

#### 鉴权

1. Token令牌鉴定接口，通过该接口可以自定义扩展实现方式，项目默认实现基于Jwt鉴权

   ```c#
   /// <summary>
   ///     webApi 验证系统基本接口
   /// </summary>
   public interface IApiAuthenticate
   {
       #region Methods
    
       /// <summary>
       ///     验证Token令牌是否合法
       /// </summary>
       /// <param name="token">令牌</param>
       /// <param name="appConfig">AppConfig</param>
       /// <returns>CheckResult</returns>
       ApiResult<string> CheckIdentityToken(string token, AppConfig appConfig);
    
       #endregion Methods
   }
   ```

2. 基于Jwt鉴权实现

   ```c#
   /// <summary>
   ///     基于Jwt 授权验证实现
   /// </summary>
   public sealed class JwtApiAuthenticate : IApiAuthenticate
   {
       /// <summary>
       ///     检查Token是否合法
       /// </summary>
       /// <param name="token">用户令牌</param>
       /// <param name="appConfig">AppConfig</param>
       /// <returns></returns>
       public ApiResult<string> CheckIdentityToken(string token, AppConfig appConfig)
       {
           ValidateOperator.Begin()
               .NotNullOrEmpty(token, "Token")
               .NotNull(appConfig, "AppConfig");
           try
           {
               var tokenText = ParseTokens(token, appConfig.SharedKey);
               if (string.IsNullOrEmpty(tokenText))
                   return ApiResult<string>.Fail("用户令牌Token为空");
    
               dynamic root = JObject.Parse(tokenText);
               string userid = root.iss;
               double iat = root.iat;
               var validTokenExpired =
                   new TimeSpan((int) (UnixEpochHelper.GetCurrentUnixTimestamp().TotalSeconds - iat))
                       .TotalDays > appConfig.TokenExpiredDay;
               return validTokenExpired
                   ? ApiResult<string>.Fail($"用户ID{userid}令牌失效")
                   : ApiResult<string>.Success(userid);
           }
           catch (FormatException)
           {
               return ApiResult<string>.Fail("用户令牌非法");
           }
           catch (SignatureVerificationException)
           {
               return ApiResult<string>.Fail("用户令牌非法");
           }
       }
    
       /// <summary>
       ///     转换Token
       /// </summary>
       /// <param name="token">令牌</param>
       /// <param name="secret">密钥</param>
       /// <returns>Token以及负载数据</returns>
       private string ParseTokens(string token, string secret)
       {
           ValidateOperator.Begin()
               .NotNullOrEmpty(token, "令牌")
               .NotNullOrEmpty(secret, "密钥");
    
           IJsonSerializer serializer = new JsonNetSerializer();
           IDateTimeProvider provider = new UtcDateTimeProvider();
           IJwtValidator validator = new JwtValidator(serializer, provider);
           IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
           IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
           return decoder.Decode(token, secret, true);
       }
   }
   ```

#### 授权与鉴权使用

1. 授权使用，通过Controller构造函数方式，代码如下

   ```c#
   /// <summary>
   ///     Api授权
   /// </summary>
   public abstract class AuthorizeController : ApiBaseController
   {
       #region Constructors
    
       /// <summary>
       ///     构造函数
       /// </summary>
       /// <param name="apiAuthorize">IApiAuthorize</param>
       /// <param name="appCfgService">IAppConfigService</param>
       protected AuthorizeController(IApiAuthorize apiAuthorize, IAppConfigService appCfgService)
       {
           ValidateOperator.Begin()
               .NotNull(apiAuthorize, "IApiAuthorize")
               .NotNull(appCfgService, "IAppConfigService");
           ApiAuthorize = apiAuthorize;
           AppCfgService = appCfgService;
       }
    
       #endregion Constructors
    
       #region Fields
    
       /// <summary>
       ///     授权接口
       /// </summary>
       protected readonly IApiAuthorize ApiAuthorize;
    
       /// <summary>
       ///     请求通道配置信息，可以从文件或者数据库获取
       /// </summary>
       protected readonly IAppConfigService AppCfgService;
    
       #endregion Fields
    
       #region Methods
    
       /// <summary>
       ///     创建合法用户的Token
       /// </summary>
       /// <param name="userId">用户Id</param>
       /// <param name="passWord">用户密码</param>
       /// <param name="signature">加密签名字符串</param>
       /// <param name="timestamp">时间戳</param>
       /// <param name="nonce">随机数</param>
       /// <param name="appid">应用接入ID</param>
       /// <returns>OperatedResult</returns>
       protected virtual ApiResult<IdentityToken> CreateIdentityToken(string userId, string passWord,
           string signature, string timestamp,
           string nonce, Guid appid)
       {
           #region  参数检查
    
           var checkResult = CheckRequest(userId, passWord, signature, timestamp, nonce, appid);
    
           if (!checkResult.State)
               return ApiResult<IdentityToken>.Fail(checkResult.Message);
    
           #endregion
    
           #region 用户鉴权
    
           var getIdentityUser = GetIdentityUser(userId, passWord);
    
           if (!getIdentityUser.State) return ApiResult<IdentityToken>.Fail(getIdentityUser.Message);
    
           #endregion
    
           #region 请求通道检查
    
           var getAppConfig = AppCfgService.Get(appid);
    
           if (!getAppConfig.State) return ApiResult<IdentityToken>.Fail(getAppConfig.Message);
           var appConfig = getAppConfig.Data;
    
           #endregion
    
           #region 检查请求签名检查
    
           var checkSignatureResult = ApiAuthorize.CheckRequestSignature(signature, timestamp, nonce, appConfig);
           if (!checkSignatureResult.State) return ApiResult<IdentityToken>.Fail(checkSignatureResult.Message);
    
           #endregion
    
           #region 生成基于Jwt Token
    
           var getTokenResult = ApiAuthorize.CreateIdentityToken(getIdentityUser.Data, getAppConfig.Data);
           if (!getTokenResult.State) return ApiResult<IdentityToken>.Fail(getTokenResult.Message);
    
           return ApiResult<IdentityToken>.Success(getTokenResult.Data);
    
           #endregion
       }
    
    
       /// <summary>
       ///     检查用户的合法性
       /// </summary>
       /// <param name="userId">用户Id</param>
       /// <param name="passWord">用户密码</param>
       /// <returns>UserInfo</returns>
       protected abstract CheckResult<IdentityUser> GetIdentityUser(string userId, string passWord);
    
       private CheckResult CheckRequest(string userId, string passWord, string signature, string timestamp,
           string nonce, Guid appid)
       {
           if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(passWord))
               return CheckResult.Fail("用户名或密码为空");
    
           if (string.IsNullOrEmpty(signature))
               return CheckResult.Fail("请求签名为空");
    
           if (string.IsNullOrEmpty(timestamp))
               return CheckResult.Fail("时间戳为空");
    
           if (string.IsNullOrEmpty(nonce))
               return CheckResult.Fail("随机数为空");
    
           if (appid == Guid.Empty)
               return CheckResult.Fail("应用接入ID非法");
    
           return CheckResult.Success();
       }
    
       #endregion Methods
   }
   ```

2. 鉴权使用，通过AuthorizationFilterAttribute形式，标注请求是否需要鉴权

   ```c#
   /// <summary>
    ///     WebApi 授权验证实现
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class AuthenticateAttribute : AuthorizationFilterAttribute
    {
        #region Constructors
       
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="apiAuthenticate">IApiAuthenticate</param>
        /// <param name="appCfgService">appCfgService</param>
        protected AuthenticateAttribute(IApiAuthenticate apiAuthenticate, IAppConfigService appCfgService)
        {
            ValidateOperator.Begin()
                .NotNull(apiAuthenticate, "IApiAuthenticate")
                .NotNull(appCfgService, "IAppConfigService");
            ApiAuthenticate = apiAuthenticate;
            AppCfgService = appCfgService;
        }
    
        #endregion Constructors
    
        #region Fields
    
        /// <summary>
        ///     授权验证接口
        /// </summary>
        protected readonly IApiAuthenticate ApiAuthenticate;
    
        /// <summary>
        ///     请求通道配置信息，可以从文件或者数据库获取
        /// </summary>
        protected readonly IAppConfigService AppCfgService;
    
        #endregion Fields
    
        #region Methods
    
        /// <summary>
        ///     验证Token令牌是否合法
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="appid">应用ID</param>
        /// <returns>CheckResult</returns>
        protected virtual ApiResult<string> CheckIdentityToken(string token, Guid appid)
        {
            #region 请求参数检查
    
            var checkResult = CheckRequest(token, appid);
    
            if (!checkResult.State)
                return ApiResult<string>.Fail(checkResult.Message);
    
            #endregion
    
            #region 请求通道检查
    
            var getAppConfig = AppCfgService.Get(appid);
    
            if (!getAppConfig.State) return ApiResult<string>.Fail(getAppConfig.Message);
            var appConfig = getAppConfig.Data;
    
            #endregion
    
            return ApiAuthenticate.CheckIdentityToken(token, appConfig);
        }
    
        private CheckResult CheckRequest(string token, Guid appid)
        {
            if (string.IsNullOrEmpty(token))
                return CheckResult.Fail("用户令牌为空");
            return Guid.Empty == appid ? CheckResult.Fail("应用ID非法") : CheckResult.Success();
        }
    
        #endregion Methods
    }
   ```

#### 基于请求缓存处理

1. 通过ICacheProvider接口，可以扩展缓存数据方式；

2. 通过配置DependsOnIdentity参数，可以配置是否依赖Token令牌进行缓存；

3. 通过配置CacheMinutes参数，可以指定具体接口缓存时间，当设置0的时候不启用缓存；

4. 通过实现ControllerCacheAttribute，可以在不同项目快速达到接口缓存功能；

   ```c#
   public class RequestCacheAttribute : ControllerCacheAttribute
   {
       public RequestCacheAttribute(int cacheMinutes) : this(cacheMinutes, true, new LocalCacheProvider())
       {
       }
    
       public RequestCacheAttribute(int cacheMinutes, bool dependsOnIdentity, ICacheProvider cacheProvider) : base(
           cacheMinutes, dependsOnIdentity, cacheProvider)
       {
       }
    
       protected override bool CheckedResponseAvailable(HttpActionContext context, string responseText)
       {
           return !string.IsNullOrEmpty(responseText) && context != null;
       }
    
       protected override string GetIdentityToken(HttpActionContext actionContext)
       {
           return actionContext.Request.GetUriOrHeaderValue("Access_token").ToStringOrDefault(string.Empty);
       }
   }
   ```

#### 异常处理

1. 通过实现ControllerExceptionAttribute，可以轻松简单构建接口请求时候异常发生，并通过HttpRequestRaw requestRaw参数，可以获取非常详尽的请求信息；

   ```c#
   public sealed class ExceptionLogAttribute : ControllerExceptionAttribute
   {
       public override void OnActionExceptioning(HttpActionExecutedContext actionExecutedContext, string actionName,
           HttpStatusCode statusCode,
           HttpRequestRaw requestRaw)
       {
           var response = new HttpResponseMessage
           {
               Content = new StringContent("发生故障，请稍后重试！"),
               StatusCode = statusCode
           };
           actionExecutedContext.Response = response;
       }
   }
   ```

#### 参数验证

1. 通过实现ValidateModelAttribute，以及DataAnnotations快速构建请求参数验证

2. 请求参数只需要DataAnnotations标注即可；

   ```c#
   public sealed class ArticleRequest
   {
       [Required(ErrorMessage = "缺少文章ID")]
       public int Id
       {
           get;
           set;
       }
    
   }
   ```

3. 项目实现ValidateModelAttribute，可以自定义构建参数处理方式

   ```c#
   /// <summary>
   /// 请求参数
   /// </summary>
   public sealed class ValidateRequestAttribute : ValidateModelAttribute
   {
       public override void OnParameterIsNulling(HttpActionContext actionContext)
       {
           actionContext.Response =
               actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, OperatedResult<string>.Fail("请求参数非法。"));
       }
    
       public override void OnParameterInvaliding(HttpActionContext actionContext, ValidationFailedResult result)
       {
           var message = result.Data.FirstOrDefault()?.Message;
           actionContext.Response =
               actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, OperatedResult<string>.Fail(message));
       }
   }
   ```