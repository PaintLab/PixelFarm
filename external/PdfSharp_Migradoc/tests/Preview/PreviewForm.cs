#region PDFsharp - A .NET library for processing PDF
//
// Copyright (c) 2005-2009 empira Software GmbH, Cologne (Germany)
//
// http://www.pdfsharp.com
//
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF THIRD PARTY RIGHTS.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Forms;

namespace Preview
{
  /// <summary>
  /// A form that shows the use of PdfSharp.Forms.PagePreview.
  /// </summary>
  public class PreviewForm : System.Windows.Forms.Form
  {
    private System.ComponentModel.IContainer components;
    private System.Windows.Forms.StatusBar statusBar;
    private System.Windows.Forms.ToolBarButton tbbFirstPage;
    private System.Windows.Forms.ToolBarButton tbbSeparator1;
    private System.Windows.Forms.ImageList ilToolbar;
    private System.Windows.Forms.ToolBarButton tbbPrevPage;
    private System.Windows.Forms.ToolBarButton tbbNextPage;
    private System.Windows.Forms.ToolBarButton tbbLastPage;
    private System.Windows.Forms.ToolBarButton tbbSeparator2;
    private System.Windows.Forms.ToolBarButton tbbOriginalSize;
    private System.Windows.Forms.ToolBarButton tbbFullPage;
    private System.Windows.Forms.ToolBarButton tbbBestFit;
    private System.Windows.Forms.ToolBarButton tbbSmaller;
    private System.Windows.Forms.ToolBarButton tbbLarger;
    private System.Windows.Forms.ToolBarButton tbbSeparator3;
    private System.Windows.Forms.ToolBarButton tbbMakePdf;
    private System.Windows.Forms.ToolBar toolBar;
    private System.Windows.Forms.MenuItem menuItem10;
    private System.Windows.Forms.MenuItem miPercent800;
    private System.Windows.Forms.MenuItem miPercent600;
    private System.Windows.Forms.MenuItem miPercent400;
    private System.Windows.Forms.MenuItem miPercent200;
    private System.Windows.Forms.MenuItem miPercent150;
    private System.Windows.Forms.MenuItem miPercent75;
    private System.Windows.Forms.MenuItem miPercent50;
    private System.Windows.Forms.MenuItem miPercent25;
    private System.Windows.Forms.MenuItem miPercent10;
    private System.Windows.Forms.MenuItem miBestFit;
    private System.Windows.Forms.MenuItem miFullPage;
    private System.Windows.Forms.ContextMenu menuZoom;
    private System.Windows.Forms.MenuItem miPercent100;
    private ToolBarButton tbbPrint;
    private PdfSharp.Forms.PagePreview pagePreview;

    public PreviewForm()
    {
      InitializeComponent();
      this.pagePreview.PageSize = PageSizeConverter.ToSize(PageSize.A4);
      UpdateStatusBar();
    }

    public PagePreview.RenderEvent RenderEvent
    {
      get { return this.renderEvent; }
      set
      {
        this.pagePreview.SetRenderEvent(value);
        this.renderEvent = value;
      }
    }
    PagePreview.RenderEvent renderEvent;

    public void UpdateDrawing()
    {
      this.pagePreview.Invalidate();
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
          components.Dispose();
      }
      base.Dispose(disposing);
    }

    /// <summary>
    /// Prints the document to the default printer.
    /// See MSDN documentation for information about printer selection and printer settings.
    /// </summary>
    void Print()
    {
      PrintDocument pd = new PrintDocument();
      pd.PrintPage += new PrintPageEventHandler(PrintPage);
      pd.Print();
    }

    /// <summary>
    /// Draws the page on the printer.
    /// </summary>
    private void PrintPage(object sender, PrintPageEventArgs ev)
    {
      Graphics graphics = ev.Graphics;
      graphics.PageUnit = GraphicsUnit.Point;
      XGraphics gfx = XGraphics.FromGraphics(graphics, PageSizeConverter.ToSize(PageSize.A4));
      if (this.renderEvent != null)
        this.renderEvent(gfx);

      ev.HasMorePages = false;
    }

