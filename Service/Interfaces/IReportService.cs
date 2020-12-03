using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interfaces
{
    public interface IReportService
    {
        IEnumerable<Report> GetReports();
        bool ReportComment(long commentId, string userId, string targetId);
        bool ReportPost(long postId, string userId, string targetId);
        void DeleteReportsFromComment(long commentId, string targetId);
        void DeleteReportsFromPost(long postId, string targetId);
    }
}