using System;
using NLog.Web;
using System.IO;
using System.Linq;

namespace MediaLibrary
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {

            logger.Info("Program started");


            MovieFile movieFile = new MovieFile("movies.scrubbed.csv");
            Console.WriteLine(movieFile);

            Console.WriteLine("1) Add Movie");
            Console.WriteLine("2) Display All Movies");
            string userInput = Console.ReadLine();

            if(userInput == "1"){
                //add movie

                Console.WriteLine("Enter movie title: ");
                string title = Console.ReadLine();
                Console.WriteLine("Enter director: ");
                string director = Console.ReadLine();
                Console.WriteLine("Enter running time (hrs): ");
                Int32 movieHrs = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter running time (mins): ");
                Int32 movieMins = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter running time (secs): ");
                Int32 movieSecs = int.Parse(Console.ReadLine());
                TimeSpan runningTime = new TimeSpan(movieHrs, movieMins, movieSecs);
                Console.WriteLine("Enter genres (seperated by |): ");
                string genres = Console.ReadLine();

                //try parse if this isn't a number then log it in the nlog
                string movieIDString = File.ReadLines("movies.scrubbed.csv").Last().Split(',')[0];
                UInt64 mediaId;
                if (!UInt64.TryParse(movieIDString, out mediaId)) {
                    // log error
                    logger.Error("Invalid Data.");
                } else {
                    mediaId += 1;
                }


                Movie movie = new Movie
                {
                    mediaId = mediaId,
                    title = title,
                    director = director,
                    // timespan (hours, minutes, seconds)
                    runningTime = runningTime,
                    genres = { genres }
                };

                //how do i use MovieFile classes AddMovie method for the movie object i just created???
                //MovieFile.AddMovie(movie);
                
                StreamWriter sw = new StreamWriter("movies.scrubbed.csv", true);
                sw.WriteLine(mediaId + "," + title + "," + genres + "," + director + "," + runningTime);

                Console.WriteLine("\n New Movie Added:");
                Console.WriteLine(movie.Display());

                sw.Close();

            } else if(userInput == "2"){
                StreamReader sr = new StreamReader("movies.scrubbed.csv ");
                while(!sr.EndOfStream)
                {
                        //splits csv into lines
                    string line = sr.ReadLine();
                    var values = line.Split(",");

                    if (line.IndexOf('"') >= 0)
                    {
                        string movieID = values[0];
                        //values[0] = movieID;
                       // string movieTitle = "";
                        string movieGenre = values[values.Length - 3];
                        String movieDirector = values[values.Length - 2];
                        String movieRunTime = values[values.Length - 1];
                        //if line contains ", take everything between " "
                        var idx1 = line.IndexOf('"');
                        var str = line.Substring(idx1+1);
                        var idx2 = str.IndexOf('"');
                        string movieTitle = line.Substring(idx1+1, idx2);

                        

                        Console.WriteLine("Movie ID: {0}, Movie Title: {1}, Movie Genre: {2}, Movie Director: {3}, Movie Runtime: {4}", movieID, movieTitle, movieGenre, movieDirector, movieRunTime);
                    } else {
                        Console.WriteLine("Movie id: {0}, Movie Title: {1}, Movie Genre: {2}, Movie Director: {3}, Movie Runtime: {4}", values[0], values[1], values[2], values[3], values[4]);
                        
                    }
                }
                sr.Close();

            }




            string scrubbedFile = FileScrubber.ScrubMovies("movies.csv");
            logger.Info(scrubbedFile);
            MovieFile movieFile1 = new MovieFile(scrubbedFile);


            logger.Info("Program ended");
        }
    }
}