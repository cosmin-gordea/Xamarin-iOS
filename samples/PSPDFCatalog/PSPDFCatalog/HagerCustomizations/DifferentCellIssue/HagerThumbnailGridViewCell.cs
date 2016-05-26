using System;
using System.Linq;
using UIKit;
using PSPDFKit.iOS;
using CoreGraphics;

namespace HagerCustomizations.DifferentCellIssue
{
    public class HagerThumbnailGridViewCell : PSPDFThumbnailGridViewCell
    {
        private const float LabelWidth = 30f;
        private UIImageView _bookmarkImageView;
        private bool _needsToRemoveCellRoundness = true;

        private readonly UIImage BookmarkFlag = UIImage.FromFile("HagerCustomizations/Images/bookmarkFlag.png");

        private readonly UIColor OrangeFlavour = UIColor.FromRGB(228, 103, 53);
        private readonly UIColor DarkGrayFlavour = UIColor.FromRGB(42, 42, 42);

        public HagerThumbnailGridViewCell(IntPtr handle) : base(handle)
        {
            SelectedBackgroundView.BackgroundColor = OrangeFlavour;
        }

        public override void LayoutSubviews()
        {
            if (_needsToRemoveCellRoundness)//one time only
            {
                _needsToRemoveCellRoundness = false;

                var background = this.Subviews.FirstOrDefault(v => v != ImageView);
                if (background != null)
                {
                    background.Layer.CornerRadius = 0;
                    background.BackgroundColor = OrangeFlavour;//should be the same as SelectedBackgroundView.BackgroundColor = OrangeFlavour;
                }
            }

            base.LayoutSubviews();
        }

        public override UIImageView BookmarkImageView
        {
            get
            {
                if (_bookmarkImageView != null) return _bookmarkImageView;

                _bookmarkImageView = new UIImageView(BookmarkFlag);
                Add(_bookmarkImageView);
                return _bookmarkImageView;
            }
        }

        public override void UpdateBookmarkImage()
        {
            base.UpdateBookmarkImage();

            if (BookmarkImageView != null)
            {
                var newLeft = ImageView.Frame.Left + 8;
                var newTop = ImageView.Frame.Top - 4;

                BookmarkImageView.Frame = new CGRect(newLeft, newTop, BookmarkImageView.Frame.Width, BookmarkImageView.Frame.Height);
            }
        }

        public override void UpdatePageLabel()
        {
            base.UpdatePageLabel();

            var roundedLabel = PageLabel as PSPDFRoundedLabel;
            if (roundedLabel != null)
            {
                roundedLabel.CornerRadius = 0f;//remove corner
                if (this.Selected)
                {
                    roundedLabel.RectColor = OrangeFlavour;
                }
                else
                {
                    roundedLabel.RectColor = DarkGrayFlavour;
                }
            }

            //change the center X; and the width - this one is lost while using no roundness for the corner
            var old = PageLabel.Frame;
            PageLabel.Frame = new CoreGraphics.CGRect(old.X - (LabelWidth - old.Width) / 2, old.Y, LabelWidth, old.Height);

            PageLabel.Text = (Page + 1).ToString();//Page is zero-base indexed

        }

        //do not enable PageLabel as we do not have the page info yet
        public override bool PageLabelEnabled
        {
            get
            {
                //return base.PageLabelEnabled;
                return true;
            }
            set
            {
                base.PageLabelEnabled = value;
            }
        }
    }
}