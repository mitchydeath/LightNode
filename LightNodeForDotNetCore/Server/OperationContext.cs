﻿using LightNode.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightNode.Server
{
    public class OperationContext
    {
        public HttpContext HttpContext { get; private set; }

        public string ContractName { get; private set; }
        public string OperationName { get; private set; }

        public AcceptVerbs Verb { get; private set; }

        // internal use

        public IContentFormatter ContentFormatter { get; internal set; }

        public IReadOnlyList<object> Parameters { get; internal set; } // object[]
        public IReadOnlyList<string> ParameterNames { get; internal set; }

        // Type as typeof(Attribute)
        internal ILookup<Type, Attribute> Attributes { get; set; }

        internal OperationContext(HttpContext httpContext, string contractName, string operationName, AcceptVerbs verb)
        {
            HttpContext = httpContext;
            ContractName = contractName;
            OperationName = operationName;
            Verb = verb;
        }

        public bool IsAttributeDefined(Type attributeType)
        {
            return Attributes.Contains(attributeType);
        }

        public bool IsAttributeDefined<T>() where T : Attribute
        {
            return Attributes.Contains(typeof(T));
        }

        public IEnumerable<Attribute> GetAttributes(Type attributeType)
        {
            return Attributes[attributeType];
        }

        public IEnumerable<T> GetAttributes<T>() where T : Attribute
        {
            return Attributes[typeof(T)].Cast<T>();
        }

        public IEnumerable<Attribute> GetAllAttributes()
        {
            return Attributes.SelectMany(xs => xs);
        }

        public override string ToString()
        {
            return ContractName + "/" + OperationName;
        }
    }
}