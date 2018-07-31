﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Owin.Testing;
using LightNode;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using LightNode.Formatter;

namespace LightNode.Server.Tests
{
    [TestClass]
    public class BasicOperation
    {
        [TestMethod]
        public void Hello()
        {
            MockEnv.CreateRequest("/Hello/Say").GetString().Is("\"Hello LightNode\"");
            MockEnv.CreateRequest("/Hello/Say").PostAndGetString(new StringKeyValuePairCollection()).Is("\"Hello LightNode\"");
        }

        [TestMethod]
        public void GetBasic()
        {
            MockEnv.CreateRequest("/TestContract/Add?x=10&y=20").GetString().Is("30");
            MockEnv.CreateRequest("/TestContract/AddWithDefault?y=20&z=5&x=10").GetString().Is("35");
            MockEnv.CreateRequest("/TestContract/AddWithDefault?x=10&y=20").GetString().Is("330");

            MockEnv.CreateRequest("/TestContract/TaskAdd?x=10&y=20").GetString().Is("30");
            MockEnv.CreateRequest("/TestContract/TaskAddWithDefault?y=20&z=5&x=10").GetString().Is("35");
            MockEnv.CreateRequest("/TestContract/TaskAddWithDefault?x=10&y=20").GetString().Is("330");

            var guid = Guid.NewGuid().ToString();
            TestContract.VoidBeforeAfter[guid] = "Before";
            MockEnv.CreateRequest("/TestContract/VoidCheck?after=After&guid=" + guid).GetAsync().Result.Dispose();
            TestContract.VoidBeforeAfter[guid].Is("After");

            TestContract.VoidBeforeAfter[guid] = "Before";
            MockEnv.CreateRequest("/TestContract/TaskVoidCheck?after=After&guid=" + guid).GetAsync().Result.Dispose();
            TestContract.VoidBeforeAfter[guid].Is("After");
        }

        [TestMethod]
        public void PostBasic()
        {
            MockEnv.CreateRequest("/TestContract/Add")
                .PostAndGetString(new StringKeyValuePairCollection { { "x", "10" }, { "y", "20" } })
                .Is("30");
            MockEnv.CreateRequest("/TestContract/AddWithDefault")
                .PostAndGetString(new StringKeyValuePairCollection { { "y", "20" }, { "z", "5" }, { "x", "10" } })
                .Is("35");
            MockEnv.CreateRequest("/TestContract/AddWithDefault")
                .PostAndGetString(new StringKeyValuePairCollection { { "y", "20" }, { "x", "10" } })
                .Is("330");

            MockEnv.CreateRequest("/TestContract/TaskAdd")
                .PostAndGetString(new StringKeyValuePairCollection { { "x", "10" }, { "y", "20" } })
                .Is("30");
            MockEnv.CreateRequest("/TestContract/TaskAddWithDefault")
                .PostAndGetString(new StringKeyValuePairCollection { { "y", "20" }, { "z", "5" }, { "x", "10" } })
                .Is("35");
            MockEnv.CreateRequest("/TestContract/TaskAddWithDefault")
                .PostAndGetString(new StringKeyValuePairCollection { { "y", "20" }, { "x", "10" } })
                .Is("330");

            var guid = Guid.NewGuid().ToString();
            TestContract.VoidBeforeAfter[guid] = "Before";
            MockEnv.CreateRequest("/TestContract/VoidCheck")
                .And(x => x.Content = new FormUrlEncodedContent(new StringKeyValuePairCollection { { "guid", guid }, { "after", "After" } }))
                .PostAsync().Result.Dispose();
            TestContract.VoidBeforeAfter[guid].Is("After");

            TestContract.VoidBeforeAfter[guid] = "Before";
            MockEnv.CreateRequest("/TestContract/TaskVoidCheck")
                .And(x => x.Content = new FormUrlEncodedContent(new StringKeyValuePairCollection { { "guid", guid }, { "after", "After" } }))
                .PostAsync().Result.Dispose();
            TestContract.VoidBeforeAfter[guid].Is("After");
        }

        [TestMethod]
        public void CaseSensitive()
        {
            MockEnv.CreateRequest("/heLLo/pInG").GetString().Is("\"Pong\"");
        }

