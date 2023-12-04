using System;

namespace WebApp.ViewModels
{
    //Класс для хранения информации о страницах разбиения
    public class PageViewModel
    {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }

        public PageViewModel(int count, int pageNumber=1, int pageSize=50)
        {
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            if(pageNumber > TotalPages || pageNumber < 1)
            {
                PageNumber = 1;
            }
            else
            {
                PageNumber = pageNumber;
            }
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageNumber < TotalPages);
            }
        }
    }
}
