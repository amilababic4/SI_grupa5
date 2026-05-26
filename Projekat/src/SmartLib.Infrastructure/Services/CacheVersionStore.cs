using System.Threading;

namespace SmartLib.Infrastructure.Services
{
    public sealed class CacheVersionStore
    {
        private long _booksVersion = 1;
        private long _categoriesVersion = 1;
        private long _forumVersion = 1;

        public long BooksVersion => Interlocked.Read(ref _booksVersion);
        public long CategoriesVersion => Interlocked.Read(ref _categoriesVersion);
        public long ForumVersion => Interlocked.Read(ref _forumVersion);

        public long BumpBooksVersion() => Interlocked.Increment(ref _booksVersion);
        public long BumpCategoriesVersion() => Interlocked.Increment(ref _categoriesVersion);
        public long BumpForumVersion() => Interlocked.Increment(ref _forumVersion);
    }
}
