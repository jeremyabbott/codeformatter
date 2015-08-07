﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.DotNet.CodeFormatter.Analyzers;

using Xunit;

namespace Microsoft.DotNet.CodeFormatter.Analyzers.Tests
{
    public sealed class ProvideExplicitVariableTypeAnalyzerTests : AnalyzerFixerTestBase
    {
        public ProvideExplicitVariableTypeAnalyzerTests()
        {
            DisableAllDiagnostics();
            EnableDiagnostic(ProvideExplicitVariableTypeAnalyzer.DiagnosticId);
        }

        [Fact]
        public void TestSimpleTypeExplicitDeclaration()
        {
            const string input = @"
class C1
{
    void M()
    {
        int x = 0;
        bool[] y = { true };  
        int[][] z = new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 6 } };
    }
}";
            const string expected = input; 
            Verify(input, expected, runFormatter: false);
        }

        [Fact]
        public void TestSimpleObviousTypeVarDeclaration()
        {
            const string input = @"
class C1
{
    void M()
    {
        var x = 0;
        var y = new bool[] { true, false };
    }
}";
            const string expected = input;
            Verify(input, expected, runFormatter: false);
        }

        [Fact]
        public void TestSimpleNonobviousTypeVarDeclaration()
        {
            const string input = @"
class C1
{
    bool[] T() 
    {
        return new[] { true };
    }

    void M(int a, bool[] b)
    {
        var x = a;
        var y = T();  
        var z = new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 6 } };
    }
}";
            const string expected = @"
class C1
{
    bool[] T() 
    {
        return new[] { true };
    }

    void M(int a, bool[] b)
    {
        int x = a;
        bool[] y = T();
        int[][] z = new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 6 } };
    }
}";
            Verify(input, expected, runFormatter: false);
        }

        [Fact]
        public void TestUserDefinedTypeExplicitDeclaration()
        {
            const string input = @"
class C1
{
    class U1
    {}

    struct U2
    {}

    enum U3 { E1, E2 }

    void M()
    {
        U1 x = new U1();
        U2 y = new U2();
        U3 z = U3.E1;        
    }
}";
            const string expected = input;
            Verify(input, expected, runFormatter: false);
        }

        [Fact]
        public void TestUserDefinedObviousTypeVarDeclaration()
        {
            const string input = @"
class C1
{
    class U1
    {}

    struct U2
    {}

    enum U3 { E1, E2 }

    void M()
    {
        var x = new U1();
        var y = new U2();
        var z = U3.E1;         
    }
}";
            const string expected = @"
class C1
{
    class U1
    {}

    struct U2
    {}

    enum U3 { E1, E2 }

    void M()
    {
        var x = new U1();
        var y = new U2();
        U3 z = U3.E1;         
    }
}";
            Verify(input, expected, runFormatter: false);
        }

        [Fact]
        public void TestUserDefinedNonobviousTypeVarDeclaration()
        {
            const string input = @"
class C1
{
    class U1
    {}

    struct U2
    {}

    enum U3 { E1, E2 }

    void M(U1 u1, U2 u2, U3 u3)
    {
        var x = u1;
        var y = u2;
        var z = u3;         
    }
}";
            const string expected = @"
class C1
{
    class U1
    {}

    struct U2
    {}

    enum U3 { E1, E2 }

    void M(U1 u1, U2 u2, U3 u3)
    {
        U1 x = u1;
        U2 y = u2;
        U3 z = u3;         
    }
}";
            Verify(input, expected, runFormatter: false);
        }

        [Fact]
        public void TestSimpleForEachExplicitDeclaration()
        {
            const string input = @"
class C1
{
    void M(string[] a1, string[][] a2)
    {
        foreach (string element in a1)
        { }  
 
        foreach (string[] element in a2)
        { }
    }
}";
            const string expected = input;
            Verify(input, expected, runFormatter: false);
        }

        [Fact]
        public void TestSimpleForEachObviousVarDeclaration()
        {
            const string input = @"
class C1
{
    void M()
    {
        foreach (var element in new string[] { "" })
        { }  
 
        foreach (var element in new int[] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 } })
        { }
    }
}";
            const string expected = input;
            Verify(input, expected, runFormatter: false);
        }

        [Fact]
        public void TestSimpleForEachNonobviousVarDeclaration()
        {
            const string input = @"
class C1
{
    int[][] T()
    {
        return new int[] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 } }
    }

    void M(string[] a)
    {
        foreach (var element in a)
        { }  
 
        foreach (var element in T())
        { }
    }
}";
            const string expected = @"
class C1
{
    int[][] T()
    {
        return new int[] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 } }
    }

    void M(string[] a)
    {
        foreach (string element in a)
        { }  
 
        foreach (int[] element in T())
        { }
    }
}";
            Verify(input, expected, runFormatter: false);
        }

        [Fact]
        public void TestAnonymousTypeDeclaration()
        {
            const string input = @"
class C1
{
    void M()
    {
        var anon = new { Name = ""Terry"", Age = 34 }; 

        foreach (var anon in new[] { new { Name = ""Terry"", Age = 34 } })
        { }
    }
}";
            const string expected = input;
            Verify(input, expected, runFormatter: false);
        }

        [Fact]
        public void TestVarDeclarationInUsing()
        {
            const string input = @"
class C1
{
    class U1 : IDisposable
    {
        public void Dispose(){}
    }

    U1 T()
    {
        return new U1();
    }

    void M()
    {      
        using (U1 x = new U1())
        { } 

        using (var x = new U1())
        { } 

        using (var x = T())
        { }         
    }
}";
            const string expected = @"
class C1
{
    class U1 : IDisposable
    {
        public void Dispose(){}
    }

    U1 T()
    {
        return new U1();
    }

    void M()
    {      
        using (U1 x = new U1())
        { } 

        using (var x = new U1())
        { } 

        using (U1 x = T())
        { }         
    }
}";
            Verify(input, expected, runFormatter: false);
        }

        [Fact]
        public void TestVarDeclarationInSimpleLoop()
        {
            const string input = @"
class C1
{
    int T()
    {
        return 9;
    }

    void M()
    {
        for (var i = 0; i < 9; ++i)
        { }
 
        for (var i = T(); i > 0; --i)
        { }
    }
}";
            const string expected = @"
class C1
{
    int T()
    {
        return 9;
    }

    void M()
    {
        for (var i = 0; i < 9; ++i)
        { }
 
        for (int i = T(); i > 0; --i)
        { }
    }
}";
            Verify(input, expected, runFormatter: false);
        }

        [Fact]
        public void TestVarDeclarationWithAsExpression()
        {
            const string input = @"
class C1
{
    void M(object o)
    {
        var x = o as string;
 
        foreach (var i in o as int[])
        { }
    }
}";
            const string expected = input;
            Verify(input, expected, runFormatter: false);
        }

        [Fact]
        public void TestVarDeclarationWithMissingNodes()
        {
            const string input = @"
class C1
{
    void M()
    {
        var x = ;
        var = 10;
        var y 10;
        foreach (var z in ){}
        foreach (var in ){}
    }
}";
            const string expected = input;
            Verify(input, expected, runFormatter: false);
        }
    }
}
