using System.ComponentModel.DataAnnotations;

namespace CMSSolutions.Websites.Extensions
{
    public enum  SearchType
    {
        Articles = 1,
        Category = 2,
        Recruitment = 3
    }

    public enum WidgetName
    {
        SectionBannerSlider,
        SectionAbout,
        SectionBusinesses,
        SectionClient,
        PageHeaderTop,
        PageNavigationMenu,
        PageFooter
    }

    public enum PageSlider
    {
        [Display(Name = "Slider Home Page")]
        HomeBannerSlider = 1,

        [Display(Name = "Banner About Us")]
        AboutBanner = 2,

        [Display(Name = "Banner Businesses")]
        Businesses = 3,

        [Display(Name = "Banner Partner")]
        Partner = 7,

        [Display(Name = "Banner Media")]
        Media = 8,

        [Display(Name = "Banner Media Details")]
        MediaDetails = 10,

        [Display(Name = "Banner Recruitment")]
        Recruitment = 9
    }

    public enum CategoriesType
    {
        None = 0,
        AboutUs = 1,
        Businesses = 2,
        Partner = 38,
        OnlineTakeAway = 4,
        CateringServices = 10,
        OurPartner = 5,
        Recruitment = 6,
        ContactUs = 7,
        CKC = 8,
        ConsultingServices = 9,
        GreenTangerine = 11,
        FineDiningRestaurants = 12,
        Franchising = 13,
        Media = 14,
        Awards = 15,
        Clips = 16,
        News = 17
    }

    public enum CategoriesCache
    {
        AboutUs_VN = 3,
        AboutUs_EN = 4,
        AboutUs_FR = 42,
        OurBusinesses_VN = 5,
        OurBusinesses_EN = 6,
        OurBusinesses_FR = 43,
        CKC_VN = 7,
        CKC_EN = 8,
        CKC_FR = 48,
        OnlineTakeAway_VN = 9,
        OnlineTakeAway_EN = 10,
        CateringServices_VN = 11,
        CateringServices_EN = 12,
        ConsultingServices_VN = 15,
        ConsultingServices_EN = 16,
        ConsultingServices_FR = 53,
        OurPartner_VN = 19,
        OurPartner_EN = 20,
        OurPartner_FR = 45,
        Recruitment_VN = 22,
        Recruitment_EN = 21,
        Recruitment_FR = 46,
        ContactUs_VN = 23,
        ContactUs_EN = 24,
        ContactUs_FR = 47,
        GreenTangerine_VN = 25,
        GreenTangerine_EN = 28,
        GreenTangerine_FR = 51,
        FineDiningRestaurants_VN = 13,
        FineDiningRestaurants_EN = 14,
        FineDiningRestaurants_FR = 50,
        Franchising_VN = 17,
        Franchising_EN = 18,
        Franchising_FR = 54,
        News_VN = 31,
        News_EN = 32,
        News_FR = 44,
        Awards_VN = 33,
        Awards_EN = 34,
        Awards_FR = 55,
        Media_VN = 35,
        Media_EN = 36,
        Media_FR = 56,
        Clips_VN = 37,
        Clips_EN = 38,
        Clips_FR = 57
    }

    public enum FixRoute
    {
        [Display(Name = "/we/")]
        AboutUs,

        [Display(Name = "/news/")]
        Media,

        [Display(Name = "/customers/")]
        OurPartner,

        [Display(Name = "/jobs/")]
        Recruitment
    }

    public enum Status
    {
        [Display(Name = "Đang sử dụng")]
        Approved = 1,

        [Display(Name = "Xóa tạm thời")]
        Deleted = 2
    }

    public class Constants
    {
        public const int WidthPartner = 164;
        public const int HeightPartner = 164;

        public const string DateTimeFomat = "dd/MM/yyyy";
        public const string DateTimeFomat2 = "dd-MM-yyyy";
        public const string DateTimeFomatFullNone = "ddMMyyyyhhmm";

