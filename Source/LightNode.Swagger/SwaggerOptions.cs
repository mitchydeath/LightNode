﻿using LightNode.Swagger.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightNode.Swagger
{
    public class SwaggerOptions
    {
        public string ApiBasePath { get; private set; }

        public Swagger.Schema.Info Info { get; set; }

        /// <summary>
        /// (FilePath, LoadedEmbeddedBytes) => CustomBytes)
        /// </summary>
        public Func<string, byte[], byte[]> InjectCustomResource { get; set; }
        public Func<IDictionary<string, object>, string> CustomHost { get; set; }
        public string XmlDocumentPath { get; set; }

        public bool IsEmitEnumAsString { get; set; }

        public SwaggerOptions(string title, string apiBasePath)
        {
            ApiBasePath = apiBasePath;
            Info = new Info { description = "", version = "1.0", title = title };
        }
    }
}
