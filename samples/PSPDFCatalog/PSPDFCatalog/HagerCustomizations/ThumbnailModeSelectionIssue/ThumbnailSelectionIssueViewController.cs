using System;
using PSPDFKit.iOS;


namespace PSPDFCatalog.HagerCustomizations.ThumbnailModeSelectionIssue
{
    public class ThumbnailSelectionIssueViewController : PSPDFViewController
    {
        public ThumbnailSelectionIssueViewController(PSPDFDocument doc) : base(doc)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UpdateConfigurationWithoutReloading((builder) =>
            {
                builder.ThumbnailGrouping = PSPDFThumbnailGrouping.Automatic;
                builder.ShouldHideHUDOnPageChange = false;
                builder.HUDViewMode = PSPDFHUDViewMode.Always;
                builder.PageTransition = PSPDFPageTransition.Curl;
            });

            ThumbnailController.Delegate = new PSPDFThumbnailViewControllerDelegateCustom(this);
        }


        public void SelectPageForThumbnail(nuint page)
        {
            var message = string.Format("Selected page is: {0}", page);
            Console.WriteLine(message);

            Page = page;


            UpdateConfigurationWithoutReloading((builder) => builder.ThumbnailGrouping = PSPDFThumbnailGrouping.Automatic );

            ViewMode = PSPDFViewMode.Document;
        }
    }
}