    /// <summary>
    /// Create a new PDF document and start the viewer.
    /// </summary>
    void MakePdf()
    {
      string filename = Guid.NewGuid().ToString().ToUpper() + ".pdf";
      PdfDocument document = new PdfDocument(filename);
      document.Info.Creator = "Preview Sample";
      PdfPage page = document.AddPage();
      page.Size = PageSize.A4;
      XGraphics gfx = XGraphics.FromPdfPage(page);

      if (this.renderEvent != null)
        this.renderEvent(gfx);
      document.Close();
      Process.Start(filename);
    }

    static int GetNewZoom(int currentZoom, bool larger)
    {
      int[] values = new int[]
      {
        10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 120, 140, 160, 180, 200, 
        250, 300, 350, 400, 450, 500, 600, 700, 800
      };

      if (currentZoom <= (int)Zoom.Mininum && !larger)
        return (int)Zoom.Mininum;
      if (currentZoom >= (int)Zoom.Maximum && larger)
        return (int)Zoom.Maximum;

      if (larger)
      {
        for (int i = 0; i < values.Length; i++)
        {
          if (currentZoom < values[i])
            return values[i];
        }
      }
      else
      {
        for (int i = values.Length - 1; i >= 0; i--)
        {
          if (currentZoom > values[i])
            return values[i];
        }
      }
      return (int)Zoom.Percent100;
    }

