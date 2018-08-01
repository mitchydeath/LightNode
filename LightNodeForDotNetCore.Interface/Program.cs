using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LightNodeForDotNetCore.Interface
{
    class Program
    {
        public static void Main(string[] args)
        {
            GenerateClientInterface();
        }


        public static void GenerateClientInterface()
        {
            // Specify contract base type names to exclude from output
            var excludedBaseContractTypes = new string[] { };

            Func<Type, string> BeautifyType = null;
            BeautifyType = (Type t) =>
            {
                if (t == typeof(void)) return "void";
                if (!t.IsGenericType) return t.FullName;

                var innerFormat = string.Join(", ", t.GetGenericArguments().Select(x => BeautifyType(x)));
                return Regex.Replace(t.GetGenericTypeDefinition().FullName, @"`.+$", "") + "<" + innerFormat + ">";
            };

            var ignoreMethods = new HashSet<string> { "Equals", "GetHashCode", "GetType", "ToString" };

            var targetAssemblies = new[] { Assembly.Load("LightNodeForDotNetCoreSample"), };

            var typeFromAssemblies = targetAssemblies
                .Where(x => !Regex.IsMatch(x.GetName().Name, "^(mscorlib|System|Sytem.Web|EnvDTE)$"))
                .SelectMany(x => x.GetTypes())
                .Where(x => x != null && x.FullName != "LightNode.Server.LightNodeContract");

            var contracts = typeFromAssemblies
                .Where(x =>
                {
                    while (x != typeof(object) && x != null)
                    {
                        if (excludedBaseContractTypes.Contains(x.FullName)) return false;
                        if (x.FullName == "LightNode.Server.LightNodeContract") return true;
                        x = x.BaseType;
                    }
                    return false;
                })
                .Where(x => !x.IsAbstract)
                .Select(x =>
                {
                    var methods = x.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .Where(methodInfo => !(methodInfo.IsSpecialName && (methodInfo.Name.StartsWith("set_") || methodInfo.Name.StartsWith("get_"))))
                        .Where(methodInfo => !ignoreMethods.Contains(methodInfo.Name))
                        .Select(methodInfo =>
                        {
                            var retType = methodInfo.ReturnType;
                            var returnType =
                                (retType == typeof(void)) ? typeof(void)
                                : (retType == typeof(Task)) ? typeof(void)
                                : (retType.IsGenericType && retType.GetGenericTypeDefinition() == typeof(Task<>)) ? retType.GetGenericArguments()[0]
                                : retType;

                            var parameter = methodInfo.GetParameters()
                                .Select(paramInfo => new
                                {
                                    paramInfo.Name,
                                    paramInfo.ParameterType,
                                    paramInfo.IsOptional,
                                    paramInfo.DefaultValue,
                                })
                                .ToArray();

                            var parameterString = string.Join(", ", parameter.Select(p =>
                            {
                                return BeautifyType(p.ParameterType) + " " + p.Name;
                            }));

                            var parameterStringWithOptional = string.Join(", ", parameter.Select(p =>
                            {
                                var @base = BeautifyType(p.ParameterType) + " " + p.Name;
                                if (p.IsOptional)
                                {
                                    @base += " = " + (
                                        (p.DefaultValue == null) ? "null"
                                      : (p.DefaultValue is string) ? "\"" + p.DefaultValue + "\""
                                      : (p.DefaultValue is CancellationToken) ? "default(CancellationToken)"
                                      : p.DefaultValue.ToString().ToLower());
                                }
                                return @base;
                            }));

                            return new
                            {
                                OperationName = methodInfo.Name,
                                ReturnType = returnType,
                                Parameters = parameter,
                                ParameterString = parameterString,
                                ParameterStringWithOptional = parameterStringWithOptional
                            };
                        })
                        .ToArray();

                    return new
                    {
                        RootName = x.Name,
                        InterfaceName = "_I" + x.Name,
                        Operations = methods
                    };
                })
                .ToArray();


            var sb = new StringBuilder();

            foreach (var contract in contracts)
            {
                sb.AppendLine($"    public interface {contract.InterfaceName} : {nameof(IClientApi)}");
                sb.AppendLine($"    {{");

                foreach (var operation in contract.Operations)
                {
                    var optionParameters = operation.Parameters.Any() ? ", " : "";
                    if (operation.ReturnType == typeof(void))
                    {
                        sb.AppendLine($"        Task {operation.OperationName}Async({operation.ParameterStringWithOptional}{optionParameters}IProgress<float> reportProgress = null);");
                    }
                    else
                    {
                        sb.AppendLine($"        Task<{BeautifyType(operation.ReturnType)}> {operation.OperationName}Async({operation.ParameterStringWithOptional}{optionParameters}IProgress<float> reportProgress = null);");
                    }
                }
                sb.AppendLine($"    }}");
                sb.AppendLine();
            }

            var template =
$@"using System; 
using System.Threading.Tasks;
using LightNodeForDotNetCore.Interface;

namespace LightNode 
{{ 
{sb.ToString()}
}}";


            var outputPath = Directory.GetCurrentDirectory() + @"\..\..\..\Output\LightNodeClient.cs";

            // Write the new file.
            File.WriteAllText(outputPath, template);
        }
    }
}