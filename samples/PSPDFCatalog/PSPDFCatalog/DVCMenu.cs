﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using MonoTouch.Dialog;

using Foundation;
using UIKit;
using ObjCRuntime;

using PSPDFKit.iOS;
using HagerCustomizations.DifferentCellIssue;
using PSPDFCatalog.HagerCustomizations.ThumbnailModeSelectionIssue;

namespace PSPDFCatalog
{
	public partial class DVCMenu : DialogViewController
	{
		public static readonly string HackerMonthlyFile = "Pdf/hackermonthly-issue.pdf";
		public static readonly string ProtectedFile = "Pdf/protected.pdf";
		public static readonly string PSPDFKitFile = "Pdf/PSPDFKit QuickStart Guide.pdf";
        public static readonly string PdfHagerFile = "Pdf/hager.pdf";

		UIColor barColor;

		public DVCMenu () : base (UITableViewStyle.Grouped, null)
		{
			Root = new RootElement (PSPDFKitGlobal.SharedInstance.Version) {

                new Section ("Different cell size") {
                    new StringElement ("Hager document", () => {

                        var document = new PSPDFDocument (NSUrl.FromFilename (PdfHagerFile));
                        var pdfViewer = new DifferentCellSizeViewController (document);
                        NavigationController.PushViewController (pdfViewer, true);
                    }),
                    new StringElement ("PsPdfKit document", () => {

                        var document = new PSPDFDocument (NSUrl.FromFilename (PSPDFKitFile));
                        var pdfViewer = new DifferentCellSizeViewController (document);
                        NavigationController.PushViewController (pdfViewer, true);
                    })
                },

                new Section ("Page selection from Thumbnail View Controller") {
                    new StringElement ("Custom delegate", () => {

                        var document = new PSPDFDocument (NSUrl.FromFilename (PdfHagerFile));
                        var pdfViewer = new ThumbnailSelectionIssueViewController (document);
                        NavigationController.PushViewController (pdfViewer, true);
                    })
                },

				new Section ("Start here") {
					new StringElement ("PSPDFViewController Playground", () => {
						var pdfViewer = new PlayGroundViewController (NSUrl.FromFilename (PSPDFKitFile));
						NavigationController.PushViewController (pdfViewer, true);
					})
				},
				new Section ("Annotations"){
					new StringElement ("Annotations From Code", () => {
						// we use a NSData document here but it'll work even better with a file-based variant.
						NSError err;
						var documentData = NSData.FromUrl (NSUrl.FromFilename (HackerMonthlyFile), NSDataReadingOptions.Mapped, out err);
						var pdfViewer = new AnnotationsFromCodeViewController (documentData);
						NavigationController.PushViewController (pdfViewer, true);
					}),
				},
				new Section ("Password / Security", "Password is: test123") {
					new StringElement ("Password Preset", () => {
						var document = new PSPDFDocument (NSUrl.FromFilename (ProtectedFile));
						document.Unlock ("test123");
						var pdfViewer = new PSPDFViewController (document);
						NavigationController.PushViewController (pdfViewer, true);
					}),
					new StringElement ("Password Not Preset", () => {
						var document = new PSPDFDocument (NSUrl.FromFilename (ProtectedFile));
						var pdfViewer = new PSPDFViewController (document);
						NavigationController.PushViewController (pdfViewer, true);
					}),
					new StringElement ("Create Password Protected PDF", async () => {
						var document = new PSPDFDocument (NSUrl.FromFilename (HackerMonthlyFile));
						var status = PSPDFStatusHUDItem.GetProgressHud ("Preparing");
						status.Push (true, null);
						// Create temp file and password
						var tempPdf = NSUrl.FromFilename (Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString () + ".pdf"));
						var password = "test123";

						// We start a new task so this executes on a separated thread since it is a hevy task and we don't want to block the UI
						await Task.Factory.StartNew (()=> {
							NSError err;
							PSPDFProcessor.GeneratePdf (configuration: new PSPDFProcessorConfiguration (document), 
							                            saveOptions: new PSPDFProcessorSaveOptions (password, password, NSNumber.FromInt32 (128)),
							                            fileUrl: tempPdf,
							                            progressHandler: (currentPage, totalPages) => InvokeOnMainThread (() => status.Progress = (nfloat) currentPage / totalPages),
							                            error: out err);
						});
						InvokeOnMainThread (()=> {
							status.Pop (true, null);
							var docToShow = new PSPDFDocument (tempPdf);
							var pdfViewer = new PSPDFViewController (docToShow);
							NavigationController.PushViewController (pdfViewer, true);
						});
					}),
				},
				new Section ("Subclassing", "Examples how to subclass PSPDFKit."){
					new StringElement ("Annotation Link Editor", () => {
						var document = new PSPDFDocument (NSUrl.FromFilename (HackerMonthlyFile));
						var editableTypes = new NSSet<NSString> (
							PSPDFAnnotationString.Link, // Important!!
							PSPDFAnnotationString.Highlight,
							PSPDFAnnotationString.Underline,
							PSPDFAnnotationString.Squiggly,
							PSPDFAnnotationString.StrikeOut,
							PSPDFAnnotationString.Note,
							PSPDFAnnotationString.FreeText,
							PSPDFAnnotationString.Ink,
							PSPDFAnnotationString.Square,
							PSPDFAnnotationString.Circle,
							PSPDFAnnotationString.Stamp );

						var pdfViewer = new LinkEditorViewController (document, PSPDFConfiguration.FromConfigurationBuilder ((builder) => {
							builder.EditableAnnotationTypes = editableTypes;
						}));
						NavigationController.PushViewController (pdfViewer, true);
					}),
					new StringElement ("Capture Bookmarks", () => {
						var document = new PSPDFDocument (NSUrl.FromFilename (HackerMonthlyFile));
						document.OverrideClass (new Class (typeof (PSPDFBookmarkParser)), new Class (typeof (CustomBookmarkParser)));
						var pdfViewer = new PSPDFViewController (document);
						pdfViewer.NavigationItem.RightBarButtonItems = new [] { pdfViewer.SearchButtonItem, pdfViewer.OutlineButtonItem, pdfViewer.BookmarkButtonItem };
						NavigationController.PushViewController (pdfViewer, true);
					}),
					new StringElement ("Change link background color to red", () => {
						var document = new PSPDFDocument (NSUrl.FromFilename (HackerMonthlyFile));
						// Note: You can also globally change the color using:
						// PSPDFLinkAnnotationView.SetGlobalBorderColor = UIColor.Green;
						// We don't use this in the example here since it would change the color globally for all examples.
						var pdfViewer = new PSPDFViewController (document, PSPDFConfiguration.FromConfigurationBuilder ((builder) => {
							builder.OverrideClass (new Class (typeof (PSPDFLinkAnnotationView)), new Class (typeof (CustomLinkAnnotationView)));
						}));
						NavigationController.PushViewController (pdfViewer, true);
					}),
					new StringElement ("Custom AnnotationProvider", () => {
						var document = new PSPDFDocument (NSUrl.FromFilename (HackerMonthlyFile));
						document.DidCreateDocumentProviderHandler = (documentProvider => {
							documentProvider.AnnotationManager.AnnotationProviders = new IPSPDFAnnotationProvider[] { new CustomAnnotationProvider (document), documentProvider.AnnotationManager.FileAnnotationProvider };
						});
						var pdfViewer = new PSPDFViewController (document);
						NavigationController.PushViewController (pdfViewer, true);
					}),
					new StringElement ("Custom Document", () => {
						var pdfViewer = new PSPDFViewController ();
						var document = new CustomPDFDocument (NSUrl.FromFilename (HackerMonthlyFile));
						pdfViewer.Document = document;
						NavigationController.PushViewController (pdfViewer, true);
					})
				},
				new Section ("PSPDFViewController Customization"){
					new StringElement ("Custom Google Text Selection Menu", () => {
						var pdfViewer = new PSCustomTextSelectionMenuController {
							Document = new PSPDFDocument (NSUrl.FromFilename (HackerMonthlyFile))
						};
						NavigationController.PushViewController (pdfViewer, true);
					}),
					new StringElement ("Simple Drawing Button", () => {
						var document = new PSPDFDocument (NSUrl.FromFilename (HackerMonthlyFile)) {
							AnnotationSaveMode = PSPDFAnnotationSaveMode.Disabled
						};
						var pdfViewer = new PSCSimpleDrawingPDFViewController (document);
						NavigationController.PushViewController (pdfViewer, true);
					}),
					new StringElement ("Stylus Support", () => {
						var document = new PSPDFDocument (NSUrl.FromFilename (HackerMonthlyFile));
						var pdfViewer = new PSPDFViewController (document, PSPDFConfiguration.FromConfigurationBuilder ((builder) => {
							builder.OverrideClass (new Class (typeof (PSPDFAnnotationToolbar)), new Class (typeof (PSCStylusEnabledAnnotationToolbar)));
						}));
						NavigationController.PushViewController (pdfViewer, true);
					})
				},

			};
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Apply the PSPDFKit blue
			barColor = UIColor.FromRGBA (0.110f, 0.529f, 0.757f, 1f);
			NavigationController.NavigationBar.BarTintColor = barColor;
			NavigationController.Toolbar.TintColor = barColor;
			NavigationController.View.TintColor = UIColor.White;
			NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes () { ForegroundColor = UIColor.White };
			NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
			NavigationController.SetToolbarHidden (true, animated);
		}

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}
	}
}
