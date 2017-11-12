﻿using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xunit;
using static TestHelpers;

namespace Moq.Proxy.Tests
{
    public class VisualBasicParameterFixupTest
    {

        [Fact]
        public async Task WhenParameterNameMatchesMethodNameParameterIsRenamed()
        {
            var (workspace, project) = CreateWorkspaceAndProject(LanguageNames.VisualBasic);
            var compilation = await project.GetCompilationAsync(TimeoutToken(5));

            Assert.False(compilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error),
                string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.GetMessage())));

            var document = await new ProxyGenerator().GenerateProxyAsync(workspace, project, TimeoutToken(5),
                new[]
                {
                    compilation.GetTypeByMetadataName(typeof(ICodeGenerator).FullName),
                    compilation.GetTypeByMetadataName(typeof(ICustomFormatter).FullName),
                });

            var syntax = await document.GetSyntaxRootAsync();
            document = project.AddDocument("proxy.vb", syntax, filePath: Path.GetTempFileName());

            await AssertCode.NoErrorsAsync(document);
        }
    }
}
