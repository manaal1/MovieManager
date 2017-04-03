using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MovieManager
{
    /// <summary>
    /// Create a Movie class for store database data into runtime
    /// and manage load, add, edit and delete through this class.
    /// This class has all fields same as database 'MOVIE' table.
    /// </summary>
    class Movie
    {
        //Declare a private string field for store the database connection.
        private static OleDbConnection conn = new OleDbConnection(
                    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Movie.accdb;Persist Security Info=True");
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string genre;

        public string Genre
        {
            get { return genre; }
            set { genre = value; }
        }

        private DateTime releasedate;

        public DateTime ReleaseDate
        {
            get { return releasedate; }
            set { releasedate = value; }
        }

        private bool sold;

        public bool Sold
        {
            get { return sold; }
            set { sold = value; }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private string picturePath;

        public string PicturePath
        {
            get { return picturePath; }
            set { picturePath = value; }
        }

        private int rating;

        public int Rating
        {
            get { return rating; }
            set { rating = value; }
        }

        private decimal boxofficeColl;

        public decimal BoxofficeColl
        {
            get { return boxofficeColl; }
            set { boxofficeColl = value; }
        }
        

        /// <summary>
        /// Read all data from database and load it into given parameter list object.
        /// </summary>
        /// <param name="listOfMovie">List of Movies</param>
        public static void ReadRecords(ObservableCollection<Movie> listOfMovie)
        {
            OleDbDataReader reader = null;
            try
            {
                //Open DB connection.
                conn.Open();

                //Query for select all data of 'MOVIE' table from database.
                OleDbCommand cmd = new OleDbCommand("Select * FROM MOVIE", conn);

                //Execute query through OleDBDataReader.
                reader = cmd.ExecuteReader();

                //Till reader has data.
                while (reader.Read())
                {
                    //Create a Movie object from each row of DB table 'MOVIE'.
                    Movie m = new Movie();

                    m.ID = Convert.ToInt32(reader.GetValue(0).ToString());
                    m.Title = reader.GetString(1);
                    m.Genre = reader.GetString(2);
                    m.ReleaseDate = Convert.ToDateTime(reader.GetValue(3).ToString());
                    m.Sold = Convert.ToBoolean(reader.GetValue(4).ToString());
                    m.Description = reader.GetString(5);
                    m.PicturePath = reader.GetValue(6).ToString();
                    m.Rating = Convert.ToInt32(reader.GetValue(7).ToString());
                    m.BoxofficeColl = Convert.ToDecimal(reader.GetValue(8).ToString());

                    //Add movie object to list.
                    listOfMovie.Add(m);
                }
            }
            catch (Exception e)
            {
                //If any connection related or other exception happen, show this message.
                MessageBox.Show(e.Message + Environment.NewLine + "Connection has not open successfully.", "Error!");
            }
            finally
            {
                //when all done close the reader and connection.
                if (reader != null)
                {
                    reader.Close();
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Take all data from given parameter Movie object and make a
        /// query string for Insert if 'isNew' else Update.
        /// Run query and change the database data.
        /// </summary>
        /// <param name="newMovie">New Movie object if 'isNew' else selected Movie object.</param>
        /// <param name="isNew">New Movie for add or Previous Movie for update</param>
        public static void addEditMovie(Movie newMovie, bool isNew)
        {
            try
            {
                conn.Open();

                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = CommandType.Text;

                if (isNew)
                {
                    //Insert query for add new Movie.
                    cmd.CommandText = "insert into MOVIE ([TITLE], [GENRE], [RELEASE_DATE], [SOLD], [DESCRIPTION], [PICTURE_PATH], [RATING], [BOXOFFICE_COLL]) " +
                        "values (@TITLE, @GENRE, @RELEASE_DATE, @SOLD, @DESCRIPTION, @PICTURE_PATH, @RATING, @BOXOFFICE_COLL)";
                    cmd.Parameters.AddWithValue("@TITLE", newMovie.Title);
                    cmd.Parameters.AddWithValue("@GENRE", newMovie.Genre);
                    cmd.Parameters.AddWithValue("@RELEASE_DATE", newMovie.ReleaseDate);
                    cmd.Parameters.AddWithValue("@SOLD", newMovie.Sold);
                    cmd.Parameters.AddWithValue("@DESCRIPTION", newMovie.Description);
                    cmd.Parameters.AddWithValue("@PICTURE_PATH", newMovie.PicturePath);
                    cmd.Parameters.AddWithValue("@RATING", newMovie.Rating);
                    cmd.Parameters.AddWithValue("@BOXOFFICE_COLL", newMovie.BoxofficeColl);
                }
                else
                {
                    //Update query for edit selected Movie.
                    cmd.CommandText = "update MOVIE set [TITLE] = @TITLE, [GENRE] = @GENRE, [RELEASE_DATE] = @RELEASE_DATE, " +
                        "[SOLD] = @SOLD, [DESCRIPTION] = @DESCRIPTION, [PICTURE_PATH] = @PICTURE_PATH, [RATING] = @RATING " +
                        ", [BOXOFFICE_COLL] = @BOXOFFICE_COLL where [ID] = " + newMovie.ID;
                    cmd.Parameters.AddWithValue("@TITLE", newMovie.Title);
                    cmd.Parameters.AddWithValue("@GENRE", newMovie.Genre);
                    cmd.Parameters.AddWithValue("@RELEASE_DATE", newMovie.ReleaseDate);
                    cmd.Parameters.AddWithValue("@SOLD", newMovie.Sold);
                    cmd.Parameters.AddWithValue("@DESCRIPTION", newMovie.Description);
                    cmd.Parameters.AddWithValue("@PICTURE_PATH", newMovie.PicturePath);
                    cmd.Parameters.AddWithValue("@RATING", newMovie.Rating);
                    cmd.Parameters.AddWithValue("@BOXOFFICE_COLL", newMovie.BoxofficeColl);
                }

                cmd.Connection = conn;
                cmd.ExecuteNonQuery();

                if (isNew)
                {
                    MessageBox.Show("A New Movie has been successfully added.", "Add New!");
                }
                else
                {
                    MessageBox.Show("A Movie has been successfully updated.", "Update!");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!");
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Delete selected Movie which has passed by parameter.
        /// </summary>
        /// <param name="selectedMovie">The selected Movie for delete</param>
        public static void deleteMovie(Movie selectedMovie)
        {
            try
            {
                conn.Open();

                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = CommandType.Text;

                //Delete query for delete the selected Movie.
                cmd.CommandText = "delete from MOVIE where [ID] = " + selectedMovie.ID;
                
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();

                MessageBox.Show("Selected Movie has been successfully deleted.", "Delete!");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!");
            }
            finally
            {
                conn.Close();
            }
        }

    }
}
