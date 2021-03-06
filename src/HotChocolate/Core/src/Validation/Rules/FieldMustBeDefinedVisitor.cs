﻿using HotChocolate.Language;
using HotChocolate.Language.Visitors;
using HotChocolate.Types;
using HotChocolate.Types.Introspection;

namespace HotChocolate.Validation
{
    /// <summary>
    /// The target field of a field selection must be defined on the scoped
    /// type of the selection set. There are no limitations on alias names.
    ///
    /// http://spec.graphql.org/June2018/#sec-Field-Selections-on-Objects-Interfaces-and-Unions-Types
    /// </summary>
    internal sealed class FieldMustBeDefinedVisitor : TypeDocumentValidatorVisitor
    {
        protected override ISyntaxVisitorAction Enter(
            SelectionSetNode node,
            IDocumentValidatorContext context)
        {
            if (context.Types.TryPeek(out IType type) &&
                type.NamedType().IsUnionType() &&
                HasFields(node))
            {
                context.Errors.Add(
                    ErrorBuilder.New()
                        .SetMessage(
                            "A union type cannot declare a field directly. " +
                            "Use inline fragments or fragments instead")
                        .AddLocation(node)
                        .SetPath(context.CreateErrorPath())
                        .SpecifiedBy("sec-Field-Selections-on-Objects-Interfaces-and-Unions-Types")
                        .Build());
                return Skip;
            }
            return Continue;
        }

        protected override ISyntaxVisitorAction Enter(
            FieldNode node,
            IDocumentValidatorContext context)
        {
            if (context.Types.Peek() is IComplexOutputType ct &&
                !FieldExists(ct, node.Name.Value))
            {
                context.Errors.Add(
                    ErrorBuilder.New()
                        .SetMessage(
                            $"The field `{node.Name.Value}` does not exist " +
                            $"on the type `{ct.Name}`.")
                        .AddLocation(node)
                        .SetPath(context.CreateErrorPath())
                        .SpecifiedBy("sec-Field-Selections-on-Objects-Interfaces-and-Unions-Types")
                        .Build());
                return Skip;
            }
            return Continue;
        }

        private static bool HasFields(SelectionSetNode selectionSet)
        {
            for (int i = 0; i < selectionSet.Selections.Count; i++)
            {
                ISelectionNode selection = selectionSet.Selections[i];
                if (selection.Kind == NodeKind.Field)
                {
                    if (!IsTypeNameField(((FieldNode)selection).Name.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool FieldExists(
            IComplexOutputType type,
            NameString fieldName)
        {
            if (IsTypeNameField(fieldName))
            {
                return true;
            }
            return type.Fields.ContainsField(fieldName);
        }

        private static bool IsTypeNameField(NameString fieldName)
        {
            return fieldName.Equals(IntrospectionFields.TypeName);
        }
    }
}
