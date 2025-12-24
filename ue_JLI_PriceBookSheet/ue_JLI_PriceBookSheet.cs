using Mongoose.Core.Common;
using Mongoose.IDO;
using Mongoose.IDO.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ue_JLI_PriceBookSheet
{
    public class ue_JLI_PriceBookSheet : IDOExtensionClass
    {

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
                //createLog("ue_JLI_JLM_CorteSchedReport", "ue_JLI_CLM_Rpt_JLM_CorteSched", 41, ex.Message);
            }
        }

        public interface IIDOExtensionClass
        {
            void SetContext(Mongoose.IDO.IIDOExtensionClassContext context);
        }

        public DataTable ue_JLI_CLM_PriceBookSheet(string inpStatus = null, 
                                                   string inpPriceBook = null, 
                                                   string inpExclusive = null, 
                                                   string inpDataSetType = null, 
                                                   string inpItem = null)
        {

            //createLog("ue_JLI_PriceBookSheet", "ue_JLI_CLM_PriceBookSheet", 55, "Method Call");
            //createLog("ue_JLI_PriceBookSheet", "ue_JLI_CLM_PriceBookSheet", 55, "INP " + inpStatus+ inpPriceBook+ inpExclusive+ inpDataSetType);
            if (string.IsNullOrEmpty(inpStatus)) { inpStatus = "APD"; }
            if (string.IsNullOrEmpty(inpPriceBook)) { inpPriceBook = "0"; }
            if (string.IsNullOrEmpty(inpExclusive)) { inpExclusive = "0"; }
            if (string.IsNullOrEmpty(inpDataSetType)) { inpDataSetType = "C"; }
            

            string str = string.Empty;
            string itemFilter = string.Empty;
            string collectionFilter = string.Empty;
            DataTable dt_NoData = new DataTable();
            dt_NoData.Columns.Add("NoData", typeof(string));
            dt_NoData.Rows.Add("No Data Exist");

            if (inpDataSetType == "C")
            {
                if (string.IsNullOrEmpty(inpItem)) { collectionFilter = ""; }
                else { collectionFilter = $@" And collectionID Like '{inpItem}'"; }

                str = $@"
    DECLARE 
     @CollStatus            InfobarType = '{inpStatus}'
    ,@PriceBook				ListYesNoType = {inpPriceBook}
    ,@Exclusive				ListYesNoType = {inpExclusive}


DECLARE @CollectionID	Nvarchar(10)
 ,@Item					ItemType
 ,@Stat					Nvarchar(5)
 ,@RowNum				Int = 0

DECLARE @ResultTbl Table(
 RowNum			Int
,CollectionID	Nvarchar(10)
,stat			Nvarchar(5)
,Item			ItemType
)



DECLARE cursor_collection CURSOR
FOR 
	SELECT collectionID FROM ue_JLI_Collection 
    Where (( @CollStatus IS NOT NULL) AND (CHARINDEX(stat,@CollStatus) <> 0)) {collectionFilter}
    Order By collectionID
OPEN cursor_collection;
FETCH NEXT FROM cursor_collection INTO @CollectionID
WHILE @@FETCH_STATUS = 0
    BEGIN
	----------------------------------------------
        

		DECLARE cursor_ItemCollection CURSOR
		FOR 
			SELECT item	FROM ue_JLI_ItemCollection Where collectionID = @CollectionID
		OPEN cursor_ItemCollection;
		FETCH NEXT FROM cursor_ItemCollection INTO @Item
		WHILE @@FETCH_STATUS = 0
			BEGIN
			---------------------------------------
				Set @RowNum = @RowNum + 1
				Insert Into @ResultTbl(RowNum,CollectionID,Item)
				Select @RowNum,@CollectionID,@Item

				FETCH NEXT FROM cursor_ItemCollection INTO @Item
			---------------------------------------
			END;
		CLOSE cursor_ItemCollection;
		DEALLOCATE cursor_ItemCollection;


		DECLARE cursor_ItemCollAddPillow CURSOR
		FOR 
			SELECT item	FROM ue_JLI_ItemCollAddPillow_mst Where collectionID = @CollectionID And record_type = 'C'
		OPEN cursor_ItemCollAddPillow;
		FETCH NEXT FROM cursor_ItemCollAddPillow INTO @Item
		WHILE @@FETCH_STATUS = 0
			BEGIN
			---------------------------------------
				Set @RowNum = @RowNum + 1
				Insert Into @ResultTbl(RowNum,CollectionID,Item)
				Select @RowNum,@CollectionID,@Item

				FETCH NEXT FROM cursor_ItemCollAddPillow INTO @Item
			---------------------------------------
			END;
		CLOSE cursor_ItemCollAddPillow;
		DEALLOCATE cursor_ItemCollAddPillow;

	-----------------------------------------------
        FETCH NEXT FROM cursor_collection INTO @CollectionID
    END;
CLOSE cursor_collection;
DEALLOCATE cursor_collection;




IF OBJECT_ID('tempdb..#Result1') IS NOT NULL
    DROP TABLE #Result1;

SELECT item, [Accents], [Basic], [DL Accents], [Leather], 
       [Premium-A], [Premium-B], [Premium-C], [Restricted], 
       [Sun Basic], [Sun-A], [Sun-B], [Sun-C], [Tier-1], [Tier-2], 
	   [Tier-3], [Tier-4], [Tier-5], [Tier-6], [Tier-7]
INTO #Result1
FROM (
    SELECT item, fabric_grade, price
    FROM ue_JLI_ItemFabricPrc
    WHERE item IN (SELECT item FROM item_mst)
) AS src
PIVOT (
    MAX(price) FOR fabric_grade IN (
        [Accents], [Basic], [DL Accents], [Leather], 
        [Premium-A], [Premium-B], [Premium-C], [Restricted], 
        [Sun Basic], [Sun-A], [Sun-B], [Sun-C], [Tier-1], [Tier-2], [Tier-3]
		, [Tier-4], [Tier-5], [Tier-6], [Tier-7]
    )
) AS pvt;



Select 
 rt.RowNum
,rt.collectionID
,rt.item
,Coll.name
,Coll.Uf_CollectionGroup
,Coll.stat
,Coll.Uf_PriceBook
,Coll.Uf_Exclusive
,Coll.Uf_TossInserts
,Coll.Uf_Items_Pictured
,Coll.Uf_Fabrics_Pictured
,Coll.Uf_ToOrder
,Coll.Uf_Specs_Details
,Coll.Uf_Misc
,item.item
,item.description
,IRA.RAGroup
,IRA.RAGroupType
,IRA.Legs
,item.Uf_LegFinish
,IRA.Nails
,item.Uf_NailheadFinish
,IRA.Arms
,IRA.SewingTrim
,item.Uf_RABacksAttached
,item.Uf_RABacksLoose
,item.Uf_RABacksTight
,item.Uf_RABacksTufted
,item.Uf_RABacksButtonTufted
,item.Uf_RABacksReversible
,item.Uf_RABacksStandard
,item.Uf_RABacksPlumaplush
,item.Uf_RABacksSinuousSpringSuppoet
,item.Uf_RABacksWebbingSupport
,item.Uf_RASeatsAttached
,item.Uf_RASeatsLoose
,item.Uf_RASeatsTight
,item.Uf_RASeatsTufted
,item.Uf_RASeatsButtonTufted
,item.Uf_RASeatsReversible
,item.Uf_RASeatsStandard
,item.Uf_RASeatsPlumaplush
,item.Uf_RASeatsGelPlush
,item.Uf_RASeatsSuperPlush
,item.Uf_RASeatsChanneled
,item.Uf_RASeatsSinuousSpringSupport
,item.Uf_RASeatWebbingSupport
,item.Uf_RAFeaturesStorage
,item.Uf_RAFeaturesModular
,item.Uf_RAFeaturesSleeper
,item.Uf_RAFeaturesAlligatorClips
,item.Uf_RAFeaturesAntiSlipGlides
,item.Uf_RAFeaturesCenterSupportLeg
,item.Uf_RAFeaturesContrastStitching
,item.Uf_RAFeaturesHeavyThread
,item.Uf_RAFeaturesBrackets
,item.Uf_RAFeatures2SlotLTracket
,item.Uf_RAFeaturesPlasticConnector
,item.Uf_BedsStorageOption
,item.Uf_HeadboardAvailable
,item.Uf_BedSize
,item.Uf_Backs--IRA.Backs 
,item.Uf_Seats
,item.Uf_Pillow_Configuration
,item.Uf_PcsSeats
,item.Uf_PcsCubes
,item.unit_weight
,item.Uf_PcsLength
,item.Uf_PcsWidth
,item.Uf_PcsHeight
,item.Uf_HeightWCushions
,item.Uf_ArmHeight
,item.Uf_SeatHeight
,item.Uf_SeatDepth
,item.Uf_ArmWidth
,item.Uf_LegHeight
,item.Uf_InsideArmToArmLength
,item.Uf_RAWeightLimitPerSeat
,item.Uf_RABedWeightLimit
,item.Uf_RASleeperOpenLength
,item.Uf_RAMattressHeight
,item.Uf_RAMattressLength
,item.Uf_RAMattressWidth
,item.Uf_RAFrameCompFSCCertified
,item.Uf_RAFrameCompJLStandard
,item.Uf_Boxed
,item.Uf_BoxedItem
,BoxedItem.Uf_PcsLength
,BoxedItem.Uf_PcsWidth
,BoxedItem.Uf_PcsHeight
,item.stat
,item.Uf_ItemLifeCycleStatus
,item.Uf_RABacksTrillium			
,item.Uf_RABacksSerene
,item.Uf_RASeatsTrillium
,item.Uf_RASeatsSerene
--------------
,Result1.*
--------------
From @ResultTbl rt 
Inner Join ue_JLI_Collection Coll On Coll.collectionID = rt.CollectionID
Left Join item_mst item On item.item = rt.item
Left Join item_mst BoxedItem On BoxedItem.item = item.Uf_BoxedItem
Left Join ue_JLI_ItemRptAttributes IRA On IRA.item = item.item
Left Join #Result1 Result1 On Result1.item = item.item
Where (( @CollStatus  IS NOT NULL) AND (CHARINDEX(Coll.stat,@CollStatus) <> 0))
And ( IsNull(Coll.Uf_PriceBook,0) = Cast(@PriceBook As Nvarchar(1)) OR IsNull(Coll.Uf_Exclusive,0) = Cast(@Exclusive As Nvarchar(1)) )
Order By rt.RowNum



    ";
            }
            else
            {
                if (string.IsNullOrEmpty(inpItem)) { itemFilter = ""; }
                else { itemFilter = $@" And item Like '{inpItem}'"; }

                str = $@"
DECLARE
 @CollStatus            InfobarType = '{inpStatus}'
,@PriceBook				ListYesNoType = {inpPriceBook}
,@Exclusive				ListYesNoType = {inpExclusive}

DECLARE @Item			ItemType
 ,@Stat					Nvarchar(5)
 ,@CollectionItem		ItemType
 ,@RowNum				Int = 0

DECLARE @ResultTbl Table(
 RowNum				Int
,CollectionItem		ItemType
,Item				ItemType
)



DECLARE cursor_collection CURSOR
FOR 
	SELECT item FROM ue_JLI_ItemCollecAccessory_mst 
    Where (( @CollStatus  IS NOT NULL) AND (CHARINDEX(stat,@CollStatus) <> 0)) {itemFilter}
    Order By item
OPEN cursor_collection;
FETCH NEXT FROM cursor_collection INTO @CollectionItem
WHILE @@FETCH_STATUS = 0
    BEGIN
	----------------------------------------------  
		Set @RowNum = @RowNum + 1
		Insert Into @ResultTbl(RowNum,CollectionItem,Item)
		Select @RowNum,@CollectionItem,@CollectionItem

		DECLARE cursor_ItemCollAddPillow CURSOR
		FOR 
			SELECT item	FROM ue_JLI_ItemCollAddPillow_mst Where collectionID = @CollectionItem And record_type = 'A'
		OPEN cursor_ItemCollAddPillow;
		FETCH NEXT FROM cursor_ItemCollAddPillow INTO @Item
		WHILE @@FETCH_STATUS = 0
			BEGIN
			---------------------------------------
				Set @RowNum = @RowNum + 1
				Insert Into @ResultTbl(RowNum,CollectionItem,Item)
				Select @RowNum,@CollectionItem,@Item

				FETCH NEXT FROM cursor_ItemCollAddPillow INTO @Item
			---------------------------------------
			END;
		CLOSE cursor_ItemCollAddPillow;
		DEALLOCATE cursor_ItemCollAddPillow;

	-----------------------------------------------
        FETCH NEXT FROM cursor_collection INTO @CollectionItem
    END;
CLOSE cursor_collection;
DEALLOCATE cursor_collection;



IF OBJECT_ID('tempdb..#Result1') IS NOT NULL
    DROP TABLE #Result1;

SELECT item, [Accents], [Basic], [DL Accents], [Leather], 
       [Premium-A], [Premium-B], [Premium-C], [Restricted], 
       [Sun Basic], [Sun-A], [Sun-B], [Sun-C], [Tier-1], [Tier-2], 
	   [Tier-3], [Tier-4], [Tier-5], [Tier-6], [Tier-7]
INTO #Result1
FROM (
    SELECT item, fabric_grade, price
    FROM ue_JLI_ItemFabricPrc
    WHERE item IN (SELECT item FROM item_mst)
) AS src
PIVOT (
    MAX(price) FOR fabric_grade IN (
        [Accents], [Basic], [DL Accents], [Leather], 
        [Premium-A], [Premium-B], [Premium-C], [Restricted], 
        [Sun Basic], [Sun-A], [Sun-B], [Sun-C], [Tier-1], [Tier-2], [Tier-3]
		, [Tier-4], [Tier-5], [Tier-6], [Tier-7]
    )
) AS pvt;



Select 
 rt.RowNum
,rt.CollectionItem
,rt.item
,Coll.name
,Coll.assort_groups
,Coll.stat
,item.Uf_PriceBook
,item.Uf_Exclusive
,Coll.toss_inserts
,Null
,Coll.Fab_Pictured
,Coll.To_Order
,Coll.copy_details
,Null
,item.item
,item.description
,IRA.RAGroup
,IRA.RAGroupType
,IRA.Legs
,item.Uf_LegFinish
,IRA.Nails
,item.Uf_NailheadFinish
,IRA.Arms
,IRA.SewingTrim
,item.Uf_RABacksAttached
,item.Uf_RABacksLoose
,item.Uf_RABacksTight
,item.Uf_RABacksTufted
,item.Uf_RABacksButtonTufted
,item.Uf_RABacksReversible
,item.Uf_RABacksStandard
,item.Uf_RABacksPlumaplush
,item.Uf_RABacksSinuousSpringSuppoet
,item.Uf_RABacksWebbingSupport
,item.Uf_RASeatsAttached
,item.Uf_RASeatsLoose
,item.Uf_RASeatsTight
,item.Uf_RASeatsTufted
,item.Uf_RASeatsButtonTufted
,item.Uf_RASeatsReversible
,item.Uf_RASeatsStandard
,item.Uf_RASeatsPlumaplush
,item.Uf_RASeatsGelPlush
,item.Uf_RASeatsSuperPlush
,item.Uf_RASeatsChanneled
,item.Uf_RASeatsSinuousSpringSupport
,item.Uf_RASeatWebbingSupport
,item.Uf_RAFeaturesStorage
,item.Uf_RAFeaturesModular
,item.Uf_RAFeaturesSleeper
,item.Uf_RAFeaturesAlligatorClips
,item.Uf_RAFeaturesAntiSlipGlides
,item.Uf_RAFeaturesCenterSupportLeg
,item.Uf_RAFeaturesContrastStitching
,item.Uf_RAFeaturesHeavyThread
,item.Uf_RAFeaturesBrackets
,item.Uf_RAFeatures2SlotLTracket
,item.Uf_RAFeaturesPlasticConnector
,item.Uf_BedsStorageOption
,item.Uf_HeadboardAvailable
,item.Uf_BedSize
,item.Uf_Backs--IRA.Backs 
,item.Uf_Seats
,item.Uf_Pillow_Configuration
,item.Uf_PcsSeats
,item.Uf_PcsCubes
,item.unit_weight
,item.Uf_PcsLength
,item.Uf_PcsWidth
,item.Uf_PcsHeight
,item.Uf_HeightWCushions
,item.Uf_ArmHeight
,item.Uf_SeatHeight
,item.Uf_SeatDepth
,item.Uf_ArmWidth
,item.Uf_LegHeight
,item.Uf_InsideArmToArmLength
,item.Uf_RAWeightLimitPerSeat
,item.Uf_RABedWeightLimit
,item.Uf_RASleeperOpenLength
,item.Uf_RAMattressHeight
,item.Uf_RAMattressLength
,item.Uf_RAMattressWidth
,item.Uf_RAFrameCompFSCCertified
,item.Uf_RAFrameCompJLStandard
,item.Uf_Boxed
,item.Uf_BoxedItem
,BoxedItem.Uf_PcsLength
,BoxedItem.Uf_PcsWidth
,BoxedItem.Uf_PcsHeight
,item.stat
,item.Uf_ItemLifeCycleStatus
,item.Uf_RABacksTrillium			
,item.Uf_RABacksSerene
,item.Uf_RASeatsTrillium
,item.Uf_RASeatsSerene
--------------
,Result1.*
--------------
From @ResultTbl rt 
Inner Join ue_JLI_ItemCollecAccessory_mst Coll On Coll.item = rt.CollectionItem
Inner Join item_mst item On item.item = rt.item
Left Join item_mst BoxedItem On BoxedItem.item = item.Uf_BoxedItem
Left Join ue_JLI_ItemRptAttributes IRA On IRA.item = item.item
Left Join #Result1 Result1 On Result1.item = item.item
Where (( @CollStatus  IS NOT NULL) AND (CHARINDEX(Coll.stat,@CollStatus) <> 0))
And ( IsNull(item.Uf_PriceBook,0) = Cast(@PriceBook As Nvarchar(1)) OR IsNull(item.Uf_Exclusive,0) = Cast(@Exclusive As Nvarchar(1)) )
Order By rt.RowNum


    ";
            }
            DataTable dt_Resultset = new DataTable();

            try
            {
                using (Mongoose.IDO.DataAccess.ApplicationDB db = this.CreateApplicationDB())
                {
                    IDbCommand sqlCommand = db.CreateCommand();
                    sqlCommand.CommandText = str;
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    IDataReader Resultset = sqlCommand.ExecuteReader();
                    //createLog("ue_JLI_DailyOrderRecieptSummaryReport", "ue_JLI_Rpt_DailyOrderRecieptSummary", 56, "After ExecuteReader Call"); 
                    dt_Resultset.Load(Resultset);
                    if (dt_Resultset != null && dt_Resultset.Rows.Count > 0)
                    {
                        return dt_Resultset;
                    }
                    else
                    {
                        return dt_NoData;
                    }
                }
            }
            catch (Exception ex)
            {
                //createLog("ue_JLI_PriceBookSheet", "ue_JLI_CLM_PriceBookSheet", 273, "ex.Message" + ex.Message);
                return dt_NoData;
            }



        }
    }
}
