' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Composition
Imports Microsoft.CodeAnalysis.Host
Imports Microsoft.CodeAnalysis.Host.Mef
Imports Microsoft.CodeAnalysis.Navigation

Namespace Microsoft.CodeAnalysis.Editor.UnitTests.GoToDefinition
    <ExportWorkspaceServiceFactory(GetType(IDocumentNavigationService), ServiceLayer.Editor), [Shared]>
    Friend Class MockDocumentNavigationServiceFactory
        Implements IWorkspaceServiceFactory

        Public Function CreateService(workspaceServices As HostWorkspaceServices) As IWorkspaceService Implements IWorkspaceServiceFactory.CreateService
            Return New MockDocumentNavigationService()
        End Function
    End Class
End Namespace
