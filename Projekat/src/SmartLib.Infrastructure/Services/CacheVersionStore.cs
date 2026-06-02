using System.Threading;

namespace SmartLib.Infrastructure.Services
{
    public sealed class CacheVersionStore
    {
        private long _booksVersion = 1;
        private long _categoriesVersion = 1;
        private long _forumVersion = 1;
        private long _newsVersion = 1;
        private long _eventsVersion = 1;

        public long BooksVersion => Interlocked.Read(ref _booksVersion);
        public long CategoriesVersion => Interlocked.Read(ref _categoriesVersion);
        public long ForumVersion => Interlocked.Read(ref _forumVersion);
        public long NewsVersion => Interlocked.Read(ref _newsVersion);
        public long EventsVersion => Interlocked.Read(ref _eventsVersion);

        public long BumpBooksVersion() => Interlocked.Increment(ref _booksVersion);
        public long BumpCategoriesVersion() => Interlocked.Increment(ref _categoriesVersion);
        public long BumpForumVersion() => Interlocked.Increment(ref _forumVersion);
        public long BumpNewsVersion() => Interlocked.Increment(ref _newsVersion);
        public long BumpEventsVersion() => Interlocked.Increment(ref _eventsVersion);
    }
}
