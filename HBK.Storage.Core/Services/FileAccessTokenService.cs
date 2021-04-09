using HBK.Storage.Adapter.Storages;
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
        /// 產生存取檔案使用的權杖
        /// </summary>
        /// <param name="fileAccessToken">檔案存取權杖</param>
        /// <returns></returns>
        public async Task<FileAccessToken> GenerateFileAccessTokenAsync(FileAccessToken fileAccessToken)
        {
            if (fileAccessToken.FileAccessTokenId == default(Guid))
            {
                fileAccessToken.FileAccessTokenId = Guid.NewGuid();
            }
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, fileAccessToken.FileAccessTokenId.ToString()));
            claims.Add(new Claim("storageProviderId", fileAccessToken.StorageProviderId.ToString()));
            claims.Add(new Claim("storageGroupId", fileAccessToken.StorageGroupId.HasValue ? fileAccessToken.StorageGroupId.Value.ToString() : string.Empty));
            claims.Add(new Claim("fileEntityId", fileAccessToken.FileEntityId.ToString()));
            var userClaimsIdentity = new ClaimsIdentity(claims);
            var privateKey = new X509Certificate2(_jwtOption.X509Certificate2Location, String.IsNullOrWhiteSpace(_jwtOption.X509Certificate2Password) ? null : _jwtOption.X509Certificate2Password);
            var credentials = new SigningCredentials(new X509SecurityKey(privateKey), SecurityAlgorithms.RsaSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOption.Issuer,
                Subject = userClaimsIdentity,
                Expires = fileAccessToken.ExpireDateTime.LocalDateTime,
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);
            fileAccessToken.Token = serializeToken;
            _dbContext.FileAccessToken.Add(fileAccessToken);
            await _dbContext.SaveChangesAsync();
            return fileAccessToken;
        }

        /// <summary>
        /// 驗證存取檔案使用的權杖是否有效
        /// </summary>
        /// <param name="token">權杖</param>
        /// <returns></returns>
        public JwtSecurityToken ValidateFileAccessToken(string token)
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
        #endregion
    }
}