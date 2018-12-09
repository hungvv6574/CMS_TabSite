using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CMSSolutions.Websites.Extensions
{
    public class Utilities
    {
        public static string FixCheckboxValue(string value)
        {
            return value == "true,false" ? "true" : value;
        }

        public static CategoriesType GetCategoryParent(int categoryId)
        {
            var cate = (CategoriesCache)categoryId;
            switch (cate)
            {
                case CategoriesCache.Awards_VN:
                    return CategoriesType.Awards;
                case CategoriesCache.Awards_EN:
                    return CategoriesType.Awards;
                case CategoriesCache.Awards_FR:
                    return CategoriesType.Awards;
                case CategoriesCache.Media_VN:
                    return CategoriesType.Media;
                case CategoriesCache.Media_EN:
                    return CategoriesType.Media;
                case CategoriesCache.Media_FR:
                    return CategoriesType.Media;
                case CategoriesCache.Clips_VN:
                    return CategoriesType.Clips;
                case CategoriesCache.Clips_EN:
                    return CategoriesType.Clips;
                case CategoriesCache.Clips_FR:
                    return CategoriesType.Clips;
                default:
                    return CategoriesType.None;
            }
        }

        public static int GetCategoryId(CategoriesType type, string languageCode)
        {
            var id = 0;
            switch (type)
            {
                case CategoriesType.AboutUs:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.AboutUs_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.AboutUs_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.AboutUs_FR;
                            break;
                    }
                    break;
                case CategoriesType.Businesses:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.OurBusinesses_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.OurBusinesses_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.OurBusinesses_FR;
                            break;
                    }
                    break;
                case CategoriesType.CKC:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.CKC_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.CKC_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.CKC_FR;
                            break;
                    }
                    break;
                case CategoriesType.ConsultingServices:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.ConsultingServices_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.ConsultingServices_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.ConsultingServices_FR;
                            break;
                    }
                    break;
                case CategoriesType.Partner:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.OurPartner_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.OurPartner_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.OurPartner_FR;
                            break;
                    }
                    break;
                //case CategoriesType.OnlineTakeAway:
                //    switch (languageCode)
                //    {
                //        case Constants.VI:
                //            id = (int)CategoriesCache.OnlineTakeAway_VN;
                //            break;
                //        case Constants.EN:
                //            id = (int)CategoriesCache.OnlineTakeAway_EN;
                //            break;
                //    }
                //    break;
                //case CategoriesType.CateringServices:
                //    switch (languageCode)
                //    {
                //        case Constants.VI:
                //            id = (int)CategoriesCache.CateringServices_VN;
                //            break;
                //        case Constants.EN:
                //            id = (int)CategoriesCache.CateringServices_EN;
                //            break;
                //    }
                //    break;
                case CategoriesType.Recruitment:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.Recruitment_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.Recruitment_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.Recruitment_FR;
                            break;
                    }
                    break;
                case CategoriesType.ContactUs:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.ContactUs_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.ContactUs_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.ContactUs_FR;
                            break;
                    }
                    break;
                
                case CategoriesType.GreenTangerine:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.GreenTangerine_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.GreenTangerine_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.GreenTangerine_FR;
                            break;
                    }
                    break;
                case CategoriesType.FineDiningRestaurants:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.FineDiningRestaurants_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.FineDiningRestaurants_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.FineDiningRestaurants_FR;
                            break;
                    }
                    break;
                case CategoriesType.Franchising:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.Franchising_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.Franchising_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.Franchising_FR;
                            break;
                    }
                    break;
                
                case CategoriesType.Awards:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.Awards_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.Awards_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.Awards_FR;
                            break;
                    }
                    break;
                case CategoriesType.Media:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.Media_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.Media_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.Media_FR;
                            break;
                    }
                    break;
                case CategoriesType.Clips:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.Clips_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.Clips_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.Clips_FR;
                            break;
                    }
                    break;
                case CategoriesType.News:
                    switch (languageCode)
                    {
                        case Constants.VI:
                            id = (int)CategoriesCache.News_VN;
                            break;
                        case Constants.EN:
                            id = (int)CategoriesCache.News_EN;
                            break;
                        case Constants.FR:
                            id = (int)CategoriesCache.News_FR;
                            break;
                    }
                    break;
            }

            return id;
        }

        public static string GetCharUnsigned(string values)
        {
            if (string.IsNullOrEmpty(values))
            {
                return string.Empty;
            }

            var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = values.Normalize(NormalizationForm.FormD);
            var converttext = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');

            converttext = Regex.Replace(converttext, "[^a-zA-Z0-9_.]+", " ", RegexOptions.Compiled);

            var list = new char[] { ' ', '/', ',', '&', '\"', '?', '|', ':', '"', '`', '\\', ';', '~', '!', '@', '#', '$', '%', '^', '*', '(', ')', '\'', '_', '=', '+', '{', '}', '[', ']', '.', '>', '<' };
            converttext = list.Aggregate(converttext, (current, schar) => current.Replace(schar, ' '));

            converttext = converttext.Replace("--", " ").Trim('.').TrimEnd(' ').TrimStart(' ');

            return converttext.ToLower();
        }

        public static int[] ParseListInt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new int[0];
            }

            return value.Split(',').Select(Int32.Parse).ToArray();
        }

        public static string ParseString(int[] list)
        {
            if (list != null && list.Length > 0)
            {
                return string.Join(", ", list);
            }

            return string.Empty;
        }

        public static string GetAlias(string values)
        {
            if (string.IsNullOrEmpty(values))
            {
                return string.Empty;
            }

            var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = values.Normalize(NormalizationForm.FormD);
            var converttext = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');

            converttext = Regex.Replace(converttext, "[^a-zA-Z0-9_.]+", " ", RegexOptions.Compiled);

            var list = new char[] { ' ', '/', ',', '&', '\"', '?', '|', ':', '"', '`', '\\', ';', '~', '!', '@', '#', '$', '%', '^', '*', '(', ')', '\'', '_', '=', '+', '{', '}', '[', ']', '.', '>', '<' };
            converttext = list.Aggregate(converttext, (current, schar) => current.Replace(schar, '-'));

            converttext = converttext.Replace("--", "-").Trim('.').TrimEnd('-').TrimStart('-');

            return converttext.ToLower();
        }

        public static bool IsNotNull(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value.Equals(Constants.IsNull) || value.Equals(""))
            {
                return false;
            }

            if (value.Equals(Constants.IsUndefined))
            {
                return false;
            }

            return true;
        }

        public static void WriteEventLog(string messages)
        {
            try
            {
                var eventLogName = "ApplicationErrors";
                if (!EventLog.SourceExists(eventLogName))
                {
                    EventLog.CreateEventSource(eventLogName, "Application Errors");
                }

                var log = new EventLog { Source = eventLogName };
                log.WriteEntry(messages, EventLogEntryType.Error);
            }
            catch (Exception)
            {

            }
        }
    }
}