using System.Security.Cryptography;
using System.Text;
using POS;

public class Encryption
{
    public Encryption()
    {
    }

    //public async Task Invoke(HttpContext httpContext)
    //{
    //    string decryptedString = "";
    //    httpContext.Response.Body = EncryptStream(httpContext.Response.Body, _configuration);
    //    httpContext.Request.Body = DecryptStream(httpContext.Request.Body, _configuration);
    //    var isPostOrGet = httpContext.Request.Method;

    //    if (httpContext.Request.QueryString.HasValue)
    //    {
    //        decryptedString = DecryptString(httpContext.Request.QueryString.Value.Substring(1), _configuration);
    //        httpContext.Request.QueryString = new QueryString($"?{decryptedString.Trim()}");
    //    }

    //    var clientId = httpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
    //    var handler = new JwtSecurityTokenHandler();
    //    if (!string.IsNullOrWhiteSpace(clientId))
    //    {
    //        foreach (var header in httpContext.Request.Headers)
    //        {
    //            _logger.LogInformation(header.Key + ": " + header.Value, DateTime.UtcNow);
    //        }
    //        var jsonToken = handler.ReadToken(clientId);
    //        var tokenS = jsonToken as JwtSecurityToken;
    //        if (tokenS != null)
    //        {
    //            var jti = tokenS.Claims.First(claim => claim.Type == "jti").Value;
    //            _logger.LogInformation("User_Name:" + tokenS.Claims.First(claim => claim.Type == "userId").Value + " Host:" + httpContext.Request.Host + " Body:"
    //                + httpContext.Request.Path + " QueryString:" + httpContext.Request.QueryString, DateTime.UtcNow);
    //        }
    //        var bodyAsText = "";
    //        if (isPostOrGet != HttpMethod.Get.ToString())
    //        {
    //             bodyAsText = await new System.IO.StreamReader(httpContext.Request.Body).ReadToEndAsync();
    //        }
    //        var QSFirstLevel = new List<string>();
    //        var accountId = "accountId=" + tokenS.Claims.First(claim => claim.Type == "accountId").Value;
    //        var branchId = tokenS.Claims.First(claim => claim.Type == "branchId").Value.Split(',').ToList().Select(s => ("branchId=" + s).ToLower()).ToList();
    //        var queryString = httpContext.Request.QueryString.ToString().Replace("?", "").Split('&').ToList();
    //        queryString.ForEach(fe => { if (fe != "") QSFirstLevel.AddRange(fe.Split('=').ToList()); });
    //        if (bodyAsText != "")
    //        {
    //            bodyAsText.Replace('"', ' ').Trim().Replace(" ", "").Replace(":", "=").Split('{').ToList().ForEach(fe=> {
    //                queryString.AddRange(fe.Split(',').ToList());
    //            });
    //            var myBody = bodyAsText.Replace("{", "").Replace("}", "").Replace(":", "=")
    //                 .Replace('"', ' ').Trim().Replace(" ", "").Split(',').ToList();
    //            myBody.ForEach(fe => { if (fe != "") QSFirstLevel.AddRange(fe.Split('=').ToList()); });

    //        }
    //        if (QSFirstLevel.Any(a => "accountId".ToLower() == a.ToLower()) && QSFirstLevel.Any(a => "branchId".ToLower() == a.ToLower()))
    //        {
    //            if (!queryString.Any(a => accountId.ToLower().Contains(a.Trim().ToLower())) || !queryString.Any(a => branchId.Contains(a.Trim().ToLower())))
    //            {
    //                _logger.LogInformation("Invalid_Account_Branch_User_Name:" + tokenS.Claims.First(claim => claim.Type == "userId").Value + " Host:" + httpContext.Request.Host + " Body:"
    //                 + httpContext.Request.Path + " QueryString:" + httpContext.Request.QueryString, DateTime.UtcNow);
    //                httpContext.Response.StatusCode = 404; // i.e. 400 works
    //                httpContext.Response.ContentType = "application/json";
    //                await httpContext.Response.WriteAsync(new ErrorDetails()
    //                {
    //                    statuscode = httpContext.Response.StatusCode,
    //                    message = "Invalid AccountId and BranchId."
    //                }.ToString());
    //            }
    //        }
    //        else
    //        {
    //            _logger.LogInformation("Missing_Account_Branch_User_Name:" + tokenS.Claims.First(claim => claim.Type == "userId").Value + " Host:" + httpContext.Request.Host + " Body:"
    //               + httpContext.Request.Path + " QueryString:" + httpContext.Request.QueryString, DateTime.UtcNow);
    //        }
            
    //        if (isPostOrGet != HttpMethod.Get.ToString())
    //        {
    //            var requestData = Encoding.UTF8.GetBytes(bodyAsText);
    //            var stream = new MemoryStream(requestData); ;
    //            httpContext.Request.Body = stream;
    //        }



    //    }

      
    //    await _next(httpContext);
    //    await httpContext.Request.Body.DisposeAsync();
    //    await httpContext.Response.Body.DisposeAsync();
    //}

    public static CryptoStream EncryptStream(Stream responseStream)
    {
        Aes aes = GetEncryptionAlgorithm();

        ToBase64Transform base64Transform = new ToBase64Transform();
        CryptoStream base64EncodedStream = new CryptoStream(responseStream, base64Transform, CryptoStreamMode.Write);
        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        CryptoStream cryptoStream = new CryptoStream(base64EncodedStream, encryptor, CryptoStreamMode.Write);

        return cryptoStream;
    }

    public static Stream DecryptStream(Stream cipherStream)
    {
        Aes aes = GetEncryptionAlgorithm();

        FromBase64Transform base64Transform = new FromBase64Transform(FromBase64TransformMode.IgnoreWhiteSpaces);
        CryptoStream base64DecodedStream = new CryptoStream(cipherStream, base64Transform, CryptoStreamMode.Read);
        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        CryptoStream decryptedStream = new CryptoStream(base64DecodedStream, decryptor, CryptoStreamMode.Read);
        return decryptedStream;
    }

    public static string DecryptString(string cipherText)
    {
        Aes aes = GetEncryptionAlgorithm();
        byte[] buffer = Convert.FromBase64String(cipherText);

        using MemoryStream memoryStream = new MemoryStream(buffer);
        using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using StreamReader streamReader = new StreamReader(cryptoStream);
        return streamReader.ReadToEnd();
    }

    public static string EncryptString(string plainText)
    {
        Aes aes = GetEncryptionAlgorithm();
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        using MemoryStream memoryStream = new MemoryStream();
        using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        cryptoStream.FlushFinalBlock();

        byte[] cipherTextBytes = memoryStream.ToArray();
        return Convert.ToBase64String(cipherTextBytes);
    }

    private static Aes GetEncryptionAlgorithm()
    {
        var secret_key = AppSettings.AppSecret;
        var initialization_vector = AppSettings.AppSecret;

        Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(secret_key);
        aes.IV = Encoding.UTF8.GetBytes(initialization_vector);

        return aes;
    }

}