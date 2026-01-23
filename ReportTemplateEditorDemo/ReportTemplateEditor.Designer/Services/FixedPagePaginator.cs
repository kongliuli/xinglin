using System;
using System.Windows;
using System.Windows.Documents;

namespace ReportTemplateEditor.Designer.Services
{
    public class FixedPagePaginator : DocumentPaginator
    {
        private readonly FixedPage _fixedPage;
        private readonly Size _pageSize;

        public FixedPagePaginator(FixedPage fixedPage)
        {
            _fixedPage = fixedPage ?? throw new ArgumentNullException(nameof(fixedPage));
            _pageSize = new Size(fixedPage.Width, fixedPage.Height);
        }

        public override bool IsPageCountValid => true;

        public override int PageCount => 1;

        public override Size PageSize
        {
            get => _pageSize;
            set => throw new NotSupportedException();
        }

        public override IDocumentPaginatorSource Source => null;

        public override DocumentPage GetPage(int pageNumber)
        {
            if (pageNumber != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            }
            return new DocumentPage(_fixedPage);
        }
    }
}