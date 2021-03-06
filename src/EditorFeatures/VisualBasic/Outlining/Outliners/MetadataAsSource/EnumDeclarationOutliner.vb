' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Microsoft.CodeAnalysis.Editor.VisualBasic.Outlining.MetadataAsSource
    Class EnumDeclarationOutliner
        Inherits AbstractMetadataAsSourceOutliner(Of EnumStatementSyntax)

        Protected Overrides Function GetEndToken(node As EnumStatementSyntax) As SyntaxToken
            Return If(node.Modifiers.Count > 0,
                      node.Modifiers.First(),
                      node.EnumKeyword)
        End Function
    End Class
End Namespace
