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

    public interface ISearchService : IGenericService<SearchInfo, long>, IDependency
    {
        string LanguageCode { get; set; }

        int SearchType { get; set; }

        SearchInfo GetByItem(long id);

        IList<SearchInfo> Search(List<SearchCondition> conditions, int pageIndex, int pageSize, ref int total);

        void ResetCache();
    }

    public class SearchService : GenericService<SearchInfo, long>, ISearchService
    {
        public string LanguageCode { get; set; }

        public int SearchType { get; set; }

        public SearchService(IEventBus eventBus, IRepository<SearchInfo, long> repository) : 
                base(repository, eventBus)
        {

        }

        public SearchInfo GetByItem(long id)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@SearchId", id),
                AddInputParameter("@SearchType", SearchType),
                AddInputParameter("@LanguageCode", LanguageCode)
            };

            return ExecuteReaderRecord<SearchInfo>("sp_Images_GetBySearchId", list.ToArray());
        }

        public IList<SearchInfo> Search(List<SearchCondition> conditions, int pageIndex, int pageSize, ref int total)
        {
            var service = new LuceneService();
            service.LanguageCode = LanguageCode;
            return service.Search(conditions, true, pageIndex, pageSize, ref total);
        }

        public void ResetCache()
        {
            var data = ExecuteReader("sp_Search_BuildJson", new SqlParameter("@LanguageCode", LanguageCode));
            var service = new LuceneService {LanguageCode = LanguageCode};
            service.ClearLuceneIndex();
            service.AddUpdateLuceneIndex(data.Tables[0]);
        }
    }
}