        public const string VI = "vi-VN";
        public const string EN = "en-US";
        public const string FR = "fr-FR";
        public const string IsNull = "null";
        public const string IsUndefined = "undefined";
        public const string ImageDefault = "/Images/themes/no-image.png";

        public const string ViewPageHeaderTop = "~/Views/Shared/PageHeaderTop.cshtml";
        public const string ViewPageNavigationMenu = "~/Views/Shared/PageNavigationMenu.cshtml";
        public const string ViewPageFooter = "~/Views/Shared/PageFooter.cshtml";
        public const string ViewSectionBannerSlider = "~/Views/Shared/SectionBannerSlider.cshtml";
        public const string ViewPageBannerImage = "~/Views/Shared/PageBannerImage.cshtml";
        public const string ViewSectionAbout = "~/Views/Shared/SectionAbout.cshtml";
        public const string ViewSectionBusinesses = "~/Views/Shared/SectionBusinesses.cshtml";
        public const string ViewOurBusinesses = "~/Views/Shared/OurBusinesses.cshtml";
        public const string ViewSectionClient = "~/Views/Shared/SectionClient.cshtml";
        public const string ViewAboutUs = "~/Views/Shared/AboutUs.cshtml";
        public const string ViewConsultingServices = "~/Views/Shared/ConsultingServices.cshtml";
        public const string ViewCKC = "~/Views/Shared/CKC.cshtml";
        public const string ViewFranchising = "~/Views/Shared/Franchising.cshtml";
        public const string ViewOurPartner = "~/Views/Shared/OurPartner.cshtml";
        public const string ViewGoogleMap = "~/Views/Shared/GoogleMap.cshtml";
        public const string ViewContact = "~/Views/Shared/Contact.cshtml";
        public const string ViewMedia = "~/Views/Shared/Media.cshtml";
        public const string ViewRecruitment = "~/Views/Shared/Recruitment.cshtml";
        public const string ViewRecruitmentTemplate = "~/Views/Shared/RecruitmentTemplate_{0}.cshtml";
        public const string ViewMediaDetails = "~/Views/Shared/MediaDetails.cshtml";
        public const string ViewSearchResults = "~/Views/Shared/SearchResults.cshtml";
        public const string ViewRecruitmentDetails = "~/Views/Shared/RecruitmentDetails.cshtml";
        public const string ViewFineDiningRestaurants = "~/Views/Shared/FineDiningRestaurants.cshtml";

        public const string LanguageCode = "LanguageCode";
        public const string PartnerId = "PartnerId";
        public const string CategoryId = "CategoryId";
        public const string StatusId = "StatusId";
        public const string FromDate = "FromDate";
        public const string ToDate = "ToDate";
        public const string SearchText = "SearchText";
        public const string UserId = "UserId";
        public const string ParentId = "ParentId";
        public const string Gender = "Gender";
        public const string ArticlesId = "ArticlesId";
        public const string PageType = "PageType";
        public const string ListCategory = "ListCategoryIds";

        public const string CssControlCustom = "form-control-custom";
        public const string CssThumbsSize = "thumbs-size";

        public const string SeoTitle = "SeoTitle";
        public const string SeoDescription = "SeoDescription";
        public const string SeoKeywords = "SeoKeywords";

        public class CacheKeys
        {
            public const string ARTICLES_BY_CATEGORY_ID_TOP = "ARTICLES_BY_CATEGORY_ID_{0}_{1}_{2}_{3}";
            public const string ARTICLES_BY_CATEGORY_ID = "ARTICLES_BY_CATEGORY_ID_{0}_{1}_{2}";
            public const string CATEGORY_ALL_TABLE = "CATEGORY_ALL_TABLE_{0}";
            public const string SEARCH_ALL_TABLE = "SEARCH_ALL_TABLE_{0}";
        }
    }
}