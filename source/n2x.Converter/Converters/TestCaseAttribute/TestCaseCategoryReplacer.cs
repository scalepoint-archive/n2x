﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using n2x.Converter.Generators;
using n2x.Converter.Utils;
using Xunit;

namespace n2x.Converter.Converters.TestCaseAttribute
{
    public class TestCaseCategoryReplacer : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            var methods = root.Classes().SelectMany(p => p.GetTestCaseMethods(semanticModel));

            foreach (var method in methods)
            {
                var testCaseAttributes = method.GetAttributes<NUnit.Framework.TestCaseAttribute>(semanticModel);
                var categoryArguments = testCaseAttributes
                    .Select(p => p.ArgumentList?.Arguments.SingleOrDefault(a => a.NameEquals != null && a.NameEquals.Name.Identifier.Text == "Category"))
                    .Where(p => p != null);

                if (!categoryArguments.Any())
                {
                    continue;
                }

                var newMethod = method;
                foreach (var categoryArg in categoryArguments)
                {
                    var key = SyntaxFactory.AttributeArgument(ExpressionGenerator.GenerateValueExpression("Category"));
                    var value = SyntaxFactory.AttributeArgument(categoryArg.Expression);

                    var traitAttrDeclaration = ExpressionGenerator.GenerateAttribute<TraitAttribute>(key, value);
                    newMethod = newMethod.AddAtribute(traitAttrDeclaration);
                }

                dict.Add(method, newMethod);
            }

            if (dict.Any())
            {
                return root.ReplaceNodes(dict.Keys, (n1, n2) => dict[n1]).NormalizeWhitespace();
            }

            return root;
        }
    }
}