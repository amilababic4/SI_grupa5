namespace SmartLib.Infrastructure.Services
{
    public static class CacheKeyBuilder
    {
        // Catalog keys (books)
        public static string CatalogListKey(long version, int page, int pageSize, string? q = null, string? sort = null)
            => $"catalog:list:v{version}:page={page}:size={pageSize}:q={q ?? ""}:sort={sort ?? ""}";
        
        public static string CatalogBookKey(long version, int id)
            => $"catalog:book:v{version}:id={id}";
        
        public static string CatalogCategoriesKey(long version)
            => $"catalog:categories:v{version}";
        
        // Home page keys
        public static string HomeFeaturedKey(long version)
            => $"home:featured:v{version}";
        
        public static string HomeRandomKey(long version)
            => $"home:random:v{version}";
        
        public static string HomeRecommendationsKey(long version, int userId)
            => $"home:recommendations:v{version}:user={userId}";
    }
}
