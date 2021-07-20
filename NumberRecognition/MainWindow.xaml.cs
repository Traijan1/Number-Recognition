using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NumberRecognition {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        Point currentPoint = new Point();
        bool isDrawing = false;

        List<String> o = new List<string>();

        public MainWindow() {
            InitializeComponent();
        }

        private void Paint_MouseDown(object sender, MouseButtonEventArgs e) {
            if(e.ButtonState == MouseButtonState.Pressed) {
                isDrawing = true;
                currentPoint = e.GetPosition(this);
            }
        }

        private void Paint_MouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Line line = new Line();

                line.Stroke = new SolidColorBrush(Colors.White);
                line.StrokeThickness = 3;
                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                line.X2 = e.GetPosition(this).X;
                line.Y2 = e.GetPosition(this).Y;

                currentPoint = e.GetPosition(this);

                Paint.Children.Add(line);
            }
        }

        // Method from here: https://teusje.wordpress.com/2012/05/01/c-save-a-canvas-as-an-image/
        private void CreateSaveBitmap(Canvas canvas, string filename) {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
             (int)canvas.Width, (int)canvas.Height,
             96d, 96d, PixelFormats.Pbgra32);

            // needed otherwise the image output is black
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));

            renderBitmap.Render(canvas);

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream file = File.Create(filename)) {
                encoder.Save(file);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            CreateSaveBitmap(Paint, "img.jpg");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            Result.Text = "";
            CreateSaveBitmap(Paint, "img.jpg");

            var process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = @"python",
                    Arguments = @"..\..\..\app.py",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            process.ErrorDataReceived += Process_OutputDataReceived;
            process.OutputDataReceived += Process_OutputDataReceived;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.Exited += Process_Exited;
        }

        private void Process_Exited(object sender, EventArgs e) {
            Application.Current.Dispatcher.Invoke(() => {
                foreach(var s in o) {
                    if(s.Length == 1) {
                        Result.Text = s;
                        break;
                    }
                }

                o.Clear();
            });
        }

        void Process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            Application.Current.Dispatcher.Invoke(() => {
                o.Add(e.Data);
            });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            Paint.Children.Clear();
        }
    }
}
