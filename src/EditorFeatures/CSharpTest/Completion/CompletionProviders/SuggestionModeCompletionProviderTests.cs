﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Completion.Providers;
using Microsoft.CodeAnalysis.CSharp.Completion.Providers;
using Microsoft.CodeAnalysis.Editor.UnitTests.Workspaces;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Completion.CompletionProviders
{
    public class SuggestionModeCompletionProviderTests : AbstractCSharpCompletionProviderTests
    {
        internal override ICompletionProvider CreateCompletionProvider()
        {
            return new SuggestionModeCompletionProvider();
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void AfterFirstExplicitArgument()
        {
            VerifyNotBuilder(AddInsideMethod(@"Func<int, int, int> f = (int x, i $$"));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void AfterFirstImplicitArgument()
        {
            VerifyNotBuilder(AddInsideMethod(@"Func<int, int, int> f = (x, i $$"));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void AfterFirstImplicitArgumentInMethodCall()
        {
            var markup = @"class c
{
    private void bar(Func<int, int, bool> f) { }
    
    private void foo()
    {
        bar((x, i $$
    }
}
";
            VerifyNotBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void AfterFirstExplicitArgumentInMethodCall()
        {
            var markup = @"class c
{
    private void bar(Func<int, int, bool> f) { }
    
    private void foo()
    {
        bar((int x, i $$
    }
}
";
            VerifyNotBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void DelegateTypeExpected1()
        {
            var markup = @"using System;

class c
{
    private void bar(Func<int, int, bool> f) { }
    
    private void foo()
    {
        bar($$
    }
}
";
            VerifyBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void DelegateTypeExpected2()
        {
            VerifyBuilder(AddUsingDirectives("using System;", AddInsideMethod(@"Func<int, int, int> f = $$")));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void ObjectInitializerDelegateType()
        {
            var markup = @"using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    public Func<int> myfunc { get; set; }
}

class a
{
    void foo()
    {
        var b = new Program() { myfunc = $$
    }
}";
            VerifyBuilder(markup);
        }

        [Fact, WorkItem(817145), Trait(Traits.Feature, Traits.Features.Completion)]
        public void ExplicitArrayInitializer()
        {
            var markup = @"using System;

class a
{
    void foo()
    {
        Func<int, int>[] myfunc = new Func<int, int>[] { $$;
    }
}";
            VerifyBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void ImplicitArrayInitializerUnknownType()
        {
            var markup = @"using System;

class a
{
    void foo()
    {
        var a = new [] { $$;
    }
}";
            VerifyNotBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void ImplicitArrayInitializerKnownDelegateType()
        {
            var markup = @"using System;

class a
{
    void foo()
    {
        var a = new [] { x => 2 * x, $$
    }
}";
            VerifyBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void TernaryOperatorUnknownType()
        {
            var markup = @"using System;

class a
{
    void foo()
    {
        var a = true ? $$
    }
}";
            VerifyNotBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void TernaryOperatorKnownDelegateType1()
        {
            var markup = @"using System;

class a
{
    void foo()
    {
        var a = true ? x => x * 2 : $$
    }
}";
            VerifyBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void TernaryOperatorKnownDelegateType2()
        {
            var markup = @"using System;

class a
{
    void foo()
    {
        Func<int, int> a = true ? $$
    }
}";
            VerifyBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void OverloadTakesADelegate1()
        {
            var markup = @"using System;

class a
{
    void foo(int a) { }
    void foo(Func<int, int> a) { }

    void bar()
    {
        this.foo($$
    }
}";
            VerifyBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void OverloadTakesDelegate2()
        {
            var markup = @"using System;

class a
{
    void foo(int i, int a) { }
    void foo(int i, Func<int, int> a) { }

    void bar()
    {
        this.foo(1, $$
    }
}";
            VerifyBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void ExplicitCastToDelegate()
        {
            var markup = @"using System;

class a
{

    void bar()
    {
        (Func<int, int>) ($$
    }
}";
            VerifyBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        [WorkItem(860580)]
        public void ReturnStatement()
        {
            var markup = @"using System;

class a
{
    Func<int, int> bar()
    {
        return $$
    }
}";
            VerifyBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void BuilderInAnonymousType1()
        {
            var markup = @"using System;

class a
{
    int bar()
    {
        var q = new {$$
    }
}";
            VerifyBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void BuilderInAnonymousType2()
        {
            var markup = @"using System;

class a
{
    int bar()
    {
        var q = new {$$ 1, 2 };
    }
}";
            VerifyBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void BuilderInAnonymousType3()
        {
            var markup = @"using System;
class a
{
    int bar()
    {
        var q = new {Name = 1, $$ };
    }
}";
            VerifyBuilder(markup);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void BuilderInFromClause()
        {
            var markup = @"using System;
using System.Linq;

class a
{
    int bar()
    {
        var q = from $$
    }
}";
            VerifyBuilder(markup.ToString());
        }

        [WorkItem(823968)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void BuilderInJoinClause()
        {
            var markup = @"using System;
using System.Linq;
using System.Collections.Generic;

class a
{
    int bar()
    {
        var list = new List<int>();
        var q = from a in list
                join $$
    }
}";
            VerifyBuilder(markup.ToString());
        }

        [WorkItem(544290)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void ParenthesizedLambdaArgument()
        {
            var markup = @"using System;
class Program
{
    static void Main(string[] args)
    {
        Console.CancelKeyPress += new ConsoleCancelEventHandler((a$$, e) => { });
    }
}";
            VerifyBuilder(markup);
        }

        [WorkItem(544379)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void IncompleteParenthesizedLambdaArgument()
        {
            var markup = @"using System;
class Program
{
    static void Main(string[] args)
    {
        Console.CancelKeyPress += new ConsoleCancelEventHandler((a$$
    }
}";
            VerifyBuilder(markup);
        }

        [WorkItem(544379)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void IncompleteNestedParenthesizedLambdaArgument()
        {
            var markup = @"using System;
class Program
{
    static void Main(string[] args)
    {
        Console.CancelKeyPress += new ConsoleCancelEventHandler(((a$$
    }
}";
            VerifyNotBuilder(markup);
        }

        [WorkItem(546363)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void BuilderForLinqExpression()
        {
            var markup = @"using System;
using System.Linq.Expressions;
 
public class Class
{
    public void Foo(Expression<Action<int>> arg)
    {
        Foo($$
    }
}";
            VerifyBuilder(markup);
        }

        [WorkItem(546363)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void NotInTypeParameter()
        {
            var markup = @"using System;
using System.Linq.Expressions;
 
public class Class
{
    public void Foo(Expression<Action<int>> arg)
    {
        Enumerable.Empty<$$
    }
}";
            VerifyNotBuilder(markup);
        }

        [WorkItem(611477)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void ExtensionMethodFaultTolerance()
        {
            var markup = @"using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
 
namespace Outer
{
    public struct ImmutableArray<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }
 
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
 
    public static class ReadOnlyArrayExtensions
    {
        public static ImmutableArray<TResult> Select<T, TResult>(this ImmutableArray<T> array, Func<T, TResult> selector)
        {
            throw new NotImplementedException();
        }
    }
 
    namespace Inner
    {
        class Program
        {
            static void Main(string[] args)
            {
                args.Select($$
            }
        }
    }
}
";
            VerifyBuilder(markup);
        }

        [WorkItem(834609)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void LambdaWithAutomaticBraceCompletion()
        {
            var markup = @"using System;
using System;
 
public class Class
{
    public void Foo()
    {
        EventHandler h = (s$$)
    }
}";
            VerifyBuilder(markup);
        }

        [WorkItem(858112)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void ThisConstructorInitializer()
        {
            var markup = @"using System;
class X 
{ 
    X(Func<X> x) : this($$) { } 
}";
            VerifyBuilder(markup);
        }

        [WorkItem(858112)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void BaseConstructorInitializer()
        {
            var markup = @"using System;
class B
{
    public B(Func<B> x) {}
}

class D : B 
{ 
    D() : base($$) { } 
}";
            VerifyBuilder(markup);
        }

        [WorkItem(887842)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void PreprocessorExpression()
        {
            var markup = @"class C
{
#if $$
}";
            VerifyBuilder(markup);
        }

        [WorkItem(967254)]
        [Fact, Trait(Traits.Feature, Traits.Features.Completion)]
        public void ImplicitArrayInitializerAfterNew()
        {
            var markup = @"using System;

class a
{
    void foo()
    {
        int[] a = new $$;
    }
}";
            VerifyBuilder(markup);
        }

        private void VerifyNotBuilder(string markup)
        {
            VerifyWorker(markup, isBuilder: false);
        }

        private void VerifyBuilder(string markup)
        {
            VerifyWorker(markup, isBuilder: true);
        }

        private void VerifyWorker(string markup, bool isBuilder)
        {
            string code;
            int position;
            MarkupTestFile.GetPosition(markup, out code, out position);

            using (var workspaceFixture = new CSharpTestWorkspaceFixture())
            {
                var document1 = workspaceFixture.UpdateDocument(code, SourceCodeKind.Regular);
                CheckResults(document1, position, isBuilder);

                if (CanUseSpeculativeSemanticModel(document1, position))
                {
                    var document2 = workspaceFixture.UpdateDocument(code, SourceCodeKind.Regular, cleanBeforeUpdate: false);
                    CheckResults(document2, position, isBuilder);
                }
            }
        }

        private void CheckResults(Document document, int position, bool isBuilder)
        {
            var triggerInfo = CompletionTriggerInfo.CreateTypeCharTriggerInfo('a', isAugment: true);
            var provider = CreateCompletionProvider();

            if (isBuilder)
            {
                var group = provider.GetGroupAsync(document, position, triggerInfo, CancellationToken.None).Result;
                Assert.NotNull(group);
                Assert.NotNull(group.Builder);
            }
            else
            {
                var group = provider.GetGroupAsync(document, position, triggerInfo, CancellationToken.None).Result;

                if (group != null)
                {
                    Assert.True(group.Builder == null, "group.Builder == " + group.Builder.DisplayText);
                }
            }
        }
    }
}
