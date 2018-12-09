using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace CMSSolutions.Websites.Services
{
    using CMSSolutions;
    using CMSSolutions.Websites.Entities;
    using CMSSolutions.Events;
    using CMSSolutions.Services;
    using CMSSolutions.Data;

    public interface IRecruitmentService : IGenericService<RecruitmentInfo, int>, IDependency
    {
        string LanguageCode { get; set; }

        int CategoryId { get; set; }

        bool CheckAlias(int id, string alias);

        RecruitmentInfo GetByAlias(string alias, string languageCode);

        List<RecruitmentInfo> SearchPaged(int status, int pageIndex, int pageSize, out int totalRecord);

        IList<RecruitmentInfo> GetByCategoryId(int recruitmentId, int pageIndex, int pageSize, out int totalRecord);
    }

    public class RecruitmentService : GenericService<RecruitmentInfo, int>, IRecruitmentService
    {
        public RecruitmentService(IEventBus eventBus, IRepository<RecruitmentInfo, int> repository) : 
                base(repository, eventBus)
        {

        }

        public string LanguageCode { get; set; }

        public int CategoryId { get; set; }

        public bool CheckAlias(int id, string alias)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@Alias", alias),
                AddInputParameter("@Id", id)
            };

            var result = (int)ExecuteReaderResult("sp_Recruitment_CheckAlias", list.ToArray());
            return result > 0;
        }

        public RecruitmentInfo GetByAlias(string alias, string languageCode)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@Alias", alias),
                AddInputParameter("@LanguageCode", languageCode)
            };

            var recruitment = ExecuteReaderRecord<RecruitmentInfo>("sp_Recruitment_GetByAlias", list.ToArray());

            if (recruitment == null)
            {
                recruitment = Repository.Table.FirstOrDefault(x => x.Alias == alias && x.RefId != 0);
                if (recruitment != null)
                {
                    return Repository.Table.FirstOrDefault(x => x.RefId == recruitment.RefId && x.LanguageCode == languageCode);
                }
            }

            return recruitment;
        }

        public List<RecruitmentInfo> SearchPaged(int status, int pageIndex, int pageSize, out int totalRecord)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@CategoryId", CategoryId),
                AddInputParameter("@Status", status),
                AddInputParameter("@PageIndex", pageIndex),
                AddInputParameter("@PageSize", pageSize)
            };

            return ExecuteReader<RecruitmentInfo>("sp_Recruitment_Search_Paged", "@TotalRecord", out totalRecord, list.ToArray());
        }

        public IList<RecruitmentInfo> GetByCategoryId(int recruitmentId, int pageIndex, int pageSize, out int totalRecord)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@LanguageCode", LanguageCode),
                AddInputParameter("@CategoryId", CategoryId),
                AddInputParameter("@RecruitmentId", recruitmentId),
                AddInputParameter("@PageIndex", pageIndex),
                AddInputParameter("@PageSize", pageSize)
            };

            return ExecuteReader<RecruitmentInfo>("sp_Recruitment_GetByCategory", "@TotalRecord", out totalRecord, list.ToArray());
        }
    }
}
