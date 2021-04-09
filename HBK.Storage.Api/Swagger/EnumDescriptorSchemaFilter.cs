using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HBK.Storage.Api.Swagger
{
    /// <summary>
    /// Add enum description to swagger gen files
    /// </summary>
    /// <remarks>https://github.com/Justin-Lessard/swagger-gen-enums</remarks>
    public class EnumDescriptorSchemaFilter : ISchemaFilter
    {
        #region Public Methods

        /// <summary>
        /// Apply filter
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var typeInfo = context.Type.GetTypeInfo();

            if (typeInfo.IsEnum)
            {
                schema.Description = this.BuildDescription(typeInfo);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private string BuildDescription(TypeInfo typeInfo)
        {
            var docMembers = this.LoadXmlMembers(typeInfo);

            var stringBuilder = new StringBuilder();

            var docMember = this.GetTypeMember(docMembers, typeInfo);
            if (typeInfo.GetCustomAttribute<FlagsAttribute>() != null)
            {
                stringBuilder.Append("[Flags] ");
            }
            stringBuilder.AppendLine(docMember.Value.Trim());
            stringBuilder.AppendLine();

            this.BuildMembersDescription(typeInfo, stringBuilder, docMembers);

            return stringBuilder.ToString();
        }

        private void BuildMembersDescription(TypeInfo typeInfo, StringBuilder stringBuilder, XElement docMembers)
        {
            var enumValues = Enum.GetValues(typeInfo).Cast<object>();
            if (typeInfo.GetCustomAttribute<FlagsAttribute>() != null)
            {
                enumValues = enumValues.Where(x =>
                {
                    long val = Convert.ToInt64(x);
                    return val != 0 && ((val & (val - 1)) == 0);
                });
            }
            SnakeCaseNamingStrategy snakeCaseNamingStrategy = new SnakeCaseNamingStrategy();

            foreach (object enumValue in enumValues)
            {
                var member = typeInfo.GetMember(enumValue.ToString()).Single();
                var docMember = this.GetDocMember(docMembers, member);

                string name = enumValue.ToString();
                string description = docMember?.Value?.Trim() ?? "(沒有描述)";

                stringBuilder.AppendFormat("* **{0}** - {1}", snakeCaseNamingStrategy.GetPropertyName(name, false), description);
                stringBuilder.AppendLine();
            }
        }
        private string GetMemberFullName(MemberInfo member)
        {
            string memberScope = "";

            if (member.DeclaringType != null)
            {
                memberScope = this.GetMemberFullName(member.DeclaringType);
            }
            else if (member is Type type)
            {
                memberScope = type.Namespace;
            }

            return $"{memberScope}.{member.Name}";
        }

        private string GetMemberId(MemberInfo member)
        {
            char memberKindPrefix = this.GetMemberPrefix(member);
            string memberName = this.GetMemberFullName(member);

            return $"{memberKindPrefix}:{memberName}";
        }

        private char GetMemberPrefix(MemberInfo member)
        {
            string typeName = member.GetType().Name;

            return typeName switch
            {
                "MdFieldInfo" => 'F',
                _ => typeName.Replace("Runtime", "")[0],
            };
        }

        /// <summary>
        /// Get the xml element representing the member
        /// </summary>
        /// <param name="membersRoot"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        private XElement GetDocMember(XElement membersRoot, MemberInfo member)
        {
            string memberId = this.GetMemberId(member);

            return membersRoot
                .Elements("member")
                .FirstOrDefault(e => e.Attribute("name")?.Value == memberId);
        }

        /// <summary>
        /// Get the xml element representing the member
        /// </summary>
        /// <param name="membersRoot"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        private XElement GetTypeMember(XElement membersRoot, TypeInfo member)
        {
            string memberId = this.GetMemberId(member);

            return membersRoot
                .Elements("member")
                .First(e => e.Attribute("name")?.Value == memberId);
        }

        private XElement LoadXmlMembers(TypeInfo typeInfo)
        {
            var file = this.GetXmlDocFile(typeInfo.Assembly);
            var docXml = XDocument.Load(file.FullName);
            var xmlRoot = docXml.Root;

            if (xmlRoot == null)
            {
                throw new ArgumentNullException(nameof(xmlRoot) + ", for " + typeInfo.FullName);
            }

            return xmlRoot.Element("members");
        }

        /// <summary>
        /// Find the XML documentation files for a given assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        private FileInfo GetXmlDocFile(Assembly assembly, CultureInfo culture = null)
        {
            string fileName = Path.GetFileNameWithoutExtension(assembly.Location) + ".xml";

            return this.EnumeratePossibleXmlDocumentationLocation(assembly, culture ?? CultureInfo.CurrentCulture)
                       .Select(directory => Path.Combine(directory, fileName))
                       .Select(filePath => new FileInfo(filePath))
                       .FirstOrDefault(file => file.Exists)
                   ?? throw new ArgumentException($"No XML doc file found for assembly '{assembly.FullName}'");
        }


        private IEnumerable<string> EnumeratePossibleXmlDocumentationLocation(Assembly assembly, CultureInfo culture)
        {
            var currentCulture = culture;

            string[] locations = new[]
            {
                new FileInfo(assembly.Location)?.Directory?.FullName,
                AppContext.BaseDirectory
            };

            foreach (string location in locations)
            {
                while (currentCulture.Name != CultureInfo.InvariantCulture.Name)
                {
                    yield return Path.Combine(location, currentCulture.Name);
                    currentCulture = currentCulture.Parent;
                }

                yield return Path.Combine(location, String.Empty);
            }
        }

        #endregion Private Methods
    }
}
