namespace ArticleManagement.Model 
{ 
    public class PagingParameter
    {
        public string? FilterText { get; set; }
        public string? Language { get; set; }
        public string? Header {get;set;}
        public string? HeaderEn {get;set;}
        public string? SubHeader {get;set;}
        public string? SubHeaderEn {get;set;}
        public bool? IsDeleted {get;set;}
        
        const int maxPageSize=50;
        public int PageNumber {get;set;}=1;

        private int _pageSize=10; 
        public int PageSize {get=>_pageSize;set=>_pageSize=value>maxPageSize?maxPageSize:value;}
    }
}