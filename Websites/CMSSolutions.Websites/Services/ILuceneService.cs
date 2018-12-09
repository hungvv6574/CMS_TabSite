using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using CMSSolutions.Websites.Entities;
using CMSSolutions.Websites.Extensions;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace CMSSolutions.Websites.Services
{
    public class SearchCondition
    {
        public string[] SearchField { get; set; }

        public string Keyword { get; set; }

        public bool ConditionForField { get; set; }

        public SearchCondition(string[] fields, string keyword, bool condition)
        {
            this.ConditionForField = condition;
            this.SearchField = fields;
            this.Keyword = keyword;
        }

        public SearchCondition(string[] fields, string keyword)
        {
            this.ConditionForField = true;
            this.SearchField = fields;
            this.Keyword = keyword;
        }

        public SearchCondition(string fields, string keyword)
        {
            this.ConditionForField = true;
            this.SearchField = new string[] { fields };
            this.Keyword = keyword;
        }
    }

    public interface ILuceneService
    {

    }

    public class LuceneService : ILuceneService
    {
        public string LanguageCode { get; set; }

        public string CacheFolderPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath("~/Media/Default/CacheSearch/");
            }
        }

        private string _luceneDir
        {
            get
            {
                return Path.Combine(CacheFolderPath, LanguageCode);
            }
        }

        private FSDirectory _directoryTemp;
        private FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null)
                {
                    _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                }
                if (IndexWriter.IsLocked(_directoryTemp))
                {
                    IndexWriter.Unlock(_directoryTemp);
                }
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath))
                {
                    File.Delete(lockFilePath);
                }
                return _directoryTemp;
            }
        }

        public bool ClearLuceneIndexRecord(int record_id)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                try
                {
                    var searchQuery = new TermQuery(new Term("Id", record_id.ToString()));
                    writer.DeleteDocuments(searchQuery);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    analyzer.Close();
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        public bool AddUpdateLuceneIndex(DataTable table)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                try
                {
                    writer.DeleteAll();
                    foreach (DataRow row in table.Rows)
                    {
                        var document = new Document();
                       
                        document.Add(new Field("Id", row["Id"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("LanguageCode", row["LanguageCode"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("CategoryId", row["CategoryId"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("Type", row["Type"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("SearchId", row["SearchId"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("Title", row["Title"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("Url", row["Url"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("Alias", row["Alias"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("VideoUrl", row["VideoUrl"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("Sumary", row["Sumary"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("Images", row["Images"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("Tags", row["Tags"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("CreateDate", DateTime.Parse(row["CreateDate"].ToString()).ToString(Extensions.Constants.DateTimeFomat), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("Keyword", Utilities.GetCharUnsigned(row["Title"].ToString()), Field.Store.YES, Field.Index.ANALYZED));

                        writer.AddDocument(document);
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    analyzer.Close();
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        public bool AddUpdateLuceneIndex(List<SearchInfo> listData)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                try
                {
                    foreach (var sampleData in listData)
                    {
                        AddToLuceneIndex(sampleData, writer);
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    analyzer.Close();
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        public bool UpdateLuceneIndex(SearchInfo dataObject)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                try
                {
                    writer.DeleteAll();
                    var doc = CreateDoc(dataObject);
                    writer.UpdateDocument(new Term("Id", dataObject.Id.ToString()), doc);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    analyzer.Close();
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        public bool AddUpdateLuceneIndex(SearchInfo data)
        {
            return AddUpdateLuceneIndex(new List<SearchInfo> { data });
        }

        private void AddToLuceneIndex(SearchInfo dataObject, IndexWriter writer)
        {
            var searchQuery = new TermQuery(new Term("Id", dataObject.Id.ToString()));
            writer.DeleteDocuments(searchQuery);
            var doc = CreateDoc(dataObject);
            writer.AddDocument(doc);
        }

        private Document CreateDoc(SearchInfo row)
        {
            var doc = new Document();
            doc.Add(new Field("Id", row.Id.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("LanguageCode", row.LanguageCode, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("CategoryId", row.CategoryId.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Type", row.Type.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("SearchId", row.SearchId, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Title", row.Title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Url", row.Alias, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Alias", row.Alias, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("VideoUrl", row.VideoUrl, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Sumary", row.Sumary, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Images", row.Images, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Tags", row.Tags, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("CreateDate", row.CreateDate.ToString(Extensions.Constants.DateTimeFomat), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Keyword", Utilities.GetCharUnsigned(row.Title), Field.Store.YES, Field.Index.ANALYZED));

            return doc;
        }

        public bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    writer.DeleteAll();
                    analyzer.Close();
                    writer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void Optimize()
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Close();
                writer.Dispose();
            }
        }

        public List<SearchInfo> Search(List<SearchCondition> conditions, int pgIndex, int pgSize, ref int nResult)
        {
            return Search(conditions, false, pgIndex, pgSize, ref nResult);
        }

        public List<SearchInfo> Search(SearchCondition condition, int pgIndex, int pgSize, ref int nResult)
        {
            return Search(new List<SearchCondition> { condition }, pgIndex, pgSize, ref nResult);
        }

        public List<SearchInfo> Search(List<SearchCondition> conditions, bool haveKeyword, int pgIndex, int pgSize, ref int nResult)
        {
            if (conditions == null || conditions.Count == 0)
            {
                throw new ArgumentNullException("searchTerms");
            }
            try
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
                var directory = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (!IndexReader.IndexExists(directory))
                {
                    return null;
                }

                var mainQuery = new BooleanQuery();
                foreach (var pair in conditions)
                {
                    string searchQuery = pair.Keyword;
                    if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", "")))
                    {
                        searchQuery = searchQuery.Replace("*", "").Replace("?", "");
                    }

                    for (int i = 0; i < pair.SearchField.Length; i++)
                    {
                        var query = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, pair.SearchField[i], analyzer).Parse(searchQuery);
                        mainQuery.Add(query, BooleanClause.Occur.SHOULD);
                    }
                }

                var searcher = new IndexSearcher(directory, true);
                try
                {
                    int endItem = pgIndex * pgSize;
                    int startItem = (pgIndex - 1) * pgSize;
                    var hits = searcher.Search(mainQuery, null, endItem, haveKeyword ? Sort.RELEVANCE : new Sort(CreateSort()));
                    nResult = hits.TotalHits;
                    return LuceneToDataList(hits.ScoreDocs.Skip(startItem).Take(pgSize), searcher).ToList();
                }
                finally
                {
                    searcher.Close();
                    analyzer.Close();
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public List<SearchInfo> Search(SearchCondition condition, bool haveKeyword, int pgIndex, int pgSize, ref int nResult)
        {
            return Search(new List<SearchCondition>() { condition }, haveKeyword, pgIndex, pgSize, ref nResult);
        }

        private SortField[] CreateSort()
        {
            var sortFields = new List<SortField>();
            sortFields.Add(new SortField(SearchField.Id.ToString(), SortField.LONG, true));
            sortFields.Add(new SortField(SearchField.LanguageCode.ToString(), SortField.STRING, true));
            sortFields.Add(new SortField(SearchField.CategoryId.ToString(), SortField.INT, true));
            sortFields.Add(new SortField(SearchField.Type.ToString(), SortField.INT, true));
            sortFields.Add(new SortField(SearchField.SearchId.ToString(), SortField.STRING, false));
            sortFields.Add(new SortField(SearchField.Title.ToString(), SortField.STRING, false));
            sortFields.Add(new SortField(SearchField.Url.ToString(), SortField.STRING, false));
            sortFields.Add(new SortField(SearchField.Alias.ToString(), SortField.STRING, false));
            sortFields.Add(new SortField(SearchField.Sumary.ToString(), SortField.STRING, false));
            sortFields.Add(new SortField(SearchField.Tags.ToString(), SortField.STRING, false));
            sortFields.Add(new SortField(SearchField.Keyword.ToString(), SortField.STRING, false));

            return sortFields.ToArray();
        }

        public int Count(SearchCondition condition)
        {
            return Count(new List<SearchCondition>() {condition});
        }

        public int Count(List<SearchCondition> conditions)
        {
            if (conditions == null || conditions.Count == 0)
                throw new ArgumentNullException("searchTerms");

            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            var directory = FSDirectory.Open(new DirectoryInfo(_luceneDir));
            if (!IndexReader.IndexExists(directory))
            {
                return 0;
            }

            var mainQuery = new BooleanQuery();
            foreach (var pair in conditions)
            {
                string searchQuery = pair.Keyword;
                if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", "")))
                {
                    searchQuery = searchQuery.Replace("*", "").Replace("?", "");
                }

                for (int i = 0; i < pair.SearchField.Length; i++)
                {
                    var query = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, pair.SearchField[i], analyzer).Parse(searchQuery);
                    mainQuery.Add(query, BooleanClause.Occur.MUST);
                }
            }
            var searcher = new IndexSearcher(directory, true);
            try
            {
                return searcher.Search(mainQuery, 1).TotalHits;
            }
            finally
            {
                searcher.Close();
                analyzer.Close();
            }
        }

        private static IEnumerable<SearchInfo> LuceneToDataList(IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {
            return hits.Select(hit => LuceneDocumentToData(searcher.Doc(hit.doc))).ToList();
        }

        private static SearchInfo LuceneDocumentToData(Document doc)
        {
            try
            {
                DateTime createDate = DateTime.ParseExact(doc.Get("CreateDate"), 
                    Extensions.Constants.DateTimeFomat,
                    System.Globalization.CultureInfo.InvariantCulture);
                var item = new SearchInfo
                {
                    Id = int.Parse(doc.Get("Id")),
                    LanguageCode = doc.Get("LanguageCode"),
                    CategoryId = int.Parse(doc.Get("CategoryId")),
                    Type = int.Parse(doc.Get("Type")),
                    SearchId = doc.Get("SearchId"),
                    Title = doc.Get("Title"),
                    Url = doc.Get("Url"),
                    Alias = doc.Get("Alias"),
                    VideoUrl = doc.Get("VideoUrl"),
                    Sumary = doc.Get("Sumary"),
                    Images = doc.Get("Images"),
                    Tags = doc.Get("Tags"),
                    CreateDate = createDate
                };

                return item;
            }
            catch (Exception)
            {
                return new SearchInfo();
            }
        }
    }
}