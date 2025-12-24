Imports Mongoose.IDO
Imports Mongoose.IDO.Protocol
Imports System.Runtime.Remoting.Contexts

Public Class ue_JLI_Catalog_Fabric_Maints
    Inherits IDOExtensionClass
    Sub JLI_LoadCollectionRequestData(ByVal CollectionName As String,
                                      ByVal Filter As String,
                                      ByVal PropertyList As List(Of String),
                                      ByVal RecordCap As Integer,
                                      ByVal Distinct As Boolean,
                                      ByVal OrderBy As String,
                                      ByRef loadResponse As LoadCollectionResponseData)

        Dim loadRequest As New LoadCollectionRequestData()
        Dim strFilter As String = Filter
        loadRequest.IDOName = CollectionName
        loadRequest.Filter = Filter
        loadRequest.OrderBy = OrderBy
        For Each PropertyName As String In PropertyList
            loadRequest.PropertyList.Add(PropertyName)
        Next
        loadRequest.RecordCap = RecordCap
        loadRequest.Distinct = Distinct
        ' Load collection of items based on the request
        loadResponse = Context.Commands.LoadCollection(loadRequest)

    End Sub
    Sub JLI_UpdateCollectionResponseData(ByVal CollectionName As String,
                                         ByVal Action As String,
                                         ByVal RowNum As Integer,
                                         ByVal PropertiesList As Dictionary(Of String, String),
                                         ByVal loadResponse As LoadCollectionResponseData,
                                         ByRef oResponseData As UpdateCollectionResponseData)

        Dim oRequestData As UpdateCollectionRequestData
        Dim oUpdateItem As IDOUpdateItem = New IDOUpdateItem()

        oResponseData = New UpdateCollectionResponseData()
        oRequestData = New UpdateCollectionRequestData(CollectionName)
        If Action = "Insert" Then
            oUpdateItem = New IDOUpdateItem(UpdateAction.Insert)
        ElseIf Action = "Update" Then
            oUpdateItem = New IDOUpdateItem(UpdateAction.Update, loadResponse.Items(RowNum).ItemID)
        ElseIf Action = "Delete" Then
            oUpdateItem = New IDOUpdateItem(UpdateAction.Delete, loadResponse.Items(RowNum).ItemID)
        End If
        If Action = "Insert" Or Action = "Update" Then
            For Each List As KeyValuePair(Of String, String) In PropertiesList
                oUpdateItem.Properties.Add(List.Key, List.Value)
            Next
        End If
        oRequestData.Items.Add(oUpdateItem)
        oResponseData = Context.Commands.UpdateCollection(oRequestData)

    End Sub

    Function ue_JLI_CatalogFabricMaints_DataInsertOrDelete(ByVal item As String,
                                                           ByVal status As String,
                                                           ByVal itmUf_Catalog_Fabric As String) As Integer

        Dim loadResponse As LoadCollectionResponseData = New LoadCollectionResponseData()
        Dim loadRequest As LoadCollectionRequestData
        Dim oResponseData As UpdateCollectionResponseData = New UpdateCollectionResponseData()
        Dim oRequestData As UpdateCollectionRequestData = New UpdateCollectionRequestData()
        Dim oUpdateItem As IDOUpdateItem = New IDOUpdateItem()
        Dim propertyList As New List(Of String)
        Dim PropertiesList As New Dictionary(Of String, String)()
        Dim strFilter As String = String.Empty
        Dim OrderBy As String = String.Empty

        propertyList.AddRange(New String() {"Item"})
        strFilter = String.Format("Item = '{0}'", item)
        OrderBy = ""
        'CollectionName, strFilter, propertyList, RecordCap, Distinct, OrderBy, loadResponse
        JLI_LoadCollectionRequestData("ue_JLI_Catalog_Fabric_Maints", strFilter, propertyList, 0, True, OrderBy, loadResponse)

        If loadResponse.Items.Count > 0 And itmUf_Catalog_Fabric = "0" Then

            Dim sFilter As String

            Dim oRequest As UpdateCollectionRequestData
            Dim oLoadResponse As LoadCollectionResponseData
            Dim oResponse As UpdateCollectionResponseData

            sFilter = String.Format("Item = '{0}'", item)
            oLoadResponse = Context.Commands.LoadCollection("ue_JLI_Catalog_Fabric_Maints", "Item", sFilter, "", 1)
            oRequest = New UpdateCollectionRequestData("ue_JLI_Catalog_Fabric_Maints")
            oUpdateItem = New IDOUpdateItem(UpdateAction.Delete)
            oUpdateItem.ItemID = oLoadResponse.Items(0).ItemID
            oRequest.Items.Add(oUpdateItem)
            oResponse = Context.Commands.UpdateCollection(oRequest)


            'JLI_UpdateCollectionResponseData("ue_JLI_Catalog_Fabric_Maints", "Delete", 0, PropertiesList, loadResponse, oResponseData)

        ElseIf loadResponse.Items.Count = 0 And itmUf_Catalog_Fabric = "1" Then

            PropertiesList.Add("Item", item)
            If status = "A" Then
                PropertiesList.Add("ShowFabric", "1")
            End If
            JLI_UpdateCollectionResponseData("ue_JLI_Catalog_Fabric_Maints", "Insert", 0, PropertiesList, loadResponse, oResponseData)

        End If

        ue_JLI_CatalogFabricMaints_DataInsertOrDelete = 0

    End Function


End Class
