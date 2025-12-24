using Mongoose.IDO;
using Mongoose.IDO.DataAccess;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using ue_JLI_RK_DataShare.Properties;

namespace ue_JLI_RK_DataShare
{
    [IDOExtensionClass("ue_JLI_RK_DataShare")]
    public class ue_JLI_RK_DataShare : IDOExtensionClass
    {
        [IDOMethod(MethodFlags.CustomLoad, "infobar")]
        public DataTable ue_JLI_RK_CLM_GetObjectList(string objectType,ref string infobar)
        {

            DataTable resultSet = new DataTable();
            // Add columns
            resultSet.Columns.Add("name", typeof(string));

            string query = string.Empty;
            DataTable dt_Resultset = new DataTable();

            try
            {
                query = Resources.ue_JLI_RK_CLM_GetObjectList;
                using (ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = query;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    db.AddCommandParameterWithValue(sqlCommand, "ObjectType", objectType);
                    IDataReader Resultset = sqlCommand.ExecuteReader();
                    dt_Resultset.Load(Resultset);
                    if (dt_Resultset != null && dt_Resultset.Rows.Count > 0)
                        return dt_Resultset;
                    else
                        return resultSet;
                }
            }
            catch (Exception ex)
            {
                infobar = ex.Message;
                return resultSet;
            }

        }//ue_JLI_RK_CLM_GetObjectList


        [IDOMethod(MethodFlags.None, "infobar")]
        public short ue_JLI_RK_CLM_GetObjectContent(string objectType, string objectName, ref object objectContent, ref string infobar)
        {
            string query = string.Empty;
            try
            {
                query = Resources.ue_JLI_RK_CLM_GetObjectContent;
                using (ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = query;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    db.AddCommandParameterWithValue(sqlCommand, "ObjectType", objectType);
                    db.AddCommandParameterWithValue(sqlCommand, "ObjectName", objectName);
                    objectContent = sqlCommand.ExecuteScalar();
                    //dt_Resultset.Load(Resultset);
                    //objectContent = string.Join("",dt_Resultset.Rows.Cast<DataRow>().SelectMany(r => r.ItemArray.Select(v => v.ToString())));

                }
            }
            catch (Exception ex)
            {
                infobar = ex.Message;
                
                return 0;
            }
            return 0;

        }//ue_JLI_RK_CLM_GetObjectContent





    }
}
