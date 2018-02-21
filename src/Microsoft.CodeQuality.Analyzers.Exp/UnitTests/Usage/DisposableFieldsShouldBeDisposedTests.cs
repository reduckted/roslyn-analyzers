﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeQuality.Analyzers.Exp.Usage;
using Test.Utilities;
using Xunit;

namespace Microsoft.CodeQuality.Analyzers.Exp.UnitTests.Usage
{
    public partial class DisposableFieldsShouldBeDisposedTests : DiagnosticAnalyzerTestBase
    {
        protected override DiagnosticAnalyzer GetBasicDiagnosticAnalyzer() => new DisposableFieldsShouldBeDisposed();
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new DisposableFieldsShouldBeDisposed();

        private DiagnosticResult GetCSharpResultAt(int line, int column, string containingType, string field, string fieldType) =>
            GetCSharpResultAt(line, column, DisposableFieldsShouldBeDisposed.Rule, containingType, field, fieldType);

        private DiagnosticResult GetBasicResultAt(int line, int column, string containingType, string field, string fieldType) =>
            GetBasicResultAt(line, column, DisposableFieldsShouldBeDisposed.Rule, containingType, field, fieldType);

        [Fact]
        public void DisposableAllocationInConstructor_AssignedDirectly_Disposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private readonly A a;
    public B()
    {
        a = new A();
    }

