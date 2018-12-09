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
    
    
    public interface IImagesService : IGenericService<ImageInfo, long>, IDependency
    {
        List<ImageInfo> GetCategoriesBusinesses(string listIds);

        List<ImageInfo> SearchPaged(string languageCode, int categoryId, int articlesId, int pageIndex, int pageSize, out int totalRecord);
    }
    
    public class ImagesService : GenericService<ImageInfo, long>, IImagesService
    {
        
        public ImagesService(IEventBus eventBus, IRepository<ImageInfo, long> repository) : 
                base(repository, eventBus)
        {

        }

        public List<ImageInfo> GetCategoriesBusinesses(string listIds)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@ListId", listIds)
            };

            return ExecuteReader<ImageInfo>("sp_Images_GetByCategoryId", list.ToArray()).OrderBy(x => x.SortOrder).ToList();
        }

        public List<ImageInfo> SearchPaged(string languageCode, int categoryId, int articlesId, int pageIndex, int pageSize, out int totalRecord)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@LanguageCode", languageCode),
                AddInputParameter("@CategoryId", categoryId),
                AddInputParameter("@ArticlesId", articlesId),
                AddInputParameter("@PageIndex", pageIndex),
                AddInputParameter("@PageSize", pageSize)
            };

            return ExecuteReader<ImageInfo>("sp_Images_Search_Paged", "@TotalRecord", out totalRecord, list.ToArray());
        }
    }
}
