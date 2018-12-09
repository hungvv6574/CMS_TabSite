using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CMSSolutions.Caching;

namespace CMSSolutions.Websites.Services
{
    using CMSSolutions;
    using CMSSolutions.Websites.Entities;
    using CMSSolutions.Events;
    using CMSSolutions.Services;
    using CMSSolutions.Data;
    
    
    public interface ICategoriesService : ICacheService<CategoryInfo, int>, IGenericService<CategoryInfo, int>, IDependency
    {
        string LanguageCode { get; set; }

        CategoryInfo GetHomePage();

        string GetCategoryName(int id);

        IList<CategoryInfo> GetPaged(bool isDeleted, int pageIndex, int pageSize, out int totals);

        bool CheckAlias(int id, string alias);

        CategoryInfo GetByAlias(string alias, string languageCode);

        List<CategoryInfo> GetChildenByParentId(int parentId);

        List<CategoryInfo> GetAllParent();

        List<CategoryInfo> GetTree();

        List<CategoryInfo> GetByListId(string listId, bool isParent);
    }
    
    public class CategoriesService : GenericService<CategoryInfo, int>, ICategoriesService
    {
        private readonly ICacheInfo cacheManager;

        public string LanguageCode { get; set; }

        public CategoriesService(
            IRepository<CategoryInfo, int> repository, 
            IEventBus eventBus,
            ICacheInfo cacheManager)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
        }

        public override void Insert(CategoryInfo record)
        {
            Repository.Insert(record);
            LanguageCode = record.LanguageCode;
            ResetCache();
        }

        public override void Update(CategoryInfo record)
        {
            Repository.Update(record);
            LanguageCode = record.LanguageCode;
            ResetCache();
        }

        public override void Delete(CategoryInfo record)
        {
            Repository.Delete(record);
            LanguageCode = record.LanguageCode;
            ResetCache();
        }

        public override void Save(CategoryInfo record)
        {
            if (record.IsTransient())
            {
                Insert(record);
            }
            else
            {
                Update(record);
            }

            ResetCache();
        }

        public CategoryInfo GetHomePage()
        {
            var list = GetAllCache();
            if (list != null && list.Count > 0)
            {
                return list.FirstOrDefault(x => x.IsHome && x.IsActived);
            }

            return null;
        }

        public string GetCategoryName(int id)
        {
            var list = GetAllCache();
            if (list != null && list.Count > 0)
            {
                return list.Where(x => x.Id == id && !x.IsActived).Select(x => x.Name).FirstOrDefault();
            }

            return string.Empty;
        }

        public bool CheckAlias(int id, string alias)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@Alias", alias),
                AddInputParameter("@Id", id)
            };

            var result = (int)ExecuteReaderResult("sp_Categories_CheckAlias", list.ToArray());
            return result > 0;
        }

        public CategoryInfo GetByAlias(string alias, string languageCode)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@Alias", alias),
                AddInputParameter("@LanguageCode", languageCode)
            };

            var cate = ExecuteReaderRecord<CategoryInfo>("sp_Categories_GetByAlias", list.ToArray());
            if (cate == null)
            {
                cate = Repository.Table.FirstOrDefault(x => x.Alias == alias && x.RefId != 0);
                if (cate != null)
                {
                    return Repository.Table.FirstOrDefault(x => x.RefId == cate.RefId && x.LanguageCode == languageCode);
                }
            }

            return cate;
        }

        public List<CategoryInfo> GetTree()
        {
            var listParent = GetAllParent();
            var list = new List<CategoryInfo>();
            if (listParent != null)
            {
                foreach (var parent in listParent)
                {
                    parent.ChildenName = parent.ShortName;
                    parent.HasChilden = true;
                    list.Add(parent);
                    var subitems = GetChildenByParentId(parent.Id).OrderBy(x => x.OrderBy);
                    list.AddRange(subitems);
                }
            }

            return list;
        }

        public List<CategoryInfo> GetByListId(string listId, bool isParent)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@ListId", listId)
            };

            return ExecuteReader<CategoryInfo>("sp_Categories_GetByIds", list.ToArray());
        }

        public List<CategoryInfo> GetChildenByParentId(int parentId)
        {
            var list = GetAllCache();
            var results = new List<CategoryInfo>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (parentId > 0 && item.ParentId == parentId && item.IsActived && !item.IsDeleted)
                    {
                        item.HasChilden = false;
                        item.ChildenName = "--- " + item.ShortName;
                        results.Add(item);
                        var subitems = GetChildenByParentId2(item.Id).OrderBy(x => x.OrderBy);
                        results.AddRange(subitems);
                    }
                }

                return results;
            }

            return null;
        }

        public List<CategoryInfo> GetChildenByParentId2(int parentId)
        {
            var list = GetAllCache();
            var results = new List<CategoryInfo>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (parentId > 0 && item.ParentId == parentId && item.IsActived && !item.IsDeleted)
                    {
                        item.HasChilden = false;
                        item.ChildenName = "--- --- " + item.ShortName;
                        results.Add(item);
                    }
                }

                results.Sort((foo1, foo2) => foo1.OrderBy.CompareTo(foo2.OrderBy));

                return results;
            }

            return null;
        }

        public List<CategoryInfo> GetAllParent()
        {
            var list = GetAllCache();
            if (list != null && list.Count > 0)
            {
                return list.Where(x => x.ParentId == 0 && x.IsActived && !x.IsDeleted).OrderBy(x=> x.OrderBy).ToList();
            }

            return null;
        }

        public CategoryInfo GetByIdCache(int id)
        {
            var list = GetAllCache();
            if (list != null && list.Count > 0)
            {
                return list.FirstOrDefault(x => x.Id == id && x.IsActived && !x.IsDeleted);
            }

            return null;
        }

        public IList<CategoryInfo> GetAllCache()
        {
            var results = cacheManager.Get(string.Format(CMSSolutions.Websites.Extensions.Constants.CacheKeys.CATEGORY_ALL_TABLE, LanguageCode));
            if (results == null)
            {
                results = ResetCache();
            }

            var list = (List<CategoryInfo>) results;
            if (results != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    foreach (var check in list)
                    {
                        if (check.Id == item.ParentId)
                        {
                            item.ParentName = check.ShortName;
                            break;
                        }
                    }
                }

                return list;
            }

            return new List<CategoryInfo>();
        }

        public IList<CategoryInfo> GetPaged(bool isDeleted, int pageIndex, int pageSize, out int totals)
        {
            var results = GetAllCache();
            if (results != null)
            {
                var list = results.Where(y => y.IsDeleted == isDeleted && y.RefId == 0);
                totals = list.Count();
                var users = (from x in list select x).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return users;
            }

            totals = 0;
            return new List<CategoryInfo>();
        }

        public IList<CategoryInfo> ResetCache()
        {
            cacheManager.Remove(string.Format(CMSSolutions.Websites.Extensions.Constants.CacheKeys.CATEGORY_ALL_TABLE, LanguageCode));
            var table = Repository.Table.Where(x => x.LanguageCode == LanguageCode).ToList();
            cacheManager.Add(string.Format(CMSSolutions.Websites.Extensions.Constants.CacheKeys.CATEGORY_ALL_TABLE, LanguageCode), table);

            return table;
        }
    }
}
