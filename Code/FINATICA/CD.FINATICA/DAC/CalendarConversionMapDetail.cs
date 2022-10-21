using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.EP;
using PX.Objects.GL.FinPeriods;

namespace CD.FINATICA
{
    [Serializable]
    [PXCacheName("CalendarConversionMapDetail")]
    public class CalendarConversionMapDetail : IBqlTable
    {

        #region CurrentFinPeriodID
        [PXDBString(6, IsKey = true, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Current Fin Period ID", Enabled = false)]
        public virtual string CurrentFinPeriodID { get; set; }
        public abstract class currentFinPeriodID : PX.Data.BQL.BqlString.Field<currentFinPeriodID> { }
        #endregion

        #region CurrentFinYear
        [PXDBString(4, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Current Fin Year", Enabled = false)]
        public virtual string CurrentFinYear { get; set; }
        public abstract class currentFinYear : PX.Data.BQL.BqlString.Field<currentFinYear> { }
        #endregion

        #region CurrentStartDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Current Start Date", Enabled = false)]
        public virtual DateTime? CurrentStartDate { get; set; }
        public abstract class currentStartDate : PX.Data.BQL.BqlDateTime.Field<currentStartDate> { }
        #endregion

        #region CurrentEndDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Current End Date", Enabled = false)]
        public virtual DateTime? CurrentEndDate { get; set; }
        public abstract class currentEndDate : PX.Data.BQL.BqlDateTime.Field<currentEndDate> { }
        #endregion

        #region NewFinYear
        [PXDBString(4, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "New Fin Year")]
        [PXSelector(typeof(Search3<MasterFinYear.year, OrderBy<Desc<MasterFinYear.year>>>))]
        [PXFieldDescription]
        public virtual string NewFinYear { get; set; }
        public abstract class newFinYear : PX.Data.BQL.BqlString.Field<newFinYear> { }
        #endregion

        #region NewFinPeriodNbr
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "New Fin Period Nbr")]
        public virtual string NewFinPeriodNbr { get; set; }
        public abstract class newFinPeriodNbr : PX.Data.BQL.BqlInt.Field<newFinPeriodNbr> { }
        #endregion

        #region NewFinPeriodID
        [PXDBString(6, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "New Fin Period ID", Visible = false)]
        [PXFormula(typeof(Add<newFinYear, newFinPeriodNbr>))]
        public virtual string NewFinPeriodID { get; set; }
        public abstract class newFinPeriodID : PX.Data.BQL.BqlString.Field<newFinPeriodID> { }
        #endregion

        #region NewStartDate
        [PXDBDate()]
        [PXUIField(DisplayName = "New Start Date")]
        public virtual DateTime? NewStartDate { get; set; }
        public abstract class newStartDate : PX.Data.BQL.BqlDateTime.Field<newStartDate> { }
        #endregion

        #region NewEndDate
        [PXDBDate()]
        [PXUIField(DisplayName = "New End Date")]
        public virtual DateTime? NewEndDate { get; set; }
        public abstract class newEndDate : PX.Data.BQL.BqlDateTime.Field<newEndDate> { }
        #endregion

        #region IsAdjustment
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Adjustment Period", Enabled = false)]
        public virtual bool? IsAdjustment
        {
            [PXDependsOnFields(typeof(newStartDate), typeof(newEndDate))]
            get
            {
                return NewStartDate != null && NewEndDate != null && NewStartDate == NewEndDate;
            }
        }
        public abstract class isAdjustment : PX.Data.BQL.BqlBool.Field<isAdjustment> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion
    }
}