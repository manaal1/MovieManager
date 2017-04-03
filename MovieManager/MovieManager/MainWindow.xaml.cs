using System;
using System.Collections.Generic;
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
using System.Data.OleDb;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MovieManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //List of Movies.
        ObservableCollection<Movie> listOfMovie = new ObservableCollection<Movie>();
        public MainWindow()
        {
            InitializeComponent();

            gridBody.Visibility = System.Windows.Visibility.Hidden;
            loadTitles();                        
        }

        //Load Combobox Movie Title.
        private void loadTitles()
        {
            cmbxTitle.ItemsSource = null;
            listOfMovie.Clear();
            Movie.ReadRecords(listOfMovie);

            if (listOfMovie.Count > 0)
            {
                cmbxTitle.ItemsSource = listOfMovie;
                cmbxTitle.DisplayMemberPath = "Title";
                cmbxTitle.SelectedValuePath = "ID";
            }
        }

        //Reset all fields for new entry.
        private void clearFields()
        {
            lblID.Content = "";
            txtDesc.Text = "";
            txtGenre.Text = "";
            txtPicPath.Text = "";
            txtRating.Text = "";
            txtRDate.Text = "";
            txtTitle.Text = "";
            txtBoxOfficeColl.Text = "";
            imgMoviePic.Source = null;
        }

        //Load a selected movie.
        private void cmbxTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbxTitle.SelectedIndex >= 0)
                {
                    clearFields();

                    gridBody.Visibility = System.Windows.Visibility.Visible;

                    Movie selectedMovie = (Movie)cmbxTitle.Items[cmbxTitle.SelectedIndex];

                    lblID.Content = selectedMovie.ID.ToString();
                    txtTitle.Text = selectedMovie.Title;
                    txtGenre.Text = selectedMovie.Genre;
                    txtRDate.Text = selectedMovie.ReleaseDate.ToShortDateString();
                    chbxSold.IsChecked = (bool)selectedMovie.Sold;
                    txtDesc.Text = selectedMovie.Description;
                    txtPicPath.Text = selectedMovie.PicturePath;
                    txtRating.Text = selectedMovie.Rating.ToString();
                    txtBoxOfficeColl.Text = string.Format("{0:0.00}", selectedMovie.BoxofficeColl);

                    var uri = new Uri(selectedMovie.PicturePath);
                    var bitmap = new BitmapImage(uri);
                    imgMoviePic.Source = bitmap;
                }
                else
                {
                    gridBody.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + "Load failed.", "Error!");
            }
            
        }

        //Make all GUI fields clear for add new entry.
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbxTitle.SelectedIndex = -1;
                gridBody.Visibility = System.Windows.Visibility.Visible;
                clearFields();               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        //Delete a selected movie and update the list.
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbxTitle.SelectedIndex >= 0)
                {
                    Movie selectedMovie = (Movie)cmbxTitle.Items[cmbxTitle.SelectedIndex];

                    Movie.deleteMovie(selectedMovie);

                    loadTitles();
                }
                else
                {
                    MessageBox.Show("Please select a movie for delete.", "Information!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + "Data has not Deleted successfully.", "Error!");
            }            
        }

        //Refresh the window.
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            gridBody.Visibility = System.Windows.Visibility.Hidden;
            cmbxTitle.SelectedIndex = -1;
            clearFields();
        }

        //Get all user input and after validate store into Movie object
        //and save it to database table.
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Movie newMovie = null;
                bool isNew = false;

                if (string.IsNullOrEmpty(lblID.Content.ToString()))
                {
                    //Add New Movie...
                    newMovie = new Movie();
                    isNew = true;
                }
                else
                {
                    //Update the selected Movie...
                    newMovie = (Movie)cmbxTitle.Items[cmbxTitle.SelectedIndex];
                    isNew = false;
                }

                newMovie.Title = txtTitle.Text;
                newMovie.Genre = txtGenre.Text;
                DateTime rDate = new DateTime();

                if (DateTime.TryParse(txtRDate.Text, out rDate))
                {
                    newMovie.ReleaseDate = rDate;
                }
                else
                {
                    MessageBox.Show("Release Date string is not in correct format.", "Invaild Input");
                    return;
                }
                newMovie.Sold = Convert.ToBoolean(chbxSold.IsChecked);
                newMovie.Description = txtDesc.Text;
                newMovie.PicturePath = txtPicPath.Text;

                int rating = 0;

                if (int.TryParse(txtRating.Text, out rating))
                {
                    newMovie.Rating = rating;
                }
                else
                {
                    MessageBox.Show("Rating should be numeric value.", "Invaild Input");
                    return;
                }

                decimal boxofficeColl = 0;

                if (decimal.TryParse(txtBoxOfficeColl.Text, out boxofficeColl))
                {
                    newMovie.BoxofficeColl = boxofficeColl;
                }
                else
                {
                    MessageBox.Show("Box Office Collection should be decimal value.", "Invaild Input");
                    return;
                }

                Movie.addEditMovie(newMovie, isNew);

                loadTitles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + "Data has not saved successfully.", "Error!");
            }
        }

        //Selete a picture by openig file location
        //and convert it to BitmapImage and display.
        private void btnSelectPicture_Click(object sender, RoutedEventArgs e)
        {
            // Configure dialog box
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp"; // Filter files by extension

            // Show open file dialog box
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Open document
                txtPicPath.Text = dlg.FileName;

                var uri = new Uri(txtPicPath.Text);
                var bitmap = new BitmapImage(uri);
                imgMoviePic.Source = bitmap;
            }
        }
    }
}
