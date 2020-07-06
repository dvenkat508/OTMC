using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace OTMC.Pages
{
    /// <summary>
    /// Interaction logic for Crop.xaml
    /// </summary>
    public partial class Crop : Page
    {
        private BitmapImage bitmapImage = new BitmapImage();
        private bool first = true;
        private Point LastPoint;
        private bool DragInProgress = false;
        private double image_width;
        private double image_height;
        private double start_x;
        private double start_y;
        private double new_x;
        private double new_y;
        private enum HitType
        {
            None, L, R, T, B, Body
        };

        HitType MouseHitType = HitType.None;

        public Crop()
        {
            InitializeComponent();
        }



        private void OpenFile()
        {
            OpenFileDialog OpenPicker = new OpenFileDialog();
            Crop_Area.Opacity = 0;
            Top.Opacity = 0;
            Bottom.Opacity = 0;
            Left.Opacity = 0;
            Right.Opacity = 0;

            OpenPicker.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (OpenPicker.ShowDialog() == true)
            {
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(OpenPicker.FileName);
                bitmapImage.EndInit();
                Image.Source = bitmapImage;
            }
            else
            {
                Create newPage = new Create();
                this.NavigationService.Navigate(newPage);
            }
        }

        private HitType SetHitType(Ellipse rect, Point point)
        {
            double left = Canvas.GetLeft(Crop_Area);
            double top = Canvas.GetTop(Crop_Area);
            double right = left + Crop_Area.Width;
            double bottom = top + Crop_Area.Height;
            if (point.X < left) return HitType.None;
            if (point.X > right) return HitType.None;
            if (point.Y < top) return HitType.None;
            if (point.Y > bottom) return HitType.None;

            const double GAP = 10;
            if (point.X - left < GAP)
            {
                return HitType.L;
            }
            if (right - point.X < GAP)
            {
                return HitType.R;
            }
            if (point.Y - top < GAP) return HitType.T;
            if (bottom - point.Y < GAP) return HitType.B;
            return HitType.Body;
        }

        private void SetMouseCursor()
        {
            // See what cursor we should display.
            Cursor desired_cursor = Cursors.Arrow;
            switch (MouseHitType)
            {
                case HitType.None:
                    desired_cursor = Cursors.Arrow;
                    break;
                case HitType.Body:
                    desired_cursor = Cursors.ScrollAll;
                    break;
                case HitType.T:
                case HitType.B:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case HitType.L:
                case HitType.R:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            // Display the desired cursor.
            if (Cursor != desired_cursor) Cursor = desired_cursor;
        }

        private void GetSize()
        {
            image_width = Image.ActualWidth;
            image_height = Image.ActualHeight;
            start_x = ((SystemParameters.PrimaryScreenWidth - image_width) / 2);
            start_y = ((SystemParameters.PrimaryScreenHeight - image_height) / 2) - 130;
            Canvas.SetLeft(Crop_Area, start_x);
            Canvas.SetTop(Crop_Area, start_y);
            Canvas.SetLeft(Top, start_x);
            Canvas.SetTop(Top, start_y);
            Canvas.SetLeft(Left, start_x);
            Canvas.SetTop(Left, start_y);
            Canvas.SetLeft(Right, start_x);
            Canvas.SetTop(Right, start_y);
            Canvas.SetLeft(Bottom, start_x);
            Canvas.SetTop(Bottom, start_y);
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            if (first)
            {
                GetSize();
                Crop_Area.Opacity = 100;
                Top.Opacity = 100;
                Bottom.Opacity = 100;
                Left.Opacity = 100;
                Right.Opacity = 100;
                first = false;
            }
        }
        private void canvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseHitType = SetHitType(Crop_Area, Mouse.GetPosition(canvas1));
            SetMouseCursor();
            if (MouseHitType == HitType.None) return;
            LastPoint = Mouse.GetPosition(canvas1);
            DragInProgress = true;
        }

        // If a drag is in progress, continue the drag.
        // Otherwise display the correct cursor.
        private void canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!DragInProgress)
            {
                MouseHitType = SetHitType(Crop_Area, Mouse.GetPosition(canvas1));
                SetMouseCursor();
            }
            else
            {
                // See how much the mouse has moved.
                Point point = Mouse.GetPosition(canvas1);
                double offset_x = point.X - LastPoint.X;
                double offset_y = point.Y - LastPoint.Y;

                // Get the rectangle's current position.
                new_x = Canvas.GetLeft(Crop_Area);
                new_y = Canvas.GetTop(Crop_Area);
                double tx = Canvas.GetLeft(Top);
                double ty = Canvas.GetTop(Top);
                double bx = Canvas.GetLeft(Bottom);
                double by = Canvas.GetTop(Bottom);
                double lx = Canvas.GetLeft(Left);
                double ly = Canvas.GetTop(Left);
                double rx = Canvas.GetLeft(Right);
                double ry = Canvas.GetTop(Right);
                double new_width = Crop_Area.Width;
                double new_height = Crop_Area.Height;

                // Update the rectangle.
                switch (MouseHitType)
                {
                    case HitType.Body:
                        new_x += offset_x;
                        new_y += offset_y;
                        tx += offset_x;
                        ty += offset_y;
                        bx += offset_x;
                        by += offset_y;
                        lx += offset_x;
                        ly += offset_y;
                        rx += offset_x;
                        ry += offset_y;

                        break;
                    case HitType.L:
                        new_x += offset_x;
                        new_y += offset_x;
                        ty += offset_x;
                        lx += offset_x;
                        rx -= offset_x;
                        by -= offset_x;
                        new_width -= 2 * offset_x;
                        new_height -= 2 * offset_x;
                        break;
                    case HitType.R:
                        new_x -= offset_x;
                        new_y -= offset_x;
                        ty -= offset_x;
                        lx -= offset_x;
                        rx += offset_x;
                        by += offset_x;
                        new_width += 2 * offset_x;
                        new_height += 2 * offset_x;
                        break;
                    case HitType.B:
                        new_x -= offset_y;
                        new_y -= offset_y;
                        by += offset_y;
                        ty -= offset_y;
                        lx -= offset_y;
                        rx += offset_y;
                        new_width += 2 * offset_y;
                        new_height += 2 * offset_y;
                        break;
                    case HitType.T:
                        new_x += offset_y;
                        new_y += offset_y;
                        by -= offset_y;
                        ty += offset_y;
                        lx += offset_y;
                        rx -= offset_y;
                        new_width -= 2 * offset_y;
                        new_height -= 2 * offset_y;
                        break;
                }

                // Don't use negative width or height.
                if ((new_width > 100) && (new_height > 100) && (new_x > start_x) && (new_y > start_y) && (new_x < (start_x + image_width - Crop_Area.Width)) && (new_y < (start_y + image_height - Crop_Area.Height)))
                {
                    // Update the rectangle.
                    Canvas.SetLeft(Crop_Area, new_x);
                    Canvas.SetTop(Crop_Area, new_y);
                    Canvas.SetLeft(Top, tx);
                    Canvas.SetTop(Top, ty);
                    Canvas.SetLeft(Bottom, bx);
                    Canvas.SetTop(Bottom, by);
                    Canvas.SetLeft(Left, lx);
                    Canvas.SetTop(Left, ly);
                    Canvas.SetLeft(Right, rx);
                    Canvas.SetTop(Right, ry);
                    Crop_Area.Width = new_width;
                    Crop_Area.Height = new_height;

                    LastPoint = point;
                }
            }
        }

        // Stop dragging.
        private void canvas1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DragInProgress = false;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            CutImage();
            String s = "true";
            Create newPage = new Create(s);
            this.NavigationService.Navigate(newPage);
        }

        private void CutImage()
        {
            BitmapImage src = bitmapImage;
            double ratiox = bitmapImage.PixelWidth / Image.ActualWidth;

            Image Crop = new Image();

            int x, y;

            x = (int)((Canvas.GetLeft(Crop_Area) - start_x) * ratiox);
            y = (int)((Canvas.GetTop(Crop_Area) - start_y) * ratiox);

            Crop.Source = new CroppedBitmap(src, new Int32Rect(x, (int)y, (int)(Crop_Area.Width * ratiox), (int)(Crop_Area.Height * ratiox)));
            WriteableBitmap wb = new WriteableBitmap(Crop.Source as BitmapSource);
            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            username = username.Substring(username.IndexOf(@"\") + 1, username.Length - username.IndexOf(@"\") - 1);
            using (FileStream stream = new FileStream(@"C:\Users\" + username + @"\Pictures\ProfilePic.png", FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wb));
                encoder.Save(stream);
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Create newPage = new Create();
            this.NavigationService.Navigate(newPage);
        }

        private void Crop_Area_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor desired_cursor = Cursors.Arrow;
            Cursor = desired_cursor;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (first)
                OpenFile();
        }
    }
}