    public void Dispose()
    {
        a.Dispose();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private ReadOnly a As A
    Sub New()
        a = New A()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        a.Dispose()
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocationInConstructor_AssignedDirectly_NotDisposed_Diagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private readonly A a;
    public B()
    {
        a = new A();
    }

    public void Dispose()
    {
    }
}
",
            // Test0.cs(14,24): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Dispose or Close on this field.
            GetCSharpResultAt(14, 24, "B", "a", "A"));

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private ReadOnly a As A
    Sub New()
        a = New A()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class",
            // Test0.vb(14,22): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Dispose or Close on this field.
            GetBasicResultAt(14, 22, "B", "a", "A"));
        }

        [Fact]
        public void DisposableAllocationInMethod_AssignedDirectly_Disposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    public void SomeMethod()
    {
        a = new A();
    }

    public void Dispose()
    {
        a.Dispose();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Sub SomeMethod()
        a = New A()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        a.Dispose()
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocationInMethod_AssignedDirectly_NotDisposed_Diagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    public void SomeMethod()
    {
        a = new A();
    }

    public void Dispose()
    {
    }
}
",
            // Test0.cs(14,15): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Dispose or Close on this field.
            GetCSharpResultAt(14, 15, "B", "a", "A"));

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Sub SomeMethod()
        a = New A()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class",
            // Test0.vb(14,13): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Dispose or Close on this field.
            GetBasicResultAt(14, 13, "B", "a", "A"));
        }

        [Fact]
        public void DisposableAllocationInFieldInitializer_AssignedDirectly_Disposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a = new A();
    private readonly A a2 = new A();
    
    public void Dispose()
    {
        a.Dispose();
        a2.Dispose();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A = New A()
    Private ReadOnly a2 As New A()

    Public Sub Dispose() Implements IDisposable.Dispose
        a.Dispose()
        a2.Dispose()
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocationInFieldInitializer_AssignedDirectly_NotDisposed_Diagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a = new A();
    private readonly A a2 = new A();
    
    public void Dispose()
    {
    }
}
",
            // Test0.cs(14,15): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Dispose or Close on this field.
            GetCSharpResultAt(14, 15, "B", "a", "A"),
            // Test0.cs(15,24): warning CA2213: 'B' contains field 'a2' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Dispose or Close on this field.
            GetCSharpResultAt(15, 24, "B", "a2", "A"));

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A = New A()
    Private ReadOnly a2 As New A()

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class",
            // Test0.vb(14,13): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Dispose or Close on this field.
            GetBasicResultAt(14, 13, "B", "a", "A"),
            // Test0.vb(15,22): warning CA2213: 'B' contains field 'a2' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Dispose or Close on this field.
            GetBasicResultAt(15, 22, "B", "a2", "A"));
        }

        [Fact]
        public void StaticField_NotDisposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private static A a = new A();
    private static readonly A a2 = new A();

    public void Dispose()
    {
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private Shared a As A = New A()
    Private Shared ReadOnly a2 As New A()

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_AssignedThroughLocal_Disposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    public void SomeMethod()
    {
        var l = new A();
        a = l;
    }

    public void Dispose()
    {
        a.Dispose();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Sub SomeMethod()
        Dim l = New A()
        a = l
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        a.Dispose()
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_AssignedThroughLocal_NotDisposed_Diagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    public void SomeMethod()
    {
        var l = new A();
        a = l;
    }

    public void Dispose()
    {
    }
}
",
            // Test0.cs(14,15): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Dispose or Close on this field.
            GetCSharpResultAt(14, 15, "B", "a", "A"));

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Sub SomeMethod()
        Dim l = New A()
        a = l
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class",
            // Test0.vb(14,13): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Dispose or Close on this field.
            GetBasicResultAt(14, 13, "B", "a", "A"));
        }

        [Fact]
        public void DisposableAllocation_AssignedThroughParameter_Disposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    public B(A p)
    {
        p = new A();
        a = p;
    }

    public void Dispose()
    {
        a.Dispose();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Sub New(p As A)
        p = New A()
        a = p
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        a.Dispose()
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_AssignedThroughParameter_NotDisposed_Diagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    public B(A p)
    {
        p = new A();
        a = p;
    }

    public void Dispose()
    {
    }
}
",
            // Test0.cs(14,15): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Dispose or Close on this field.
            GetCSharpResultAt(14, 15, "B", "a", "A"));

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Sub New(p As A)
        p = New A()
        a = p
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class",
            // Test0.vb(14,13): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Dispose or Close on this field.
            GetBasicResultAt(14, 13, "B", "a", "A"));
        }

        [Fact]
        public void DisposableSymbolWithoutAllocation_AssignedThroughParameter_Disposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    public B(A p)
    {
        a = p;
    }

    public void Dispose()
    {
        a.Dispose();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Sub New(p As A)
        a = p
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        a.Dispose()
    End Sub
End Class");
        }

        [Fact]
        public void DisposableSymbolWithoutAllocation_AssignedThroughParameter_NotDisposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    public B(A p)
    {
        a = p;
    }

    public void Dispose()
    {
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Sub New(p As A)
        a = p
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_AssignedThroughInstanceInvocation_Disposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    public B()
    {
        a = GetA();
    }

    private A GetA() => new A();

    public void Dispose()
    {
        a.Dispose();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Sub New()
        a = GetA()
    End Sub

    Private Function GetA() As A
        Return New A()
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        a.Dispose()
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_AssignedThroughInstanceInvocation_NotDisposed_Diagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    public B()
    {
        a = GetA();
    }

    private A GetA() => new A();

    public void Dispose()
    {
    }
}
",
            // Test0.cs(14,15): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Close or Dispose on this field.
            GetCSharpResultAt(14, 15, "B", "a", "A"));

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Sub New()
        a = GetA()
    End Sub

    Private Function GetA() As A
        Return New A()
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class",
            // Test0.vb(14,13): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Close or Dispose on this field.
            GetBasicResultAt(14, 13, "B", "a", "A"));
        }

        [Fact]
        public void DisposableAllocation_AssignedThroughStaticCreateInvocation_Disposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    public B()
    {
        a = Create();
    }

    private static A Create() => new A();

    public void Dispose()
    {
        a.Dispose();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Sub New()
        a = Create()
    End Sub

    Private Shared Function Create() As A
        Return New A()
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        a.Dispose()
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_AssignedThroughStaticCreateInvocation_NotDisposed_Diagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    public B()
    {
        a = Create();
    }

    private static A Create() => new A();

    public void Dispose()
    {
    }
}
",
            // Test0.cs(14,15): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Close or Dispose on this field.
            GetCSharpResultAt(14, 15, "B", "a", "A"));

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Sub New()
        a = Create()
    End Sub

    Private Shared Function Create() As A
        Return New A()
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class",
            // Test0.vb(14,13): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Close or Dispose on this field.
            GetBasicResultAt(14, 13, "B", "a", "A"));
        }

        [Fact]
        public void DisposableAllocation_AssignedInDifferentType_DisposedInContainingType_NoDiagnostic()
        {
            // We don't track disposable field assignments in different type.
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    public A a;
    public void Dispose()
    {
        a.Dispose();
    }
}

class WrapperB
{
    private B b;
    public void Create()
    {
        b.a = new A();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Public a As A

    Public Sub Dispose() Implements IDisposable.Dispose
        a.Dispose()
    End Sub
End Class

Class WrapperB
    Dim b As B
    Public Sub Create()
        b.a = new A()
    End Sub
End Class
");
        }

