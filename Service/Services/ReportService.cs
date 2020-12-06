using Data;
using Repo;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Services
{
    public class ReportService : IReportService
    {
        private IRepository<Report> reportRepository;
        private IRepository<ReportUser> reportUserRepository;

        public ReportService(IRepository<Report> reportRepository, IRepository<ReportUser> reportUserRepository)
        {
            this.reportRepository = reportRepository;
            this.reportUserRepository = reportUserRepository;
        }
        
        public IEnumerable<Report> GetReports()
        {
            var reports = new List<Report>();
            reportUserRepository.GetAll().ToList().ForEach(u =>
            {
                reports.Add(reportRepository.Get(u.ReportId));
            });
            return reports;
        }
        public IEnumerable<Report> GetReports(string userId)
        {
            var reports = new List<Report>();
            reportUserRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.UserId == userId)
                {
                    reports.Add(reportRepository.Get(u.ReportId));
                }
            });
            return reports;
        }
        public bool ReportComment(long commentId, string userId, string targetId)
        {
            var reportUsers = reportUserRepository.GetAll().ToList();
            foreach (var reportUser in reportUsers)
            {
                if (reportUser.UserId == userId && reportRepository.Get(reportUser.ReportId).CommentId == commentId)
                {
                    // report already exists
                    return false;
                }
            }
            // or insert new report
            Report reportEntity = new Report()
            {
                TargetId = targetId,
                CommentId = commentId,
            };
            reportRepository.Insert(reportEntity);
            reportUserRepository.Insert(new ReportUser()
            {
                ReportId = reportEntity.Id,
                UserId = userId,
            });
            return true;
        }
        public bool ReportPost(long postId, string userId, string targetId)
        {
            var reportUsers = reportUserRepository.GetAll().ToList();
            foreach (var reportUser in reportUsers)
            {
                if (reportUser.UserId == userId && reportRepository.Get(reportUser.ReportId).PostId == postId)
                {
                    // report already exists
                    return false;
                }
            }
            // or insert new report
            Report reportEntity = new Report()
            {
                TargetId = targetId,
                PostId = postId,
            };
            reportRepository.Insert(reportEntity);
            reportUserRepository.Insert(new ReportUser()
            {
                ReportId = reportEntity.Id,
                UserId = userId,
            });
            return true;
        }

        public void DeleteReport(long id)
        {
            reportUserRepository.GetAll().ToList().ForEach(u =>
            {
                if (u.ReportId == id)
                {
                    reportUserRepository.Remove(u);
                    reportRepository.Remove(reportRepository.Get(id));
                }
            });
            reportUserRepository.SaveChanges();
            reportRepository.SaveChanges();
        }

        public void DeleteReportsFromComment(long commentId)
        {
            reportUserRepository.GetAll().ToList().ForEach(u =>
            {
                var report = reportRepository.Get(u.ReportId);
                if (report.CommentId == commentId)
                {
                    reportUserRepository.Remove(u);
                    reportRepository.Remove(report);

                }
            });
            reportUserRepository.SaveChanges();
            reportRepository.SaveChanges();
        }
        public void DeleteReportsFromPost(long postId)
        {
            reportUserRepository.GetAll().ToList().ForEach(u =>
            {
                var report = reportRepository.Get(u.ReportId);
                if (report.PostId == postId)
                {
                    reportUserRepository.Remove(u);
                    reportRepository.Remove(report);
                }
            });
            reportUserRepository.SaveChanges();
            reportRepository.SaveChanges();
        }
    }
}
