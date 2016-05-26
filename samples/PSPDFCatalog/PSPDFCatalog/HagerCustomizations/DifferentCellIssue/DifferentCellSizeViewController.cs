using Foundation;
using ObjCRuntime;
using PSPDFKit.iOS;

namespace HagerCustomizations.DifferentCellIssue
{
    public class DifferentCellSizeViewController : PSPDFViewController
    {
        public DifferentCellSizeViewController(PSPDFDocument doc) : base(doc)
        {
            UpdateConfigurationWithoutReloading((builder) =>
                {
                    builder.OverrideClass(new Class(typeof(PSPDFThumbnailGridViewCell)), new Class(typeof(HagerThumbnailGridViewCell)));
                });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UpdateConfiguration((builder) =>
                {
                    builder.PageTransition = PSPDFPageTransition.Curl;
                    builder.ThumbnailBarMode = PSPDFThumbnailBarMode.Scrollable;
                    builder.Margin = new UIKit.UIEdgeInsets(80, 0, 150, 0);

                    builder.PageMode = PSPDFPageMode.Single;
                    builder.PageLabelEnabled = false;
                    builder.ThumbnailGrouping = PSPDFThumbnailGrouping.Automatic;
                    builder.ShouldHideHUDOnPageChange = false;
                });
        }
    }
}