        [Fact]
        public void DisposableAllocation_AssignedInDifferentType_DisposedInDifferentNonDisposableType_NoDiagnostic()
        {
            // We don't track disposable field assignments in different type.
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    public A a;
    public void Dispose()
    {
    }
}

class WrapperB
{
    private B b;

    public void Create()
    {
        b.a = new A();
    }

    public void Dispose()
    {
        b.a.Dispose();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Public a As A

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class

Class WrapperB
    Dim b As B

    Public Sub Create()
        b.a = new A()
    End Sub

    Public Sub Dispose()
        b.a.Dispose()
    End Sub
End Class
");
        }

        [Fact]
        public void DisposableAllocation_AssignedInDifferentType_NotDisposed_NoDiagnostic()
        {
            // We don't track disposable field assignments in different type.
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    public A a;
    public void Dispose()
    {
    }
}

class Test
{
    public void M(B b)
    {
        b.a = new A();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Public a As A

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class

Class Test
    Public Sub M(b As B)
        b.a = new A()
    End Sub
End Class
");
        }

        [Fact]
        public void DisposableOwnershipTransferSpecialCases_Disposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;
using System.IO;
using System.Resources;

class A : IDisposable
{
    private Stream s;
    private TextReader tr;
    private TextWriter tw;
    private IResourceReader rr;

    public A(Stream s)
    {
        this.s = s;
    }

    public A(TextReader tr)
    {
        this.tr = tr;
    }

    public A(TextWriter tw)
    {
        this.tw = tw;
    }

    public A(IResourceReader rr)
    {
        this.rr = rr;
    }

    public void Dispose()
    {
        if (s != null)
        {
            s.Dispose();
        }

        if (tr != null)
        {
            tr.Dispose();
        }

        if (tw != null)
        {
            tw.Dispose();
        }

        if (rr != null)
        {
            rr.Dispose();
        }
    }
}
");

            VerifyBasic(@"
Imports System
Imports System.IO
Imports System.Resources

Class A
    Implements IDisposable

    Private s As Stream
    Private tr As TextReader
    Private tw As TextWriter
    Private rr As IResourceReader

    Public Sub New(ByVal s As Stream)
        Me.s = s
    End Sub

    Public Sub New(ByVal tr As TextReader)
        Me.tr = tr
    End Sub

    Public Sub New(ByVal tw As TextWriter)
        Me.tw = tw
    End Sub

    Public Sub New(ByVal rr As IResourceReader)
        Me.rr = rr
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If s IsNot Nothing Then
            s.Dispose()
        End If

        If tr IsNot Nothing Then
            tr.Dispose()
        End If

        If tw IsNot Nothing Then
            tw.Dispose()
        End If

        If rr IsNot Nothing Then
            rr.Dispose()
        End If
    End Sub
End Class
");
        }

