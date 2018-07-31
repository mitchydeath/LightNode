﻿using LightNode.Server;
using LightNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace LightNode.Performance
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseLightNode(new LightNode.Server.LightNodeOptions(Server.AcceptVerbs.Get | Server.AcceptVerbs.Post,
                new LightNode.Formatter.JsonNetContentFormatter()));
        }
    }

    public class Perf : LightNode.Server.LightNodeContract
    {
        public MyClass Echo(string name, int x, int y, MyEnum e)
        {
            return new MyClass { Name = name, Sum = (x + y) * (int)e };
        }

        public void Test(string a = null, int? x = null, MyEnum2? z = null)
        {
        }

        public System.Threading.Tasks.Task Te()
        {
            return System.Threading.Tasks.Task.FromResult(1);
        }

        public void TestArray(string[] array, int[] array2, MyEnum[] array3)
        {
        }

        public void TeVoid()
        {
        }

        [Post]
        public void ByteArrayCheck1(int x, string y, byte[] byteArray)
        {
        }

        [Post]
        public void ByteArrayCheck2(string[] array, int[] array2, MyEnum[] array3, byte[] byteArray)
        {
        }

        [Post]
        public void ByteArrayCheck3(int x, string y, byte[] byteArray, string a = null, int? xxx = null, MyEnum2? z = null)
        {
        }

        public string Te4(string xs)
        {
            return xs;
        }

        [LightNode.Server.IgnoreOperation]
        public void Ignore(string a)
        {
        }

        [LightNode.Server.IgnoreClientGenerate]
        public void IgnoreClient(string a)
        {
        }

        [Post]
        public string PostString(string hoge)
        {
            return hoge;
        }
    }

    [DebugOnlyClientGenerate]
    public class DebugOnlyTest : LightNodeContract
    {
        public void Hoge()
        {
        }
    }

    public class DebugOnlyMethodTest :LightNodeContract
    {
        [DebugOnlyClientGenerate]
        public void Hoge()
        {
        }
    }

    public class MyClass
    {
        public string Name { get; set; }
        public int Sum { get; set; }
    }

    public enum MyEnum
    {
        A = 2,
        B = 3,
        C = 4
    }

    public enum MyEnum2 : ulong
    {
        A = 100,
        B = 3000,
        C = 50000
    }
}