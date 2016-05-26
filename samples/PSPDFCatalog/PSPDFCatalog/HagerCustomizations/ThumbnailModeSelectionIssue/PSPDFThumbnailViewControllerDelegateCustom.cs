using System;
using PSPDFKit.iOS;

namespace PSPDFCatalog.HagerCustomizations.ThumbnailModeSelectionIssue
{
    public class PSPDFThumbnailViewControllerDelegateCustom : PSPDFThumbnailViewControllerDelegate
    {
        private ThumbnailSelectionIssueViewController _pdfViewer;

        public PSPDFThumbnailViewControllerDelegateCustom(ThumbnailSelectionIssueViewController pdfViewer)
        {
            _pdfViewer = pdfViewer;
        }

        public override void DidSelectPage(PSPDFThumbnailViewController thumbnailViewController, nuint page, PSPDFDocument document)
        {
            _pdfViewer.SelectPageForThumbnail(page);
        }
    }
}