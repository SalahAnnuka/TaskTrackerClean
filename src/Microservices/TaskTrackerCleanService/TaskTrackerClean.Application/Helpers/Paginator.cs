namespace TaskTrackerClean.Application.Helpers
{
    public static class Paginator
    {
        public static object ToPagedResult<T>(
            IEnumerable<T> items,
            int currentPage,
            int pageSize,
            int totalItems,
            int totalPages)
        {
            return new
            {
                page = currentPage,
                pageContent = items,
                numberOfPages = totalPages,
                //next = currentPage < totalPages ? $"http://localhost:5036/api/Projects?page={currentPage + 1}&pageSize={pageSize}" : null,
                //prev = currentPage > 1 ? $"http://localhost:5036/api/Projects?page={currentPage - 1}&pageSize={pageSize}" : null
            };
        }
    }
}
