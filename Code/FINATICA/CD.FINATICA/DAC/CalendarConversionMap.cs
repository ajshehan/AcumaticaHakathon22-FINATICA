using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.GL.FinPeriods;
using System;

namespace CD.FINATICA
{
    [Serializable]
    [PXHidden]
    public class CalendarConversionMap : IBqlTable
    {
        #region StartYearID
        [PXString(4, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Start Year")]
        [PXSelector(typeof(SelectFrom<MasterFinYear>
                                .OrderBy<MasterFinYear.year.Desc>
                                    .SearchFor<MasterFinYear.year>))]
        public virtual string StartYearID { get; set; }
        public abstract class startYearID : PX.Data.BQL.BqlString.Field<startYearID> { }
        #endregion

        #region StartPeriodNbr
        [PXString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Start Period Number")]
        [PXSelector(typeof(SelectFrom<MasterFinPeriod>
                                .Where<MasterFinPeriod.finYear.IsEqual<startYearID.FromCurrent>>
                                    .OrderBy<MasterFinPeriod.periodNbr.Desc>
                                        .SearchFor<MasterFinPeriod.periodNbr>),
                                typeof(MasterFinPeriod.periodNbr))]
        public virtual string StartPeriodNbr { get; set; }
        public abstract class startPeriodNbr : PX.Data.BQL.BqlString.Field<startPeriodNbr> { }
        #endregion


        #region CurrentStartPeriodID
        [PXString(6, IsFixed = true, InputMask = "")]
        [PXFormula(typeof(Add<startYearID, startPeriodNbr>))]
        public virtual string CurrentStartPeriodID { get; set; }
        public abstract class currentStartPeriodID : PX.Data.BQL.BqlString.Field<currentStartPeriodID> { }
        #endregion

        //#region Tstamp
        //[PXDBTimestamp()]
        //[PXUIField(DisplayName = "Tstamp")]
        //public virtual byte[] Tstamp { get; set; }
        //public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        //#endregion

        //#region CreatedByID
        //[PXDBCreatedByID()]
        //public virtual Guid? CreatedByID { get; set; }
        //public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        //#endregion

        //#region CreatedByScreenID
        //[PXDBCreatedByScreenID()]
        //public virtual string CreatedByScreenID { get; set; }
        //public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        //#endregion

        //#region CreatedDateTime
        //[PXDBCreatedDateTime()]
        //public virtual DateTime? CreatedDateTime { get; set; }
        //public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        //#endregion

        //#region LastModifiedByID
        //[PXDBLastModifiedByID()]
        //public virtual Guid? LastModifiedByID { get; set; }
        //public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        //#endregion

        //#region LastModifiedByScreenID
        //[PXDBLastModifiedByScreenID()]
        //public virtual string LastModifiedByScreenID { get; set; }
        //public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        //#endregion

        //#region LastModifiedDateTime
        //[PXDBLastModifiedDateTime()]
        //public virtual DateTime? LastModifiedDateTime { get; set; }
        //public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        //#endregion
    }
}