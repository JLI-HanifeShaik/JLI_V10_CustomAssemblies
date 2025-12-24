using Mongoose.IDO;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ue_JLI_JLM_PurchaseOrderReport
{
    public class ue_JLI_JLM_PurchaseOrderReport : IDOExtensionClass
    {
        public void JLI_LoadCollectionRequestData(string collectionName, string filter, List<string> propertyList, int recordCap, bool distinct, string orderBy, ref LoadCollectionResponseData loadResponse)
        {
            LoadCollectionRequestData loadRequest = new LoadCollectionRequestData();
            string strFilter = filter;

            loadRequest.IDOName = collectionName;
            loadRequest.Filter = filter;
            loadRequest.OrderBy = orderBy;

            foreach (string propertyName in propertyList)
            {
                loadRequest.PropertyList.Add(propertyName);
            }

            loadRequest.RecordCap = recordCap;
            loadRequest.Distinct = distinct;

            // Load collection of items based on the request
            loadResponse = Context.Commands.LoadCollection(loadRequest);
        }
        public void createLog(string Log_ClassName, string Log_MethodName, int Log_LineNumber, string Log_Details)
        {
            try
            {
                if (string.IsNullOrEmpty(Log_Details))
                {
                    Log_Details = "Error Details not Updated";
                }

                UpdateCollectionResponseData oResponseData;
                UpdateCollectionRequestData oRequestData;
                IDOUpdateItem oUpdateItem;

                oResponseData = new UpdateCollectionResponseData();
                oRequestData = new UpdateCollectionRequestData("ue_ZESHT_CustomAssemblyLogs"); // IDO Name
                oUpdateItem = new IDOUpdateItem(UpdateAction.Insert); // Insert Or Update Or Delete

                oUpdateItem.Properties.Add("ClassName", Log_ClassName);
                oUpdateItem.Properties.Add("MethodName", Log_MethodName);
                oUpdateItem.Properties.Add("LineNumber", Log_LineNumber);
                oUpdateItem.Properties.Add("Comments", Log_Details);

                oRequestData.Items.Add(oUpdateItem);
                oResponseData = Context.Commands.UpdateCollection(oRequestData);
            }
            catch (Exception ex)
            {
                // Optionally, log the exception or handle it accordingly
            }
        }

        public DataTable ue_CLM_JLI_JLM_PurchaseOrderReport(string inpPoNum = null)
        {
            //createLog("ue_JLI_JLM_PurchaseOrderReport", "ue_CLM_JLI_JLM_PurchaseOrderReport", 68, "Method Calling");
            //createLog("ue_JLI_JLM_PurchaseOrderReport", "ue_CLM_JLI_JLM_PurchaseOrderReport", 68, "Method Calling New Line");

            // Create a new DataTable
            DataTable resultset = new DataTable("resultset");

            // Add columns to the DataTable with appropriate data types
            resultset.Columns.Add("UbPoNum", typeof(string));  // C-11
            resultset.Columns.Add("UbVendNum", typeof(string));  // C-8
            resultset.Columns.Add("UbShipTo", typeof(string));  // Assuming WhseType is string
            resultset.Columns.Add("UbWhse", typeof(string));  // Assuming WhseType is string
            resultset.Columns.Add("UbOrderDate", typeof(string));  // DateType
            resultset.Columns.Add("UbReqDate", typeof(string));  // DateType

            resultset.Columns.Add("UbVendName", typeof(string));  // NameType
            resultset.Columns.Add("UbVendAddr1", typeof(string));  // AddressType
            resultset.Columns.Add("UbVendCity", typeof(string));  // CityType
            resultset.Columns.Add("UbVendZip", typeof(string));  // PostalCodeType
            resultset.Columns.Add("UbVendCountry", typeof(string));  // CountryType
            resultset.Columns.Add("UbVendContactPerson", typeof(string));  // NameType
            resultset.Columns.Add("UbVendOfficePhone", typeof(string));  // PhoneType
            resultset.Columns.Add("UbVendEmail", typeof(string));  // EmailType
            resultset.Columns.Add("UbVendRFC", typeof(string));  // C-50

            resultset.Columns.Add("UbWhseName", typeof(string));  // NameType
            resultset.Columns.Add("UbWhseAddr_1", typeof(string));  // AddressType
            resultset.Columns.Add("UbWhseCity", typeof(string));  // CityType
            resultset.Columns.Add("UbWhseState", typeof(string));  // StateType
            resultset.Columns.Add("UbWhseZip", typeof(string));  // PostalCodeType
            resultset.Columns.Add("UbWhseCountry", typeof(string));  // CountryType
            resultset.Columns.Add("UbWhseContact", typeof(string));  // ContactType
            resultset.Columns.Add("UbWhsePhone", typeof(string));  // PhoneType
            resultset.Columns.Add("UbWhseFaxNum", typeof(string));  // PhoneType
            resultset.Columns.Add("UbWhseReceiverName", typeof(string));  // NameType
            resultset.Columns.Add("UbWhseReceiverEmail", typeof(string));  // EmailType

            resultset.Columns.Add("UbTax", typeof(string));  // C-1
            resultset.Columns.Add("UbPoLine", typeof(int));  // PoLineType
            resultset.Columns.Add("UbItem", typeof(string));  // ItemType
            resultset.Columns.Add("UbDescription", typeof(string));  // DescriptionType
            resultset.Columns.Add("UbQty", typeof(string));  // QtyPerType
            resultset.Columns.Add("UbUM", typeof(string));  // UMType
            resultset.Columns.Add("UbCurrency", typeof(string));  // C-10
            resultset.Columns.Add("UbUnitPrice", typeof(string));  // AmountType
            resultset.Columns.Add("UbExt", typeof(string));  // AmountType
            resultset.Columns.Add("UbGL_AccountNum", typeof(string));  // C-30
            resultset.Columns.Add("UbDepartment", typeof(string));  // DeptType
            resultset.Columns.Add("UbDerDeptName", typeof(string));  // DescriptionType
            resultset.Columns.Add("UbOrderBy", typeof(string));  // UserNameType
            resultset.Columns.Add("UbApprovedBy", typeof(string));  // UserNameType
            resultset.Columns.Add("UbAmount", typeof(string));  // AmountType
            resultset.Columns.Add("UbDerSubTotal", typeof(string));  // AmountType
            resultset.Columns.Add("UbDerDiscountAmount", typeof(string));  // AmountType
            resultset.Columns.Add("UbDerTaxAmount", typeof(string));  // AmountType
            resultset.Columns.Add("UbDerTotalAmount", typeof(string));  // AmountType
            resultset.Columns.Add("UbDerAccountNumDesc", typeof(string));  // DescriptionType
            resultset.Columns.Add("UbProject", typeof(string));  // C-35
            resultset.Columns.Add("UbReference", typeof(string));  // C-35
            resultset.Columns.Add("UbVendPartNum", typeof(string));  // C-15
            resultset.Columns.Add("UbIncomeTaxWithholding", typeof(string));  // 



            LoadCollectionResponseData loadResponse1 = new LoadCollectionResponseData();
            LoadCollectionResponseData loadResponse2 = new LoadCollectionResponseData();
            List<string> propertyList = new List<string>();
            string strFilter = string.Empty;
            string vendNum = string.Empty;
            string whse = string.Empty;
            string shipTo = string.Empty;
            string orderDate = string.Empty;
            string reqDate = string.Empty;
            string derSubTotal = string.Empty;
            string derDiscountAmount = string.Empty;
            string derTaxAmount = string.Empty;
            string derTotalAmount = string.Empty;
            string incomeTaxWithholding = string.Empty;
            
            string approvedBy = string.Empty;

            try
            {
                loadResponse2 = new LoadCollectionResponseData();
                propertyList.Clear();
                propertyList.AddRange(new string[] { "VendNum", "Whse", "ShipTo", "OrderDate", "ReqDate", "DerSubTotal", "DerDiscountAmount", "DerTaxAmount", "DerTotalAmount", "ApprovedBy", "IncomeTaxWithholding" });
                strFilter = string.Format("PoNum = '{0}'", inpPoNum);
                //collectionName,filter,propertyList,recordCap,distinct,orderBy,loadResponse
                JLI_LoadCollectionRequestData("ue_JLI_JLM_PUR_PoHdrs", strFilter, propertyList, 0, true, "", ref loadResponse2);

                if (loadResponse2.Items.Count > 0)
                {
                    for (int i = 0; i < loadResponse2.Items.Count; i++)
                    {
                        vendNum = loadResponse2[i, "VendNum"].Value;
                        whse = loadResponse2[i, "Whse"].Value;
                        shipTo = loadResponse2[i, "ShipTo"].Value;
                        orderDate = loadResponse2[i, "OrderDate"].Value;
                        reqDate = loadResponse2[i, "ReqDate"].Value;
                        derSubTotal = loadResponse2[i, "DerSubTotal"].Value;
                        derDiscountAmount = loadResponse2[i, "DerDiscountAmount"].Value;
                        derTaxAmount = loadResponse2[i, "DerTaxAmount"].Value;
                        derTotalAmount = loadResponse2[i, "DerTotalAmount"].Value;
                        approvedBy = loadResponse2[i, "ApprovedBy"].Value;
                        incomeTaxWithholding = loadResponse2[i, "IncomeTaxWithholding"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_JLM_PurchaseOrderReport", "ue_CLM_JLI_JLM_PurchaseOrderReport", 153, "Exception1 "+ ex.Message);
            }



            string vendName = string.Empty;
            string vendAddr1 = string.Empty;
            string vendAddr2 = string.Empty;
            string vendCity = string.Empty;
            string vendZip = string.Empty;
            string vendCountry = string.Empty;
            string vendContactPerson = string.Empty;
            string vendOfficePhone = string.Empty;
            string vendEmail = string.Empty;
            string vendRFC = string.Empty;

            try
            {
                loadResponse2 = new LoadCollectionResponseData();
                propertyList.Clear();
                propertyList.AddRange(new string[] { "Name", "Addr1", "Addr2", "City", "Zip", "Country", "ContactPerson", "OfficePhone", "Email", "RFC" });
                strFilter = string.Format("JLM_VendNum = '{0}'", vendNum);
                //collectionName,filter,propertyList,recordCap,distinct,orderBy,loadResponse
                JLI_LoadCollectionRequestData("ue_JLI_JLM_PUR_Vendors", strFilter, propertyList, 0, false, "", ref loadResponse2);

                if (loadResponse2.Items.Count > 0)
                {
                    for (int i = 0; i < loadResponse2.Items.Count; i++)
                    {
                        vendName = loadResponse2[i, "Name"].Value;
                        vendAddr1 = loadResponse2[i, "Addr1"].Value;
                        vendAddr2 = loadResponse2[i, "Addr2"].Value;
                        vendCity = loadResponse2[i, "City"].Value;
                        vendZip = loadResponse2[i, "Zip"].Value;
                        vendCountry = loadResponse2[i, "Country"].Value;
                        vendContactPerson = loadResponse2[i, "ContactPerson"].Value;
                        vendOfficePhone = loadResponse2[i, "OfficePhone"].Value;
                        vendEmail = loadResponse2[i, "Email"].Value;
                        vendRFC = loadResponse2[i, "RFC"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_JLM_PurchaseOrderReport", "ue_CLM_JLI_JLM_PurchaseOrderReport", 219, "Exception2 " + ex.Message);
            }


            string whseName = string.Empty;
            string whseAddr_1 = string.Empty;
            string whseAddr_2 = string.Empty;
            string whseCity = string.Empty;
            string whseState = string.Empty;
            string whseZip = string.Empty;
            string whseCountry = string.Empty;
            string whseContact = string.Empty;
            string whsePhone = string.Empty;
            string whseFaxNum = string.Empty;

            try
            {
                loadResponse2 = new LoadCollectionResponseData();
                propertyList.Clear();
                propertyList.AddRange(new string[] { "Name", "Addr_1", "Addr_2", "City", "State", "Zip", "Country", "Contact", "Phone", "FaxNum" });
                strFilter = string.Format("Whse = '{0}'", shipTo);
                //collectionName,filter,propertyList,recordCap,distinct,orderBy,loadResponse
                JLI_LoadCollectionRequestData("SLWhses", strFilter, propertyList, 0, false, "", ref loadResponse2);

                if (loadResponse2.Items.Count > 0)
                {
                    for (int i = 0; i < loadResponse2.Items.Count; i++)
                    {
                        whseName = loadResponse2[i, "Name"].Value;
                        whseAddr_1 = loadResponse2[i, "Addr_1"].Value;
                        whseAddr_2 = loadResponse2[i, "Addr_2"].Value;
                        whseCity = loadResponse2[i, "City"].Value;
                        whseState = loadResponse2[i, "State"].Value;
                        whseZip = loadResponse2[i, "Zip"].Value;
                        whseCountry = loadResponse2[i, "Country"].Value;
                        whseContact = loadResponse2[i, "Contact"].Value;
                        whsePhone = loadResponse2[i, "Phone"].Value;
                        whseFaxNum = loadResponse2[i, "FaxNum"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_JLM_PurchaseOrderReport", "ue_CLM_JLI_JLM_PurchaseOrderReport", 260, "Exception3 " + ex.Message);
            }


            string whseReceiverName = string.Empty;
            string whseReceiverEmail = string.Empty;

            try
            {
                loadResponse2 = new LoadCollectionResponseData();
                propertyList.Clear();
                propertyList.AddRange(new string[] { "Name", "Email" });
                strFilter = string.Format("Whse = '{0}' And Designation = 'Receving'", shipTo);
                //collectionName,filter,propertyList,recordCap,distinct,orderBy,loadResponse
                JLI_LoadCollectionRequestData("ue_JLI_JLM_Whse_Contacts", strFilter, propertyList, 1, false, "", ref loadResponse2);

                if (loadResponse2.Items.Count > 0)
                {
                    for (int i = 0; i < loadResponse2.Items.Count; i++)
                    {
                        whseReceiverName = loadResponse2[i, "Name"].Value;
                        whseReceiverEmail = loadResponse2[i, "Email"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_JLM_PurchaseOrderReport", "ue_CLM_JLI_JLM_PurchaseOrderReport", 287, "Exception4 " + ex.Message);
            }


            string tax = string.Empty;
            string poLine = string.Empty;
            string item = string.Empty;
            string description = string.Empty;
            string qty = string.Empty;
            string um = string.Empty;
            string currency = string.Empty;
            string unitPrice = string.Empty;
            string ext = string.Empty;
            string accountNum = string.Empty;
            string department = string.Empty;
            string derDeptNam = string.Empty;
            string orderBy = string.Empty;            
            string amount = string.Empty;
            string derAccountNumDesc = string.Empty;
            string project = string.Empty;
            string reference = string.Empty;
            string vendPartNum = string.Empty;


            try
            {
                loadResponse2 = new LoadCollectionResponseData();
                propertyList.Clear();
                propertyList.AddRange(new string[] { "Tax", "PoLine", "Item", "Description", "Qty", "UM", "Currency", "UnitPrice", "GL_AccountNum", "Department", "DerDeptName", "OrderBy", "ApprovedBy", "Amount", "DerAccountNumDesc", "Project", "Reference", "VendPartNum" });
                strFilter = string.Format("PoNum = '{0}'", inpPoNum);
                //collectionName,filter,propertyList,recordCap,distinct,orderBy,loadResponse
                JLI_LoadCollectionRequestData("ue_JLI_JLM_PUR_PoItems", strFilter, propertyList, 0, false, "", ref loadResponse2);

                if (loadResponse2.Items.Count > 0)
                {
                    for (int i = 0; i < loadResponse2.Items.Count; i++)
                    {
                        tax = loadResponse2[i, "Tax"].Value;
                        if(tax == "1") { tax = "Y"; }else { tax = "N"; }
                        poLine = loadResponse2[i, "PoLine"].Value;
                        item = loadResponse2[i, "Item"].Value;
                        description = loadResponse2[i, "Description"].Value;
                        qty = loadResponse2[i, "Qty"].Value;
                        um = loadResponse2[i, "UM"].Value;
                        currency = loadResponse2[i, "Currency"].Value;
                        unitPrice = loadResponse2[i, "UnitPrice"].Value;
                        amount = loadResponse2[i, "Amount"].Value;
                        accountNum = loadResponse2[i, "GL_AccountNum"].Value;
                        department = loadResponse2[i, "Department"].Value;
                        derDeptNam = loadResponse2[i, "DerDeptName"].Value;
                        orderBy = loadResponse2[i, "OrderBy"].Value;                    
                        derAccountNumDesc = loadResponse2[i, "DerAccountNumDesc"].Value;
                        project = loadResponse2[i, "Project"].Value;
                        reference = loadResponse2[i, "Reference"].Value;
                        vendPartNum = loadResponse2[i, "VendPartNum"].Value;




                        DataRow row = resultset.NewRow();
                        row["UbPoNum"] = inpPoNum;
                        row["UbVendNum"] = vendNum;
                        row["UbShipTo"] = shipTo;
                        row["UbWhse"] = whse;
                        row["UbOrderDate"] = orderDate;
                        row["UbReqDate"] = reqDate;
                        row["UbVendName"] = vendName;
                        row["UbVendAddr1"] = vendAddr1 + Environment.NewLine + vendAddr2;
                        row["UbVendCity"] = vendCity;
                        row["UbVendZip"] = vendZip;
                        row["UbVendCountry"] = vendCountry;
                        row["UbVendContactPerson"] = vendContactPerson;
                        row["UbVendOfficePhone"] = vendOfficePhone;
                        row["UbVendEmail"] = vendEmail;
                        row["UbVendRFC"] = vendRFC;
                        row["UbWhseName"] = whseName;
                        row["UbWhseAddr_1"] = whseAddr_1 + Environment.NewLine + whseAddr_2;
                        row["UbWhseCity"] = whseCity;
                        row["UbWhseState"] = whseState;
                        row["UbWhseZip"] = whseZip;
                        row["UbWhseCountry"] = whseCountry;
                        row["UbWhseContact"] = whseContact;
                        row["UbWhsePhone"] = whsePhone;
                        row["UbWhseFaxNum"] = whseFaxNum;
                        row["UbWhseReceiverName"] = whseReceiverName;
                        row["UbWhseReceiverEmail"] = whseReceiverEmail;
                        row["UbIncomeTaxWithholding"] = incomeTaxWithholding;


                        row["UbTax"] = tax;
                        row["UbPoLine"] = poLine;
                        row["UbItem"] = item;
                        row["UbDescription"] = description;
                        row["UbQty"] = qty;
                        row["UbUM"] = um;
                        row["UbCurrency"] = currency;
                        row["UbUnitPrice"] = unitPrice;
                        row["UbExt"] = ext;
                        row["UbGL_AccountNum"] = accountNum;
                        row["UbDepartment"] = department;
                        row["UbDerDeptName"] = derDeptNam;
                        row["UbOrderBy"] = orderBy;
                        row["UbApprovedBy"] = approvedBy;
                        row["UbAmount"] = amount;
                        row["UbDerSubTotal"] = derSubTotal;
                        row["UbDerDiscountAmount"] = derDiscountAmount;
                        row["UbDerTaxAmount"] = derTaxAmount;
                        row["UbDerTotalAmount"] = derTotalAmount;
                        row["UbDerAccountNumDesc"] = derAccountNumDesc;
                        row["UbProject"] = project;
                        row["UbReference"] = reference;
                        row["UbVendPartNum"] = vendPartNum;



                        resultset.Rows.Add(row);

                    }
                }
                else
                {

                    DataRow row = resultset.NewRow();
                    row["UbPoNum"] = inpPoNum;
                    row["UbVendNum"] = vendNum;
                    row["UbShipTo"] = shipTo;
                    row["UbWhse"] = whse;
                    row["UbOrderDate"] = orderDate;
                    row["UbReqDate"] = reqDate;
                    row["UbApprovedBy"] = approvedBy;
                    row["UbVendName"] = vendName;
                    row["UbVendAddr1"] = vendAddr1;
                    row["UbVendCity"] = vendCity;
                    row["UbVendZip"] = vendZip;
                    row["UbVendCountry"] = vendCountry;
                    row["UbVendContactPerson"] = vendContactPerson;
                    row["UbVendOfficePhone"] = vendOfficePhone;
                    row["UbVendEmail"] = vendEmail;
                    row["UbVendRFC"] = vendRFC;
                    row["UbWhseName"] = whseName;
                    row["UbWhseAddr_1"] = whseAddr_1;
                    row["UbWhseCity"] = whseCity;
                    row["UbWhseState"] = whseState;
                    row["UbWhseZip"] = whseZip;
                    row["UbWhseCountry"] = whseCountry;
                    row["UbWhseContact"] = whseContact;
                    row["UbWhsePhone"] = whsePhone;
                    row["UbWhseFaxNum"] = whseFaxNum;
                    row["UbWhseReceiverName"] = whseReceiverName;
                    row["UbWhseReceiverEmail"] = whseReceiverEmail;
                    row["UbIncomeTaxWithholding"] = incomeTaxWithholding;
                

                    resultset.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_JLM_PurchaseOrderReport", "ue_CLM_JLI_JLM_PurchaseOrderReport", 445, "Exception5 " + ex.Message);
            }






            return resultset;
        }

    }
}
