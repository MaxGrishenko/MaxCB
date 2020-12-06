using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Interfaces
{
    public interface IReportService
    {
        IEnumerable<Report> GetReports();
        IEnumerable<Report> GetReports(string userId);
        
        bool ReportComment(long commentId, string userId, string targetId);
        bool ReportPost(long postId, string userId, string targetId);

        void DeleteReport(long id);


        void DeleteReportsFromComment(long commentId);
        void DeleteReportsFromPost(long postId);
    }
}