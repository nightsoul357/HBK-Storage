using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.Enums;
using HBK.Storage.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Services
{
    /// <summary>
    /// 檔案存取權杖服務
    /// </summary>
    public class FileAccessTokenService
    {
        private readonly HBKStorageContext _dbContext;
        private readonly JWTOption _jwtOption;


        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="dbContext">資料庫實體</param>
        /// <param name="jwtOption">JWT 設定檔</param>
        public FileAccessTokenService(HBKStorageContext dbContext, JWTOption jwtOption)
        {
            _dbContext = dbContext;
            _jwtOption = jwtOption;
        }

        #region DAL
        /// <summary>
        /// 取得檔案存取權杖列表
        /// </summary>
        /// <returns></returns>
        public IQueryable<FileAccessToken> ListQuery()
        {
            return _dbContext.FileAccessToken;
        }
        /// <summary>
        /// 以 ID 取得檔案存取權杖
        /// </summary>
        /// <param name="fileAccessTokenId">檔案存取權杖 ID</param>
        /// <returns></returns>
        public Task<FileAccessToken> FindByIdAsync(Guid fileAccessTokenId)
        {
            return this.ListQuery()
                .FirstOrDefaultAsync(x => x.FileAccessTokenId == fileAccessTokenId);
        }
        #endregion
        #region BAL
        /// <summary>
        /// 撤銷檔案存取權杖
        /// </summary>
        /// <param name="fileAccessTokenId">檔案存取權杖 ID</param>
        /// <returns></returns>
        public async Task<FileAccessToken> RevokeTokenAsync(Guid fileAccessTokenId)
        {
            var fileAccessToken = await this.FindByIdAsync(fileAccessTokenId);
            fileAccessToken.Status = fileAccessToken.Status | FileAccessTokenStatusEnum.Revoke;
            await _dbContext.SaveChangesAsync();
            return await this.FindByIdAsync(fileAccessTokenId);
        }
        /// <summary>
        /// 產生檔案存取權杖模型
        /// </summary>
        /// <param name="jwtSecurityToken"></param>
        /// <returns></returns>
        public FileAccessToken BuildFileAccessToken(JwtSecurityToken jwtSecurityToken)
        {
            FileAccessToken result = new FileAccessToken();
            result.FileAccessTokenId = Guid.Parse(jwtSecurityToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Jti).Value);
            result.StorageProviderId = Guid.Parse(jwtSecurityToken.Claims.First(x => x.Type == "storageProviderId").Value);
            result.StorageGroupId = string.IsNullOrWhiteSpace(jwtSecurityToken.Claims.First(x => x.Type == "storageGroupId").Value) ? null : Guid.Parse(jwtSecurityToken.Claims.First(x => x.Type == "storageGroupId").Value);
            var c = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "fileEntityId");
            if (c != null)
            {
                result.FileEntityId = Guid.Parse(c.Value);
            }
            return result;
        }
        /// <summary>
        /// 產生一般存取檔案使用的權杖
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="strogaeGroupId">強制指定的存取對象的儲存個體集合 ID</param>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="expireDateTime">過期時間</param>
        /// <param name="accessTimeLimit">存取次數限制</param>
        /// <returns></returns>
        public async Task<FileAccessToken> GenerateNormalFileAccessTokenAsync(Guid storageProviderId, Guid? strogaeGroupId, Guid fileEntityId, DateTime expireDateTime, int accessTimeLimit)
        {
            FileAccessToken fileAccessToken = new FileAccessToken()
            {
                AccessTimesLimit = accessTimeLimit,
                StorageProviderId = storageProviderId,
                StorageGroupId = strogaeGroupId,
                FileEntityId = fileEntityId,
                FileAccessTokenId = Guid.NewGuid(),
                ExpireDateTime = expireDateTime
            };
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, fileAccessToken.FileAccessTokenId.ToString()));
            claims.Add(new Claim("storageProviderId", fileAccessToken.StorageProviderId.ToString()));
            claims.Add(new Claim("storageGroupId", fileAccessToken.StorageGroupId.HasValue ? fileAccessToken.StorageGroupId.Value.ToString() : string.Empty));
            claims.Add(new Claim("fileEntityId", fileAccessToken.FileEntityId.ToString()));
            claims.Add(new Claim("tokenType", FileAccessTokenTypeEnum.Normal.ToString()));
            fileAccessToken.Token = this.GenerateJWTToken(claims, fileAccessToken.ExpireDateTime.LocalDateTime);
            _dbContext.FileAccessToken.Add(fileAccessToken);
            await _dbContext.SaveChangesAsync();
            return fileAccessToken;
        }
        /// <summary>
        /// 產生無限制的一般存取檔案使用的權杖
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="strogaeGroupId">強制指定的存取對象的儲存個體集合 ID</param>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="expireDateTime">過期時間</param>
        /// <returns></returns>
        public string GenerateNormalNoLimitFileAccessToken(Guid storageProviderId, Guid? strogaeGroupId, Guid fileEntityId, DateTime expireDateTime)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim("storageProviderId", storageProviderId.ToString()));
            claims.Add(new Claim("storageGroupId", strogaeGroupId.HasValue ? strogaeGroupId.Value.ToString() : string.Empty));
            claims.Add(new Claim("fileEntityId", fileEntityId.ToString()));
            claims.Add(new Claim("tokenType", FileAccessTokenTypeEnum.NormalNoLimit.ToString()));
            var token = this.GenerateJWTToken(claims, expireDateTime);
            return token;
        }
        /// <summary>
        /// 產生允許指定 Tag 通過的檔案存取權杖
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="strogaeGroupId">強制指定的存取對象的儲存個體集合 ID</param>
        /// <param name="allowTagPattern"></param>
        /// <param name="expireDateTime">過期時間</param>
        /// <param name="accessTimeLimit">存取次數限制</param>
        /// <returns></returns>
        public async Task<FileAccessToken> GenerateAllowTagFileAccessTokenAsync(Guid storageProviderId, Guid? strogaeGroupId, string allowTagPattern, DateTime expireDateTime, int accessTimeLimit)
        {
            FileAccessToken fileAccessToken = new FileAccessToken()
            {
                AccessTimesLimit = accessTimeLimit,
                StorageProviderId = storageProviderId,
                StorageGroupId = strogaeGroupId,
                FileAccessTokenId = Guid.NewGuid(),
                ExpireDateTime = expireDateTime
            };

            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, fileAccessToken.FileAccessTokenId.ToString()));
            claims.Add(new Claim("storageProviderId", storageProviderId.ToString()));
            claims.Add(new Claim("storageGroupId", strogaeGroupId.HasValue ? strogaeGroupId.Value.ToString() : string.Empty));
            claims.Add(new Claim("allowTagPattern", allowTagPattern));
            claims.Add(new Claim("tokenType", FileAccessTokenTypeEnum.AllowTag.ToString()));
            fileAccessToken.Token = this.GenerateJWTToken(claims, fileAccessToken.ExpireDateTime.LocalDateTime);
            _dbContext.FileAccessToken.Add(fileAccessToken);
            await _dbContext.SaveChangesAsync();
            return fileAccessToken;
        }
        /// <summary>
        /// 產生無限制的允許指定 Tag 通過的檔案存取權杖
        /// </summary>
        /// <param name="storageProviderId">儲存服務 ID</param>
        /// <param name="strogaeGroupId">強制指定的存取對象的儲存個體集合 ID</param>
        /// <param name="allowTagPattern"></param>
        /// <param name="expireDateTime">過期時間</param>
        /// <returns></returns>
        public string GenerateAllowTagNoLimitFileAccessToken(Guid storageProviderId, Guid? strogaeGroupId, string allowTagPattern, DateTime expireDateTime)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim("storageProviderId", storageProviderId.ToString()));
            claims.Add(new Claim("storageGroupId", strogaeGroupId.HasValue ? strogaeGroupId.Value.ToString() : string.Empty));
            claims.Add(new Claim("allowTagPattern", allowTagPattern));
            claims.Add(new Claim("tokenType", FileAccessTokenTypeEnum.AllowTagNoLimit.ToString()));
            var token = this.GenerateJWTToken(claims, expireDateTime);
            return token;
        }
        /// <summary>
        /// 驗證存取檔案使用的權杖是否有效
        /// </summary>
        /// <param name="token">權杖</param>
        /// <returns></returns>
        public async Task<JwtSecurityToken> ValidateFileAccessTokenAsync(string token)
        {
            var privateKey = new X509Certificate2(_jwtOption.X509Certificate2Location, String.IsNullOrWhiteSpace(_jwtOption.X509Certificate2Password) ? null : _jwtOption.X509Certificate2Password);
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new X509SecurityKey(privateKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            JwtSecurityToken jwtSecurityToken = (JwtSecurityToken)validatedToken;
            var fileAccessToken = this.BuildFileAccessToken(jwtSecurityToken);
            FileAccessTokenTypeEnum tokenType = Enum.Parse<FileAccessTokenTypeEnum>(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "tokenType").Value);

            if (tokenType == FileAccessTokenTypeEnum.AllowTag || tokenType == FileAccessTokenTypeEnum.Normal)
            {
                var currentAccessToken = await this.FindByIdAsync(fileAccessToken.FileAccessTokenId);
                if (currentAccessToken.Status.HasFlag(FileAccessTokenStatusEnum.Revoke))
                {
                    throw new ArgumentException("權杖以撤銷");
                }
                if (currentAccessToken.AccessTimesLimit <= currentAccessToken.AccessTimes)
                {
                    throw new ArgumentException("超過存取次數上限");
                }
            }

            return (JwtSecurityToken)validatedToken;
        }
        /// <summary>
        /// 嘗試增加檔案存取次數
        /// </summary>
        /// <param name="fileAccessTokenId">檔案存取權杖 ID</param>
        /// <returns></returns>
        public async Task TryAddAccessTimesAsync(Guid fileAccessTokenId)
        {
            using (var transcation = await _dbContext.Database.BeginTransactionAsync())
            {
                var fileAccessToken = await _dbContext.FileAccessToken.FirstOrDefaultAsync(x => x.FileAccessTokenId == fileAccessTokenId);
                if (fileAccessToken.AccessTimesLimit <= fileAccessToken.AccessTimes)
                {
                    await transcation.RollbackAsync();
                    throw new InvalidOperationException("Over acess times limit");
                }

                fileAccessToken.AccessTimes++;
                await _dbContext.SaveChangesAsync();
                await transcation.CommitAsync();
            }
        }

        /// <summary>
        /// 產生 JWT Token
        /// </summary>
        /// <param name="claims">宣告式內容</param>
        /// <param name="expireDateTime">過期時間</param>
        /// <returns></returns>
        private string GenerateJWTToken(List<Claim> claims, DateTime expireDateTime)
        {
            var userClaimsIdentity = new ClaimsIdentity(claims);
            var privateKey = new X509Certificate2(_jwtOption.X509Certificate2Location, String.IsNullOrWhiteSpace(_jwtOption.X509Certificate2Password) ? null : _jwtOption.X509Certificate2Password);
            var credentials = new SigningCredentials(new X509SecurityKey(privateKey), SecurityAlgorithms.RsaSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOption.Issuer,
                Subject = userClaimsIdentity,
                Expires = expireDateTime,
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);
            return serializeToken;
        }
        #endregion
    }
}