    void UpdateStatusBar()
    {
      string status = String.Format("PageSize: {0}pt x {1}pt, Zoom: {2}%",
        this.pagePreview.PageSize.Width,
        this.pagePreview.PageSize.Height,
        this.pagePreview.ZoomPercent);
      this.statusBar.Text = status;
    }


    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PreviewForm));
      this.toolBar = new System.Windows.Forms.ToolBar();
      this.tbbSeparator1 = new System.Windows.Forms.ToolBarButton();
      this.tbbFirstPage = new System.Windows.Forms.ToolBarButton();
      this.tbbPrevPage = new System.Windows.Forms.ToolBarButton();
      this.tbbNextPage = new System.Windows.Forms.ToolBarButton();
      this.tbbLastPage = new System.Windows.Forms.ToolBarButton();
      this.tbbSeparator2 = new System.Windows.Forms.ToolBarButton();
      this.tbbOriginalSize = new System.Windows.Forms.ToolBarButton();
      this.menuZoom = new System.Windows.Forms.ContextMenu();
      this.miPercent800 = new System.Windows.Forms.MenuItem();
      this.miPercent600 = new System.Windows.Forms.MenuItem();
      this.miPercent400 = new System.Windows.Forms.MenuItem();
      this.miPercent200 = new System.Windows.Forms.MenuItem();
      this.miPercent150 = new System.Windows.Forms.MenuItem();
      this.miPercent100 = new System.Windows.Forms.MenuItem();
      this.miPercent75 = new System.Windows.Forms.MenuItem();
      this.miPercent50 = new System.Windows.Forms.MenuItem();
      this.miPercent25 = new System.Windows.Forms.MenuItem();
      this.miPercent10 = new System.Windows.Forms.MenuItem();
      this.menuItem10 = new System.Windows.Forms.MenuItem();
      this.miBestFit = new System.Windows.Forms.MenuItem();
      this.miFullPage = new System.Windows.Forms.MenuItem();
      this.tbbFullPage = new System.Windows.Forms.ToolBarButton();
      this.tbbBestFit = new System.Windows.Forms.ToolBarButton();
      this.tbbSmaller = new System.Windows.Forms.ToolBarButton();
      this.tbbLarger = new System.Windows.Forms.ToolBarButton();
      this.tbbSeparator3 = new System.Windows.Forms.ToolBarButton();
      this.tbbPrint = new System.Windows.Forms.ToolBarButton();
      this.tbbMakePdf = new System.Windows.Forms.ToolBarButton();
      this.ilToolbar = new System.Windows.Forms.ImageList(this.components);
      this.statusBar = new System.Windows.Forms.StatusBar();
      this.pagePreview = new PdfSharp.Forms.PagePreview();
      this.SuspendLayout();
      // 
      // toolBar
      // 
      this.toolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
      this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
                                                                               this.tbbSeparator1,
                                                                               this.tbbFirstPage,
                                                                               this.tbbPrevPage,
                                                                               this.tbbNextPage,
                                                                               this.tbbLastPage,
                                                                               this.tbbSeparator2,
                                                                               this.tbbOriginalSize,
                                                                               this.tbbFullPage,
                                                                               this.tbbBestFit,
                                                                               this.tbbSmaller,
                                                                               this.tbbLarger,
                                                                               this.tbbSeparator3,
                                                                               this.tbbPrint,
                                                                               this.tbbMakePdf});
      this.toolBar.DropDownArrows = true;
      this.toolBar.ImageList = this.ilToolbar;
      this.toolBar.Location = new System.Drawing.Point(0, 0);
      this.toolBar.Name = "toolBar";
      this.toolBar.ShowToolTips = true;
      this.toolBar.Size = new System.Drawing.Size(638, 42);
      this.toolBar.TabIndex = 1;
      this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
      // 
      // tbbSeparator1
      // 
      this.tbbSeparator1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
      this.tbbSeparator1.Visible = false;
      // 
      // tbbFirstPage
      // 
      this.tbbFirstPage.ImageIndex = 0;
      this.tbbFirstPage.Tag = "FirstPage";
      this.tbbFirstPage.Visible = false;
      // 
      // tbbPrevPage
      // 
      this.tbbPrevPage.Enabled = false;
      this.tbbPrevPage.ImageIndex = 1;
      this.tbbPrevPage.Tag = "PrevPage";
      this.tbbPrevPage.Visible = false;
      // 
      // tbbNextPage
      // 
      this.tbbNextPage.Enabled = false;
      this.tbbNextPage.ImageIndex = 2;
      this.tbbNextPage.Tag = "NextPage";
      this.tbbNextPage.Visible = false;
      // 
      // tbbLastPage
      // 
      this.tbbLastPage.Enabled = false;
      this.tbbLastPage.ImageIndex = 3;
      this.tbbLastPage.Tag = "LastPage";
      this.tbbLastPage.Visible = false;
      // 
      // tbbSeparator2
      // 
      this.tbbSeparator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
      this.tbbSeparator2.Visible = false;
      // 
      // tbbOriginalSize
      // 
      this.tbbOriginalSize.DropDownMenu = this.menuZoom;
      this.tbbOriginalSize.ImageIndex = 4;
      this.tbbOriginalSize.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
      this.tbbOriginalSize.Tag = "OriginalSize";
      this.tbbOriginalSize.Text = "Original Size";
      // 
      // menuZoom
      // 
      this.menuZoom.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                             this.miPercent800,
                                                                             this.miPercent600,
                                                                             this.miPercent400,
                                                                             this.miPercent200,
                                                                             this.miPercent150,
                                                                             this.miPercent100,
                                                                             this.miPercent75,
                                                                             this.miPercent50,
                                                                             this.miPercent25,
                                                                             this.miPercent10,
                                                                             this.menuItem10,
                                                                             this.miBestFit,
                                                                             this.miFullPage});
      // 
      // miPercent800
      // 
      this.miPercent800.Index = 0;
      this.miPercent800.Text = "800%";
      this.miPercent800.Click += new System.EventHandler(this.miPercent800_Click);
      // 
      // miPercent600
      // 
      this.miPercent600.Index = 1;
      this.miPercent600.Text = "600%";
      this.miPercent600.Click += new System.EventHandler(this.miPercent600_Click);
      // 
      // miPercent400
      // 
      this.miPercent400.Index = 2;
      this.miPercent400.Text = "400%";
      this.miPercent400.Click += new System.EventHandler(this.miPercent400_Click);
      // 
      // miPercent200
      // 
      this.miPercent200.Index = 3;
      this.miPercent200.Text = "200%";
      this.miPercent200.Click += new System.EventHandler(this.miPercent200_Click);
      // 
      // miPercent150
      // 
      this.miPercent150.Index = 4;
      this.miPercent150.Text = "150%";
      this.miPercent150.Click += new System.EventHandler(this.miPercent150_Click);
      // 
      // miPercent100
      // 
      this.miPercent100.Index = 5;
      this.miPercent100.Text = "100%";
      this.miPercent100.Click += new System.EventHandler(this.miPercent100_Click);
      // 
      // miPercent75
      // 
      this.miPercent75.Index = 6;
      this.miPercent75.Text = "75%";
      this.miPercent75.Click += new System.EventHandler(this.miPercent75_Click);
      // 
      // miPercent50
      // 
      this.miPercent50.Index = 7;
      this.miPercent50.Text = "50%";
      this.miPercent50.Click += new System.EventHandler(this.miPercent50_Click);
      // 
      // miPercent25
      // 
      this.miPercent25.Index = 8;
      this.miPercent25.Text = "25%";
      this.miPercent25.Click += new System.EventHandler(this.miPercent25_Click);
      // 
      // miPercent10
      // 
      this.miPercent10.Index = 9;
      this.miPercent10.Text = "10%";
      this.miPercent10.Click += new System.EventHandler(this.miPercent10_Click);
      // 
      // menuItem10
      // 
      this.menuItem10.Index = 10;
      this.menuItem10.Text = "-";
      // 
      // miBestFit
      // 
      this.miBestFit.Index = 11;
      this.miBestFit.Text = "Best fit";
      this.miBestFit.Click += new System.EventHandler(this.miBestFit_Click);
      // 
      // miFullPage
      // 
      this.miFullPage.Index = 12;
      this.miFullPage.Text = "Full Page";
      this.miFullPage.Click += new System.EventHandler(this.miFullPage_Click);
      // 
      // tbbFullPage
      // 
      this.tbbFullPage.ImageIndex = 5;
      this.tbbFullPage.Tag = "FullPage";
      this.tbbFullPage.Text = "Full Page";
      // 
      // tbbBestFit
      // 
      this.tbbBestFit.ImageIndex = 6;
      this.tbbBestFit.Tag = "BestFit";
      this.tbbBestFit.Text = "Best Fit";
      // 
      // tbbSmaller
      // 
      this.tbbSmaller.ImageIndex = 7;
      this.tbbSmaller.Tag = "Smaller";
      this.tbbSmaller.Text = "Smaller";
      // 
      // tbbLarger
      // 
      this.tbbLarger.ImageIndex = 8;
      this.tbbLarger.Tag = "Larger";
      this.tbbLarger.Text = "Larger";
      // 
      // tbbSeparator3
      // 
      this.tbbSeparator3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
      // 
      // tbbPrint
      // 
      this.tbbPrint.ImageIndex = 10;
      this.tbbPrint.Tag = "Print";
      this.tbbPrint.Text = "Print";
      // 
      // tbbMakePdf
      // 
      this.tbbMakePdf.ImageIndex = 9;
      this.tbbMakePdf.Tag = "MakePdf";
      this.tbbMakePdf.Text = "Create PDF";
      // 
      // ilToolbar
      // 
      this.ilToolbar.ImageSize = new System.Drawing.Size(16, 16);
      this.ilToolbar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilToolbar.ImageStream")));
      this.ilToolbar.TransparentColor = System.Drawing.Color.Lime;
      // 
      // statusBar
      // 
      this.statusBar.Location = new System.Drawing.Point(0, 624);
      this.statusBar.Name = "statusBar";
      this.statusBar.Size = new System.Drawing.Size(638, 22);
      this.statusBar.TabIndex = 2;
      // 
      // pagePreview
      // 
      this.pagePreview.BackColor = System.Drawing.SystemColors.Control;
      this.pagePreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.pagePreview.DesktopColor = System.Drawing.SystemColors.ControlDark;
      this.pagePreview.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pagePreview.Location = new System.Drawing.Point(0, 42);
      this.pagePreview.Name = "pagePreview";
      this.pagePreview.PageColor = System.Drawing.Color.GhostWhite;
      this.pagePreview.PageSizeF = new System.Drawing.Size(595, 842);
      this.pagePreview.Size = new System.Drawing.Size(638, 582);
      this.pagePreview.TabIndex = 4;
      this.pagePreview.Zoom = PdfSharp.Forms.Zoom.BestFit;
      this.pagePreview.ZoomPercent = 77;
      // 
      // PreviewForm
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(638, 646);
      this.Controls.Add(this.pagePreview);
      this.Controls.Add(this.statusBar);
      this.Controls.Add(this.toolBar);
      this.Name = "PreviewForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Preview Sample";
      this.ResumeLayout(false);

    }
    #endregion

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      UpdateStatusBar();
    }

    private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
    {
      object tag = e.Button.Tag;
      if (tag != null)
      {
        switch (tag.ToString())
        {
          case "OriginalSize":
            this.pagePreview.Zoom = Zoom.OriginalSize;
            break;

          case "FullPage":
            this.pagePreview.Zoom = Zoom.FullPage;
            break;

          case "BestFit":
            this.pagePreview.Zoom = Zoom.BestFit;
            break;

          case "Smaller":
            this.pagePreview.ZoomPercent = GetNewZoom(this.pagePreview.ZoomPercent, false);
            break;

          case "Larger":
            this.pagePreview.ZoomPercent = GetNewZoom(this.pagePreview.ZoomPercent, true);
            break;

          case "Print":
            Print();
            break;

          case "MakePdf":
            MakePdf();
            break;
        }
        UpdateStatusBar();
      }
    }

    private void miPercent800_Click(object sender, System.EventArgs e)
    {
      this.pagePreview.Zoom = Zoom.Percent800;
      UpdateStatusBar();
    }

    private void miPercent600_Click(object sender, System.EventArgs e)
    {
      this.pagePreview.Zoom = Zoom.Percent600;
      UpdateStatusBar();
    }

    private void miPercent400_Click(object sender, System.EventArgs e)
    {
      this.pagePreview.Zoom = Zoom.Percent400;
      UpdateStatusBar();
    }

    private void miPercent200_Click(object sender, System.EventArgs e)
    {
      this.pagePreview.Zoom = Zoom.Percent200;
      UpdateStatusBar();
    }

    private void miPercent100_Click(object sender, System.EventArgs e)
    {
      this.pagePreview.Zoom = Zoom.Percent100;
      UpdateStatusBar();
    }

    private void miPercent150_Click(object sender, System.EventArgs e)
    {
      this.pagePreview.Zoom = Zoom.Percent150;
      UpdateStatusBar();
    }

    private void miPercent75_Click(object sender, System.EventArgs e)
    {
      this.pagePreview.Zoom = Zoom.Percent75;
      UpdateStatusBar();
    }

    private void miPercent50_Click(object sender, System.EventArgs e)
    {
      this.pagePreview.Zoom = Zoom.Percent50;
      UpdateStatusBar();
    }

    private void miPercent25_Click(object sender, System.EventArgs e)
    {
      this.pagePreview.Zoom = Zoom.Percent25;
      UpdateStatusBar();
    }

    private void miPercent10_Click(object sender, System.EventArgs e)
    {
      this.pagePreview.Zoom = Zoom.Percent10;
      UpdateStatusBar();
    }

    private void miBestFit_Click(object sender, System.EventArgs e)
    {
      this.pagePreview.Zoom = Zoom.BestFit;
      UpdateStatusBar();
    }

    private void miFullPage_Click(object sender, System.EventArgs e)
    {
      this.pagePreview.Zoom = Zoom.FullPage;
      UpdateStatusBar();
    }

  }
}
