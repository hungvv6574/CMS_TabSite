using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMSSolutions.Localization.Domain;
using CMSSolutions.Websites.Entities;

namespace CMSSolutions.Websites.Models
{
    public class DataViewerModel
    {
        public int TotalRow { get; set; }

        public int TotalPage
        {
            get
            {
                if (TotalRow <= PageSize)
                {
                    return 1;
                }

                var count = TotalRow % PageSize;
                if ((count == 0))
                {
                    return TotalRow / PageSize;
                }

                return ((TotalRow - count) / PageSize) + 1;
            }
        }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public bool Status { get; set; }

        public string Html { get; set; }

        public string Data { get; set; }

        public bool IsShowPartner { get; set; }

        public StringBuilder Breadcrumb { get; set; }

        public IList<CategoryInfo> ListCategoriesParent { get; set; }

        public IList<CategoryInfo> ListCategories { get; set; }

        public IList<SliderInfo> ListSliderImages { get; set; }

        public SliderInfo BannerImages { get; set; }

        public IList<int> ListYear { get; set; } 

        public IList<ArticlesInfo> ListArticles { get; set; }

        public IList<SearchInfo> ListSearch { get; set; }

        public CategoryInfo CategoryInfo { get; set; }

        public CategoryInfo CategoryChilden { get; set; }

        public ArticlesInfo Articles { get; set; }

        public int CategoryId { get; set; }

        public int ArticlesId { get; set; }

        public int Year { get; set; }

        public string Keyword { get; set; }

        public IList<PartnerInfo> ListPartners { get; set; }

        public List<Language> ListLanguages { get; set; }

        public List<ImageInfo> ListImages { get; set; }

        public List<ImageInfo> ListImagesFigure1
        {
            get
            {
                if (ListImages != null && ListImages.Count > 0)
                {
                    return ListImages.Where(x => x.SortOrder == 1).ToList();
                }

                return null;
            }
        }

        public List<ImageInfo> ListImagesFigureOther
        {
            get
            {
                if (ListImagesFigure1 != null && ListImagesFigure1.Count > 0)
                {
                   return ListImages.Where(x => x.SortOrder != 1).ToList();
                }

                return null;
            }
        } 

        public IList<RecruitmentInfo> ListRecruitments { get; set; }

        public RecruitmentInfo Recruitments { get; set; }

        public Dictionary<ImageInfo, List<CategoryInfo>> ListCategoryImages { get; set; }
    }
}