        [Fact]
        public void DisposableOwnershipTransferSpecialCases_NotDisposed_Diagnostic()
        {
            VerifyCSharp(@"
using System;
using System.IO;
using System.Resources;

class A : IDisposable
{
    private Stream s;
    private TextReader tr;
    private TextWriter tw;
    private IResourceReader rr;

    public A(Stream s)
    {
        this.s = s;
    }

    public A(TextReader tr)
    {
        this.tr = tr;
    }

    public A(TextWriter tw)
    {
        this.tw = tw;
    }

    public A(IResourceReader rr)
    {
        this.rr = rr;
    }

    public void Dispose()
    {
    }
}
",
            // Test0.cs(8,20): warning CA2213: 'A' contains field 's' that is of IDisposable type 'Stream', but it is never disposed. Change the Dispose method on 'A' to call Close or Dispose on this field.
            GetCSharpResultAt(8, 20, "A", "s", "Stream"),
            // Test0.cs(9,24): warning CA2213: 'A' contains field 'tr' that is of IDisposable type 'TextReader', but it is never disposed. Change the Dispose method on 'A' to call Close or Dispose on this field.
            GetCSharpResultAt(9, 24, "A", "tr", "TextReader"),
            // Test0.cs(10,24): warning CA2213: 'A' contains field 'tw' that is of IDisposable type 'TextWriter', but it is never disposed. Change the Dispose method on 'A' to call Close or Dispose on this field.
            GetCSharpResultAt(10, 24, "A", "tw", "TextWriter"),
            // Test0.cs(11,29): warning CA2213: 'A' contains field 'rr' that is of IDisposable type 'IResourceReader', but it is never disposed. Change the Dispose method on 'A' to call Close or Dispose on this field.
            GetCSharpResultAt(11, 29, "A", "rr", "IResourceReader"));

            VerifyBasic(@"
Imports System
Imports System.IO
Imports System.Resources

Class A
    Implements IDisposable

    Private s As Stream
    Private tr As TextReader
    Private tw As TextWriter
    Private rr As IResourceReader

    Public Sub New(ByVal s As Stream)
        Me.s = s
    End Sub

    Public Sub New(ByVal tr As TextReader)
        Me.tr = tr
    End Sub

    Public Sub New(ByVal tw As TextWriter)
        Me.tw = tw
    End Sub

    Public Sub New(ByVal rr As IResourceReader)
        Me.rr = rr
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class
",
            // Test0.vb(9,13): warning CA2213: 'A' contains field 's' that is of IDisposable type 'Stream', but it is never disposed. Change the Dispose method on 'A' to call Close or Dispose on this field.
            GetBasicResultAt(9, 13, "A", "s", "Stream"),
            // Test0.vb(10,13): warning CA2213: 'A' contains field 'tr' that is of IDisposable type 'TextReader', but it is never disposed. Change the Dispose method on 'A' to call Close or Dispose on this field.
            GetBasicResultAt(10, 13, "A", "tr", "TextReader"),
            // Test0.vb(11,13): warning CA2213: 'A' contains field 'tw' that is of IDisposable type 'TextWriter', but it is never disposed. Change the Dispose method on 'A' to call Close or Dispose on this field.
            GetBasicResultAt(11, 13, "A", "tw", "TextWriter"),
            // Test0.vb(12,13): warning CA2213: 'A' contains field 'rr' that is of IDisposable type 'IResourceReader', but it is never disposed. Change the Dispose method on 'A' to call Close or Dispose on this field.
            GetBasicResultAt(12, 13, "A", "rr", "IResourceReader"));
        }

        [Fact]
        public void DisposableAllocation_DisposedWithConditionalAccess_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a = new A();
    
    public void Dispose()
    {
        a?.Dispose();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A = New A()

    Public Sub Dispose() Implements IDisposable.Dispose
        a?.Dispose()
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_AssignedToLocal_Disposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a = new A();
    
    public void Dispose()
    {
        A l = a;
        l.Dispose();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A = New A()

    Public Sub Dispose() Implements IDisposable.Dispose
        Dim l = a
        l.Dispose()
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_AssignedToLocal_NotDisposed_Diagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a = new A();
    
    public void Dispose()
    {
        A l = a;
    }
}
",
            // Test0.cs(14,15): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Close or Dispose on this field.
            GetCSharpResultAt(14, 15, "B", "a", "A"));

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A = New A()

    Public Sub Dispose() Implements IDisposable.Dispose
        Dim l = a
    End Sub
End Class",
            // Test0.vb(14,13): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Close or Dispose on this field.
            GetBasicResultAt(14, 13, "B", "a", "A"));
        }

        [Fact]
        public void DisposableAllocation_IfElseStatement_Disposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    private A b;

    public B(bool flag)
    {
        A l = new A();
        if (flag)
        {
            a = l;
        }
        else
        {
            b = l;
        }
    }

    public void Dispose()
    {
        A l = null;
        if (a != null)
        {
            l = a;
        }
        else if (b != null)
        {
            l = b;
        }

        l.Dispose();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Private b As A

    Public Sub New(ByVal flag As Boolean)
        Dim l As A = New A()
        If flag Then
            a = l
        Else
            b = l
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dim l As A = Nothing
        If a IsNot Nothing Then
            l = a
        ElseIf b IsNot Nothing Then
            l = b
        End If
        l.Dispose()
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_IfElseStatement_NotDisposed_Diagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a;
    private A b;

    public B(bool flag)
    {
        A l = new A();
        if (flag)
        {
            a = l;
        }
        else
        {
            b = l;
        }
    }

    public void Dispose()
    {
        A l = null;
        if (a != null)
        {
            l = a;
        }
        else if (b != null)
        {
            l = b;
        }
    }
}
",
            // Test0.cs(14,15): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Close or Dispose on this field.
            GetCSharpResultAt(14, 15, "B", "a", "A"),
            // Test0.cs(15,15): warning CA2213: 'B' contains field 'b' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Close or Dispose on this field.
            GetCSharpResultAt(15, 15, "B", "b", "A"));

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A
    Private b As A

    Public Sub New(ByVal flag As Boolean)
        Dim l As A = New A()
        If flag Then
            a = l
        Else
            b = l
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dim l As A = Nothing
        If a IsNot Nothing Then
            l = a
        ElseIf b IsNot Nothing Then
            l = b
        End If
    End Sub
