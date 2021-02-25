#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Infrastructure.Controls
{
  /// <summary>
  ///   Interaction logic for UcControlHeader.xaml
  /// </summary>
  public partial class UcControlHeader
  {
    public UcControlHeader()
    {
      InitializeComponent();

      MouseLeftButtonDown += (o,
                              e) =>
                               {
                                 var bs = DataContext as BServiceWorkspaceViewModel;

                                 if (bs != null)
                                 {
                                   try
                                   {
                                     var parent = VisualTreeHelper.GetParent(this) as DockPanel;
                                     var grMain = parent.Parent as Grid;
                                     var border = grMain.Parent;

                                     var bitmapsource = captureVisualBitmap((Visual)border, 100, 60);

                                     DragSourceHelper.DoDragDrop(this,
                                                                 bitmapsource,
                                                                 e.GetPosition(this),
                                                                 DragDropEffects.Move,
                                                                 new KeyValuePair<string, object>(
                                                                   bs.PositionInGrid.GetType().ToString(),
                                                                   bs.PositionInGrid));
                                   }
                                   catch (Exception ex)
                                   {
                                     TraceHelper.Trace(this,
                                                       (ex));
                                   }
                                 }
                               };



    }

    private static BitmapSource captureVisualBitmap(Visual targetVisual, double dpiX, double dpiY)
    {
      var bounds = VisualTreeHelper.GetDescendantBounds(targetVisual);
      var renderTargetBitmap = new RenderTargetBitmap(
        (int)(bounds.Width * dpiX / 96.0),
        (int)(bounds.Height * dpiY / 96.0),
        dpiX,
        dpiY,

        //PixelFormats.Default
        PixelFormats.Pbgra32
        );

      var drawingVisual = new DrawingVisual();
      using (DrawingContext drawingContext = drawingVisual.RenderOpen())
      {
        var visualBrush = new VisualBrush(targetVisual);
        drawingContext.DrawRectangle(visualBrush, null, new Rect(new System.Windows.Point(), bounds.Size));
      }
      renderTargetBitmap.Render(drawingVisual);

      return renderTargetBitmap;
    }

    public enum ImageFormats
    {
      PNG,
      BMP,
      JPG
    }
  }
}