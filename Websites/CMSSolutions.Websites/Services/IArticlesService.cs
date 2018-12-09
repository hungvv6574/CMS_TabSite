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
    
    
    public interface IArticlesService : IGenericService<ArticlesInfo, int>, IDependency
    {
        string LanguageCode { get; set; }

        int CategoryId { get; set; }

        bool CheckAlias(int id, string alias);

        ArticlesInfo GetByAlias(string alias, string languageCode);

        List<ArticlesInfo> SearchPaged(string searchText, int userId, int status, int pageIndex, int pageSize, out int totalRecord);

        ArticlesInfo GetByCategoryId(int categoryId);

        IList<ArticlesInfo> GetAllYear();

        IList<ArticlesInfo> GetMediaByCategoryId(int year, int pageIndex, int pageSize, out int totalRecord);

        IList<ArticlesInfo> GetMediaCountTop(int year, int articlesId, int top);
    }
    
    public class ArticlesService : GenericService<ArticlesInfo, int>, IArticlesService
    {
        public string LanguageCode { get; set; }

        public int CategoryId { get; set; }

        public ArticlesService(IEventBus eventBus, IRepository<ArticlesInfo, int> repository) : 
                base(repository, eventBus)
        {

        }

        public bool CheckAlias(int id, string alias)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@Alias", alias),
                AddInputParameter("@Id", id)
            };

            var result = (int)ExecuteReaderResult("sp_Articles_CheckAlias", list.ToArray());
            return result > 0;
        }

        public ArticlesInfo GetByAlias(string alias, string languageCode)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@Alias", alias),
                AddInputParameter("@LanguageCode", languageCode)
            };

            var articlesInfo = ExecuteReaderRecord<ArticlesInfo>("sp_Articles_GetByAlias", list.ToArray());
            if (articlesInfo == null)
            {
                articlesInfo = Repository.Table.FirstOrDefault(x => x.Alias == alias && x.RefId != 0);
                if (articlesInfo != null)
                {
                    return Repository.Table.FirstOrDefault(x => x.RefId == articlesInfo.RefId && x.LanguageCode == languageCode);
                }
            }

            return articlesInfo;
        }

        public List<ArticlesInfo> SearchPaged(string searchText, int userId, int status, int pageIndex, int pageSize, out int totalRecord)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@SearchText", searchText),
                AddInputParameter("@CategoryId", CategoryId),
                AddInputParameter("@UserId", userId),
                AddInputParameter("@Status", status),
                AddInputParameter("@PageIndex", pageIndex),
                AddInputParameter("@PageSize", pageSize)
            };

            return ExecuteReader<ArticlesInfo>("sp_Articles_Search_Paged", "@TotalRecord", out totalRecord, list.ToArray());
        }

        public ArticlesInfo GetByCategoryId(int categoryId)
        {
            if (Repository.Table.Any())
            {
                return Repository.Table.FirstOrDefault(x => x.LanguageCode == LanguageCode && x.CategoryId == categoryId && x.IsPublished && !x.IsDeleted);
            }

            return  null;
        }

        public IList<ArticlesInfo> GetAllYear()
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@LanguageCode", LanguageCode),
                AddInputParameter("@CategoryId", CategoryId)
            };

            return ExecuteReader<ArticlesInfo>("sp_Articles_GetAllYear", list.ToArray());
        }

        public IList<ArticlesInfo> GetMediaByCategoryId(int year, int pageIndex, int pageSize, out int totalRecord)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@LanguageCode", LanguageCode),
                AddInputParameter("@CategoryId", CategoryId),
                AddInputParameter("@Year", year),
                AddInputParameter("@PageIndex", pageIndex),
                AddInputParameter("@PageSize", pageSize)
            };

            return ExecuteReader<ArticlesInfo>("sp_Articles_GetMedia", "@TotalRecord", out totalRecord, list.ToArray());
        }

        public IList<ArticlesInfo> GetMediaCountTop(int year, int articlesId, int top)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@LanguageCode", LanguageCode),
                AddInputParameter("@CategoryId", CategoryId),
                AddInputParameter("@Year", year),
                AddInputParameter("@ArticlesId", articlesId),
                AddInputParameter("@CountTop", top),
            };

            return ExecuteReader<ArticlesInfo>("sp_Articles_GetMediaCountTop", list.ToArray());
        }
    }
}
