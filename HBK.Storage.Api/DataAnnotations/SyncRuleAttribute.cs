using HBK.Storage.Adapter.Storages;
using HBK.Storage.Core.Strategies;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.DataAnnotations
{
    /// <summary>
    /// 判斷同步規則是否符合格式
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class SyncRuleAttribute : ValidationAttribute
    {
        /// <summary>
        /// 判斷是否有效
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            if (!(value is string))
            {
                throw new ArgumentException($"{nameof(SyncRuleAttribute)} 僅能修飾 String 參數");
            }

            var query = new List<FileEntity>() {
                new FileEntity()
                {
                    FileEntityTag = new List<FileEntityTag>()
                }
            }.AsQueryable();

            try
            {
                query = query.ApplyPolicy(new Adapter.Models.SyncPolicy()
                {
                    Rule = (string)value
                });
            }
            catch (System.Linq.Dynamic.Core.Exceptions.ParseException ex)
            {
                return new ValidationResult(ex.Message);
            }

            return ValidationResult.Success;
        }
    }
}
