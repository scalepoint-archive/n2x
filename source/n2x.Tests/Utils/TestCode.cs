﻿using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using Xunit;

namespace n2x.Tests.Utils
{
    public class TestCode
    {
        public Document Document { get; set; }
        //public SyntaxTree SyntaxTree { get; private set; }

        //public SyntaxNode SyntaxTreeRoot => SyntaxTree.GetRoot();

        //public Compilation Compilation { get; private set; }
        //public SemanticModel SemanticModel { get; private set; }

        //public ClassDeclarationSyntax ClassDeclaration
        //{
        //    get
        //    {
        //        var compil = (CompilationUnitSyntax)SyntaxTreeRoot;
        //        var @namespace = (NamespaceDeclarationSyntax) compil.Members.Single();
        //        return (ClassDeclarationSyntax)@namespace.Members.Single();
        //    }
        //}

        public TestCode(string text)
        {
            //SyntaxTree = SyntaxFactory.ParseSyntaxTree(text);

            var systemAsseemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            if (systemAsseemblyPath == null)
            {
                throw new Exception("Could not obtain system assembly reference path");
            }

            var nUnitAssemblyPath = Path.GetDirectoryName(typeof(TestFixtureAttribute).Assembly.Location);
            if (nUnitAssemblyPath == null)
            {
                throw new Exception("Could not obtain NUnit assembly reference path");
            }

            var xUnitAssemblyPath = Path.GetDirectoryName(typeof (FactAttribute).Assembly.Location);
            if (xUnitAssemblyPath == null)
            {
                throw new Exception("Could not obtain Xunit assembly reference path");
            }

            var xUnitExtensionsAssemblyPath = Path.GetDirectoryName(typeof(Xunit.TheoryAttribute).Assembly.Location);
            if (xUnitExtensionsAssemblyPath == null)
            {
                throw new Exception("Could not obtain Xunit extensions assembly reference path");
            }

            //Compilation = CSharpCompilation
            //    .Create("test")
            //    .AddReferences(new MetadataFileReference(Path.Combine(systemAsseemblyPath, "mscorlib.dll")))
            //    .AddReferences(new MetadataFileReference(Path.Combine(systemAsseemblyPath, "System.dll")))
            //    .AddReferences(new MetadataFileReference(Path.Combine(systemAsseemblyPath, "System.Core.dll")))
            //    .AddReferences(new MetadataFileReference(Path.Combine(nUnitAssemblyPath, "nunit.framework.dll")))
            //    .AddSyntaxTrees(SyntaxTree);

            //SemanticModel = Compilation.GetSemanticModel(SyntaxTree);

            var ws = new CustomWorkspace();

            var projectInfo = ProjectInfo.Create(
                    ProjectId.CreateNewId(),
                    version: VersionStamp.Default,
                    name: "TestProject",
                    assemblyName: "TestProject.dll",
                    language: LanguageNames.CSharp);
            

            var solutionInfo = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Default, projects: new[] { projectInfo });

            var solution = ws.AddSolution(solutionInfo);

            Document = solution.GetProject(projectInfo.Id)
                .AddMetadataReference(new MetadataFileReference(Path.Combine(systemAsseemblyPath, "mscorlib.dll")))
                .AddMetadataReference(new MetadataFileReference(Path.Combine(systemAsseemblyPath, "System.dll")))
                .AddMetadataReference(new MetadataFileReference(Path.Combine(systemAsseemblyPath, "System.Core.dll")))
                .AddMetadataReference(new MetadataFileReference(Path.Combine(nUnitAssemblyPath, "nunit.framework.dll")))
                .AddMetadataReference(new MetadataFileReference(Path.Combine(xUnitAssemblyPath, "xunit.core.dll")))
                .AddMetadataReference(new MetadataFileReference(Path.Combine(xUnitExtensionsAssemblyPath, "xunit.extensions.dll")))

                .AddDocument("code", SourceText.From(text));
        }
    }
}