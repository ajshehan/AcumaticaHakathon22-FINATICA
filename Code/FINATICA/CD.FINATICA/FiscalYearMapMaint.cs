using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.Update;
using PX.Objects.GL;
using PX.Objects.GL.FinPeriods;
using PX.Objects.GL.FinPeriods.TableDefinition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CD.FINATICA
{
    public class FiscalYearMapMaint : PXGraph<FiscalYearMapMaint>
    {
        public PXSave<CalendarConversionMapDetail> Save;
        public PXCancel<CalendarConversionMapDetail> Cancel;

        public PXFilter<CalendarConversionMap> CalendarConversionMap;
        public SelectFrom<CalendarConversionMapDetail>.View CalendarConversionMapDetails;

        //public IEnumerable calendarConversionMapDetails()
        //{
        //    if (string.IsNullOrEmpty(CalendarConversionMap.Current.CurrentStartPeriodID)) yield return null;

        //    CalendarConversionMapDetails.Cache.Clear();

        //    var results = SelectFrom<FinPeriod>
        //                        .Where<FinPeriod.finPeriodID.IsGreaterEqual<@P.AsString>.And<FinPeriod.active.IsEqual<True>>>
        //                        .View
        //                        .Select(this, CalendarConversionMap.Current.CurrentStartPeriodID);

        //    foreach (FinPeriod period in results)
        //    {
        //        var cci = new CalendarConversionMapDetail
        //        {
        //            CurrentFinPeriodID = period.FinPeriodID,
        //            CurrentFinYear = period.FinYear,
        //            CurrentStartDate = period.StartDate,
        //            CurrentEndDate = period.EndDate
        //        };

        //        CalendarConversionMapDetails.Cache.SetStatus(cci, PXEntryStatus.Held);
        //        yield return cci;
        //    }
        // }

        public PXAction<CalendarConversionMapDetail> processMapping;
        [PXButton(CommitChanges = true), PXUIField(DisplayName = "Process Mapping",
            MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable ProcessMapping(PXAdapter adapter)
        {
            var clone = this.Clone();
            PXLongOperation.StartOperation(this, delegate ()
            {
                clone.DoProcessingMapping();
            });

            return adapter.Get();
        }

        private void DoProcessingMapping()
        {
            List<(int companyId, string cYear, DateTime? cStartDate, DateTime? cEndDate, string cPeriodId, string nYear, DateTime? nStartDate, DateTime? nEndDate, string nPeriodId)> mappingInfo
                    = new List<(int companyId, string cYear, DateTime? cStartDate, DateTime? cEndDate, string cPeriodId, string nYear, DateTime? nStartDate, DateTime? nEndDate, string nPeriodId)>();

            foreach (CalendarConversionMapDetail mapping in CalendarConversionMapDetails.Select())
            {
                mappingInfo.Add((PXInstanceHelper.CurrentCompany,
                                        mapping.CurrentFinYear,
                                        mapping.CurrentStartDate,
                                        mapping.CurrentEndDate,
                                        mapping.CurrentFinPeriodID,
                                        mapping.NewFinYear,
                                        mapping.NewStartDate,
                                        mapping.NewEndDate,
                                        mapping.NewFinPeriodID));
            }

            var yearList = mappingInfo.OrderByDescending(e => e.cYear).ThenByDescending(e => e.nPeriodId).GroupBy(c => c.cYear).Distinct().ToList();
            var periodList = mappingInfo.OrderByDescending(e => e.nYear).ToList();

            FiscalYearMapMaint graph = PXGraph.CreateInstance<FiscalYearMapMaint>();
            PXLongOperation.StartOperation(this, delegate ()
            {
                using (new PXConnectionScope())
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        int errorNbr = 0;
                        string errorMsg = "";

                        foreach (var item in yearList)
                        {
                            graph.ExecuteDropFinYearPKSP();

                            graph.ExecuteChangeFinYearSP(int.Parse(item.ElementAt(0).cYear),
                                            int.Parse(item.ElementAt(0).nYear),
                                            item.ElementAt(0).cStartDate.Value,
                                            item.ElementAt(0).nStartDate.Value,
                                            item.ElementAt(0).cEndDate.Value,
                                            item.ElementAt(0).nEndDate.Value,
                                            int.Parse(item.ElementAt(0).nYear + item.ElementAt(0).nPeriodId));

                            graph.ExecuteAddFinYearPKSP();

                            graph.ExecuteDropFinPeriodPKSP();

                            graph.ExecuteChangeFinPeriodSP(int.Parse(item.ElementAt(0).cYear),
                                            int.Parse(item.ElementAt(0).nYear),
                                            item.ElementAt(0).cStartDate.Value,
                                            item.ElementAt(0).nStartDate.Value,
                                            item.ElementAt(0).cEndDate.Value,
                                            item.ElementAt(0).nEndDate.Value,
                                            int.Parse(item.ElementAt(0).cPeriodId),
                                            int.Parse(item.ElementAt(0).nYear + item.ElementAt(0).nPeriodId));

                            graph.ExecuteChangeFinancialPeriodsSP(int.Parse(item.ElementAt(0).cPeriodId),
                                int.Parse(item.ElementAt(0).nPeriodId),
                                item.ElementAt(0).cStartDate.Value,
                                item.ElementAt(0).cEndDate.Value);

                            graph.ExecuteAddFinPeriodPKSP();

                            graph.ExecuteDeleteGLHistorySP(int.Parse(item.ElementAt(0).cPeriodId));


                        }

                        ts.Complete();
                    }
                }
            });
        }

        private bool ExecuteDropFinYearPKSP()
        {
            return ExecuteSP("ft_dropFinYearPK", new PXSPInParameter("companyID", 2), new PXSPInParameter("username", "ddd"));
        }

        private bool ExecuteChangeFinYearSP(int cYear, int nYear, DateTime cStartDate, DateTime nStartDate, DateTime cEndDate,
            DateTime nEndDate, int nStartMasterFinPeriodID)
        {
            //ft_changeFinYear
            return ExecuteSP("ft_changeFinYear",
                                new PXSPInParameter("companyID", 2),
                                new PXSPInParameter("cYear", cYear),
                                new PXSPInParameter("nYear", nYear),
                                new PXSPInParameter("cStartDate", cStartDate),
                                new PXSPInParameter("nStartDate", nStartDate),
                                new PXSPInParameter("cEndDate", cEndDate),
                                new PXSPInParameter("nEndDate", nEndDate),
                                new PXSPInParameter("nStartMasterFinPeriodID", nStartMasterFinPeriodID));
        }

        private bool ExecuteAddFinYearPKSP()
        {
            //ft_addFinYearPK
            return ExecuteSP("ft_addFinYearPK");
        }
        private bool ExecuteDropFinPeriodPKSP()
        {
            //ft_dropFinPeriodPK
            return ExecuteSP("ft_dropFinPeriodPK");
        }

        private bool ExecuteChangeFinPeriodSP(int cYear, int nYear, DateTime cStartDate, DateTime nStartDate, DateTime cEndDate, DateTime nEndDate,
            int cFinPeriodID, int nFinPeriodID)
        {
            //ft_changeFinPeriod
            return ExecuteSP("ft_changeFinPeriod",
                                new PXSPInParameter("companyID", 2),
                                new PXSPInParameter("cYear", cYear),
                                new PXSPInParameter("nYear", nYear),
                                new PXSPInParameter("cStartDate", cStartDate),
                                new PXSPInParameter("nStartDate", nStartDate),
                                new PXSPInParameter("cEndDate", cEndDate),
                                new PXSPInParameter("nEndDate", nEndDate),
                                new PXSPInParameter("cFinPeriodID", cFinPeriodID),
                                new PXSPInParameter("nFinPeriodID", nFinPeriodID));
        }

        private bool ExecuteAddFinPeriodPKSP()
        {
            //ft_addFinPeriodPK
            return ExecuteSP("ft_addFinPeriodPK");
        }

        private bool ExecuteDeleteGLHistorySP(int cFinPeriodID)
        {
            //ft_deleteGLHistory
            return ExecuteSP("ft_deleteGLHistory", new PXSPInParameter("cFinPeriodID", cFinPeriodID));
        }
        private bool ExecuteChangeFinancialPeriodsSP(int cFinPeriodID, int nFinPeriodID, DateTime cStartDate, DateTime cEndDate)
        {
            //ft_changeFinancialPeriods
            return ExecuteSP("ft_changeFinancialPeriods",
                                               new PXSPInParameter("companyID", 2),
                                               new PXSPInParameter("cFinPeriodID", cFinPeriodID),
                                               new PXSPInParameter("nFinPeriodID", nFinPeriodID),
                                               new PXSPInParameter("cStartDate", cStartDate),
                                               new PXSPInParameter("cEndDate", cEndDate));
        }

        private bool ExecuteSP(string spName, params PXSPParameter[] pars)
        {
            string errorNbr = string.Empty, errorMsg = string.Empty;

            if (pars != null)
            {
                pars = new PXSPParameter[] { };
            }

            pars.Append(new PXSPOutParameter("ErrorNbr", errorNbr));
            pars.Append(new PXSPOutParameter("ErrorMsg", errorMsg));

            var result = PXDatabase.Execute(spName, pars);

            return string.IsNullOrEmpty(errorMsg);
        }

        //private void DoProcessingMapping()
        //{
        //    var masterFinPeriodMaint = PXGraph.CreateInstance<MasterFinPeriodMaint>();
        //    var list = CalendarConversionMapDetails.Select();

        //    foreach (CalendarConversionMapDetail detailRecord in list)
        //    {
        //        var recs = masterFinPeriodMaint.Periods.Select(detailRecord.NewFinYear)
        //                            .Where(x => x.GetItem<MasterFinPeriod>().FinYear == detailRecord.NewFinYear)
        //                            .RowCast<MasterFinPeriod>().ToList();

        //        var deleteRec = recs.Where(x => x.IsAdjustment == true).FirstOrDefault();
        //        if (deleteRec != null) masterFinPeriodMaint.Periods.Cache.Delete(deleteRec);

        //        MasterFinPeriod rec = recs.Where(x => x.FinPeriodID == detailRecord.NewFinPeriodID).FirstOrDefault();

        //        if (rec != null)
        //        {
        //            rec.StartDate = detailRecord.NewStartDate;
        //            rec.EndDate = detailRecord.NewEndDate;
        //            masterFinPeriodMaint.Periods.Cache.Update(rec);
        //        }
        //        //else
        //        //{
        //        //    using (new MassInsertingOfPeriodsScope())
        //        //    {
        //        //        MasterFinPeriod newRec = new MasterFinPeriod
        //        //        {
        //        //            FinYear = detailRecord.NewFinYear,
        //        //            FinPeriodID = detailRecord.NewFinPeriodID,
        //        //            StartDate = detailRecord.NewStartDate,
        //        //            EndDate = detailRecord.NewEndDate,
        //        //            Active = true,
        //        //            Descr = "New Rec",
        //        //            IsAdjustment = detailRecord.IsAdjustment,
        //        //            Status = FinPeriod.status.Open,
        //        //            PeriodNbr = detailRecord.NewFinPeriodNbr,
        //        //            ARClosed = false,
        //        //            APClosed = false,
        //        //            FAClosed = false,
        //        //            CAClosed = false,
        //        //            INClosed = false
        //        //        };
        //        //        masterFinPeriodMaint.Periods.Insert(newRec);
        //        //    }
        //        //}
        //    }

        //    masterFinPeriodMaint.Actions.PressSave();
        //}

        public void UpdateCalendarMapDetails()
        {
            CalendarConversionMapDetails.Cache.Clear();

            if (string.IsNullOrEmpty(CalendarConversionMap.Current.StartYearID)
                    || string.IsNullOrEmpty(CalendarConversionMap.Current.StartPeriodNbr)) return;

            var results = SelectFrom<FinPeriod>
                                .Where<FinPeriod.finPeriodID.IsGreaterEqual<@P.AsString>.And<FinPeriod.active.IsEqual<True>>>
                                .View
                                .Select(this, CalendarConversionMap.Current.CurrentStartPeriodID);

            foreach (FinPeriod period in results)
            {
                var cci = new CalendarConversionMapDetail
                {
                    CurrentFinPeriodID = period.FinPeriodID,
                    CurrentFinYear = period.FinYear,
                    CurrentStartDate = period.StartDate,
                    CurrentEndDate = period.EndDateUI

                };

                CalendarConversionMapDetails.Cache.Insert(cci);
            }
        }

        public void _(Events.RowUpdated<CalendarConversionMap> e)
        {
            UpdateCalendarMapDetails();
        }

        public void _(Events.FieldUpdated<CalendarConversionMapDetail, CalendarConversionMapDetail.currentStartDate> e)
        {
            if (!(e.Row is CalendarConversionMapDetail row)) return;

            e.Row.NewStartDate = row.CurrentStartDate;
        }

        public void _(Events.FieldUpdated<CalendarConversionMapDetail, CalendarConversionMapDetail.currentEndDate> e)
        {
            if (!(e.Row is CalendarConversionMapDetail row)) return;

            e.Row.NewEndDate = row.CurrentEndDate;
        }

        //public void _(Events.RowUpdated<CalendarConversionMap> e)
        //{
        //    CalendarConversionMap row = e.Row;

        //    if (string.IsNullOrEmpty(row.StartYearID) || string.IsNullOrEmpty(row.StartPeriodNbr)) return;

        //    var results = SelectFrom<FinPeriod>
        //                        .Where<FinPeriod.finPeriodID.IsGreaterEqual<@P.AsString>.And<FinPeriod.active.IsEqual<True>>>
        //                        .View
        //                        .Select(this, CalendarConversionMap.Current.CurrentStartPeriodID);

        //    foreach (FinPeriod period in results)
        //    {
        //        var cci = new CalendarConversionMapDetail
        //        {
        //            CurrentFinPeriodID = period.FinPeriodID,
        //            CurrentFinYear = period.FinYear,
        //            CurrentStartDate = period.StartDate,
        //            CurrentEndDate = period.EndDate
        //        };

        //        CalendarConversionMapDetails.Insert(cci);
        //    }
        //}
    }
}