End Class",
            // Test0.vb(14,13): warning CA2213: 'B' contains field 'a' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Close or Dispose on this field.
            GetBasicResultAt(14, 13, "B", "a", "A"),
            // Test0.vb(15,13): warning CA2213: 'B' contains field 'b' that is of IDisposable type 'A', but it is never disposed. Change the Dispose method on 'B' to call Close or Dispose on this field.
            GetBasicResultAt(15, 13, "B", "b", "A"));
        }

        [Fact]
        public void DisposableAllocation_EscapedField_NotDisposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a = new A();
    
    public void Dispose()
    {
        DisposeA(ref this.a);
    }

    private static void DisposeA(ref A a)
    {
        a.Dispose();
        a = null;
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A = New A()

    Public Sub Dispose() Implements IDisposable.Dispose
        DisposeA(a)
    End Sub

    Private Shared Sub DisposeA(ByRef a As A)
        a.Dispose()
        a = Nothing
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_DisposedWithDisposeBoolInvocation_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Dispose(bool disposed)
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a = new A();
    
    public void Dispose()
    {
        a.Dispose(true);
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub

    Public Sub Dispose(disposed As Boolean)
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A = New A()

    Public Sub Dispose() Implements IDisposable.Dispose
        a.Dispose(True)
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_DisposedInsideDisposeBool_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Dispose(bool disposed)
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a = new A();
    
    public void Dispose()
    {
        Dispose(true);
    }

    public void Dispose(bool disposed)
    {
        a.Dispose(disposed);
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub

    Public Sub Dispose(disposed As Boolean)
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A = New A()

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub

    Public Sub Dispose(disposed As Boolean)
        a.Dispose(disposed)
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_DisposedWithDisposeCloseInvocation_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Close()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a = new A();
    
    public void Dispose()
    {
        a.Close();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub

    Public Sub Close()
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A = New A()

    Public Sub Dispose() Implements IDisposable.Dispose
        a.Close()
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_AllDisposedMethodsMixed_Disposed_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Dispose(bool disposed)
    {
        throw new NotImplementedException();
    }

    public void Close()
    {
        throw new NotImplementedException();
    }
}

class B : IDisposable
{
    private A a = new A();
    private A a2 = new A();
    private A a3 = new A();
    
    public void Dispose()
    {
        a.Close();
    }
    
    public void Dispose(bool disposed)
    {
        a2.Dispose();
    }
    
    public void Close()
    {
        a3.Dispose(true);
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub

    Public Sub Dispose(disposed As Boolean)
        Throw New NotImplementedException()
    End Sub

    Public Sub Close()
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A = New A()
    Private a2 As A = New A()
    Private a3 As A = New A()

    Public Sub Dispose() Implements IDisposable.Dispose
        a.Close()
    End Sub

    Public Sub Dispose(disposed As Boolean)
        a2.Dispose()
    End Sub

    Public Sub Close()
        a3.Dispose(True)
    End Sub
End Class");
        }

        [Fact]
        public void DisposableAllocation_DisposedInsideDisposeClose_NoDiagnostic()
        {
            VerifyCSharp(@"
using System;

class A : IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Close()
    {
    }
}

class B : IDisposable
{
    private A a = new A();
    
    public void Dispose()
    {
        Close();
    }

    public void Close()
    {
        a.Close();
    }
}
");

            VerifyBasic(@"
Imports System

Class A
    Implements IDisposable
    Public Sub Dispose() Implements IDisposable.Dispose
        Throw New NotImplementedException()
    End Sub

    Public Sub Dispose(disposed As Boolean)
        Throw New NotImplementedException()
    End Sub
End Class

Class B
    Implements IDisposable

    Private a As A = New A()

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub

    Public Sub Dispose(disposed As Boolean)
        a.Dispose(disposed)
    End Sub
End Class");
        }



    }
}
