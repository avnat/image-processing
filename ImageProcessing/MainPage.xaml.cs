using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using Microsoft.Phone;
using System.Windows.Controls;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Media;
using System.Runtime.InteropServices;

namespace ImageProcessing
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
         
        }
        PhotoChooserTask photoChooser = new PhotoChooserTask();
        BitmapImage image = new BitmapImage();
        PhotoResult photoResult = new PhotoResult();
        WriteableBitmap wb = new WriteableBitmap(512,512);

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            photoChooser.ShowCamera = true;
            photoChooser.Show();
            photoChooser.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);   
        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            photoResult = e;
            if (e.TaskResult == TaskResult.OK)
            {   
                image.SetSource(e.ChosenPhoto);
                imgContainer.Source = image;
            }
            else if (e.TaskResult == TaskResult.None)
            {
                return;
            }
         
        }


        private void btnEffect_Click(object sender, RoutedEventArgs e)
        {
            int color = unchecked((int)0xFF0000FF);
            WriteableBitmap wbmp = new WriteableBitmap(image);

            for (int i = 0; i < wbmp.PixelHeight; i++)
            {
                int index = (i * wbmp.PixelWidth);
                for (int counter = -19; counter < 20; counter++)
                {
                    if (index + counter <= wbmp.PixelHeight * wbmp.PixelWidth && index + counter > 0)
                    {
                        wbmp.Pixels.SetValue(color, index + counter);
                    }
                }
            }

            for (int i = 0; i <wbmp.PixelWidth; i++)
            {
                for (int counter=0;counter<15;counter++)
                    {
                        wbmp.Pixels.SetValue(color, i + (counter * wbmp.PixelWidth));
                    }
            }

            for (int i = (wbmp.PixelWidth * (wbmp.PixelHeight - 1)); i < (wbmp.PixelWidth * wbmp.PixelHeight); i++)
            {
                for (int counter = -15; counter < 1; counter++)
                {
                    wbmp.Pixels.SetValue(color, i + (counter * wbmp.PixelWidth));
                }

            }

            imgContainer.Source = wbmp;
            wb = wbmp;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveImage(photoResult.ChosenPhoto, 0, 100, wb);
        }

        public void SaveImage(Stream imageStream, int orientation, int quality, WriteableBitmap wb)
        {
            using (var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isolatedStorage.FileExists(imgContainer.Name))
                    isolatedStorage.DeleteFile(imgContainer.Name);

                IsolatedStorageFileStream fileStream = isolatedStorage.CreateFile(imgContainer.Name);
                image.SetSource(photoResult.ChosenPhoto);

                wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, orientation, quality);
                fileStream.Close();
            }

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(imgContainer.Name , FileMode.Open , FileAccess.Read ))
                {
                        MediaLibrary mediaLibrary = new MediaLibrary();
                        Picture pic = mediaLibrary.SavePicture(imgContainer.Name, fileStream);
                        fileStream.Close();
                }   
            }


            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Show();
        }
    }
}
