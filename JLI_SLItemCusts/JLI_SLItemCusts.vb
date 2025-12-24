Imports Mongoose.IDO
Imports Mongoose.IDO.Protocol
Public Class JLI_SLItemCusts
    Inherits IDOExtensionClass
    'Public Sub CreateLog(ByVal Log_ClassName As String,
    '                           ByVal Log_MethodName As String,
    '                           ByVal Log_LineNumber As Integer,
    '                           ByVal Log_Details As String)

    '    Try
    '        If String.IsNullOrEmpty(Log_Details) Then
    '            Log_Details = "Error Details not Updated"
    '        End If

    '        Dim oResponseData As UpdateCollectionResponseData
    '        Dim oRequestData As UpdateCollectionRequestData
    '        Dim oUpdateItem As IDOUpdateItem
    '        oResponseData = New UpdateCollectionResponseData()
    '        oRequestData = New UpdateCollectionRequestData("ue_ZESHT_CustomAssemblyLogs") 'IDO Name
    '        oUpdateItem = New IDOUpdateItem(UpdateAction.Insert) 'Insert Or Update Or Delete


    '        oUpdateItem.Properties.Add("ClassName", Log_ClassName)
    '        oUpdateItem.Properties.Add("MethodName", Log_MethodName)
    '        oUpdateItem.Properties.Add("LineNumber", Log_LineNumber)
    '        oUpdateItem.Properties.Add("Comments", Log_Details)

    '        oRequestData.Items.Add(oUpdateItem)
    '        oResponseData = Context.Commands.UpdateCollection(oRequestData)

    '    Catch ex As Exception

    '    End Try

    'End Sub
    Public Function JLI_CustomerLineUpDataInsert(ByVal CustNum As String,
                                                 ByVal Item As String,
                                                 ByVal CustItemSeq As Integer) As Integer

        Try
            Dim oResponseData As UpdateCollectionResponseData = New UpdateCollectionResponseData()
            Dim oRequestData As UpdateCollectionRequestData = New UpdateCollectionRequestData("JLI_CustomerLineups")
            Dim oUpdateItem As IDOUpdateItem = New IDOUpdateItem(UpdateAction.Insert)

            oUpdateItem.Properties.Add("CustNum", CustNum)
            oUpdateItem.Properties.Add("CustItemSeq", CStr(CustItemSeq))
            oUpdateItem.Properties.Add("Item", Item)

            oRequestData.Items.Add(oUpdateItem)
            oResponseData = Context.Commands.UpdateCollection(oRequestData)

        Catch ex As Exception

        End Try

        JLI_CustomerLineUpDataInsert = 0

    End Function



End Class