        [TestMethod]
        public void NotFound()
        {
            MockEnv.CreateRequest("").GetAsync().Result.StatusCode.Is(System.Net.HttpStatusCode.NotFound);
            MockEnv.CreateRequest("/hello").GetAsync().Result.StatusCode.Is(System.Net.HttpStatusCode.NotFound);
            MockEnv.CreateRequest("/hello/").GetAsync().Result.StatusCode.Is(System.Net.HttpStatusCode.NotFound);
            MockEnv.CreateRequest("/hello/pin").GetAsync().Result.StatusCode.Is(System.Net.HttpStatusCode.NotFound);
            MockEnv.CreateRequest("/hello/pingoo").GetAsync().Result.StatusCode.Is(System.Net.HttpStatusCode.NotFound);
            MockEnv.CreateRequest("/hello/ping/oo").GetAsync().Result.StatusCode.Is(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void Array()
        {
            MockEnv.CreateRequest("/ArrayContract/Sum?xs=1&xs=2&xs=3").GetString().Is("6");
            MockEnv.CreateRequest("/ArrayContract/Sum?xs=1000").GetString().Is("1000"); // single arg

            MockEnv.CreateRequest("/ArrayContract/Sum2?x=2&xs=1&xs=2&xs=3&y=30&ys=40&ys=50").GetString().Is("128");
            MockEnv.CreateRequest("/ArrayContract/Sum2?x=2&xs=1&y=30&ys=50").GetString().Is("83");
        }

        [TestMethod]
        public void ParameterMismatch()
        {
            MockEnv.CreateRequest("/TestContract/Add").GetAsync().Result.StatusCode.Is(System.Net.HttpStatusCode.BadRequest);
            MockEnv.CreateRequest("/TestContract/Add?x=10").GetAsync().Result.StatusCode.Is(System.Net.HttpStatusCode.BadRequest);
            MockEnv.CreateRequest("/TestContract/Add?x=10&x=20").GetAsync().Result.StatusCode.Is(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void UrlEncode()
        {
            MockEnv.CreateRequest("/Hello/Echo?x=あいうえお").GetString().Is("\"あいうえお\"");
            MockEnv.CreateRequest("/Hello/Echo").PostAndGetString(new StringKeyValuePairCollection { { "x", "あいうえお" } })
                .Is("\"あいうえお\"");
        }

        [TestMethod]
        public void VerbOption()
        {
            MockEnv.CreateRequest("/TestContract/PutOnly").GetAsync().Result.StatusCode.Is(System.Net.HttpStatusCode.MethodNotAllowed);
            MockEnv.CreateRequest("/TestContract/PutOnly").SendAsync("PUT").Result.StatusCode.Is(System.Net.HttpStatusCode.NoContent);
            MockEnv.CreateRequest("/TestContract/PatchOnly").SendAsync("PATCH").Result.StatusCode.Is(System.Net.HttpStatusCode.NoContent);
        }

        [TestMethod]
        public void RawBody()
        {
            var sendBytes = Encoding.UTF8.GetBytes("dファdfasfzjew３ふぁｚｆｄｆｓｆうぇｗ"); // japanese hiragana:)

            // echo
            var req = MockEnv.CreateRequest("/TestContract/EchoByte")
                .And(x => { x.Content = new ByteArrayContent(sendBytes); });
            req.PostAsync().Result.Content.ReadAsByteArrayAsync().Result.IsStructuralEqual(sendBytes);

            // length
            var req2 = MockEnv.CreateRequest("/TestContract/PostByte")
                .And(x => { x.Content = new ByteArrayContent(sendBytes); });
            req2.PostAsync().Result.Content.ReadAsStringAsync().Result.Is(sendBytes.Length.ToString());
        }
    }

    public class Hello : LightNodeContract
    {
        public string Say()
        {
            return "Hello LightNode";
        }

        public string Ping()
        {
            return "Pong";
        }

        public string Echo(string x)
        {
            return x;
        }
    }

    public class TestContract : LightNodeContract
    {
        public int Add(int x, int y)
        {
            Environment.IsNotNull();
            return x + y;
        }

        public int AddWithDefault(int x, int y, int z = 300)
        {
            Environment.IsNotNull();
            return x + y + z;
        }

        public Task<int> TaskAdd(int x, int y)
        {
            Environment.IsNotNull();
            return Task.Run(() => x + y);
        }
        public Task<int> TaskAddWithDefault(int x, int y, int z = 300)
        {
            Environment.IsNotNull();
            return Task.Run(() => x + y + z);
        }

        public static ConcurrentDictionary<string, string> VoidBeforeAfter = new ConcurrentDictionary<string, string>();
        public void VoidCheck(string guid, string after)
        {
            Environment.IsNotNull();
            VoidBeforeAfter[guid] = after;
        }

        public async Task TaskVoidCheck(string guid, string after)
        {
            Environment.IsNotNull();
            await Task.Delay(TimeSpan.FromMilliseconds(10)).ConfigureAwait(false);
            VoidBeforeAfter[guid] = after;
        }

        [OperationOption(AcceptVerbs.Put)]
        public void PutOnly()
        {
        }

        [Patch]
        public void PatchOnly()
        {
        }

        [Post, IgnoreClientGenerate]
        public int PostByte()
        {
            // Take raw stream
            var body = this.Environment["owin.RequestBody"] as Stream;
            byte[] bodyBytes;
            using (var ms = new MemoryStream())
            {
                body.CopyTo(ms);
                bodyBytes = ms.ToArray();
            }
            return bodyBytes.Length;
        }


        [IgnoreClientGenerate]
        [OperationOption(AcceptVerbs.Post, typeof(RawOctetStreamContentFormatterFactory))]
        public byte[] EchoByte()
        {
            // Take row stream
            var body = this.Environment["owin.RequestBody"] as Stream;
            byte[] bodyBytes;
            using (var ms = new MemoryStream())
            {
                body.CopyTo(ms);
                bodyBytes = ms.ToArray();
            }
            return bodyBytes;
        }

    }

    public class ArrayContract : LightNodeContract
    {
        public int Sum(int[] xs)
        {
            return xs.Sum();
        }

        public int Sum2(int x, int[] xs, int y, int[] ys)
        {
            return x + xs.Sum() + y + ys.Sum();
        }
    }
}