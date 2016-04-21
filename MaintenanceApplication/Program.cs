using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Text;
namespace MaintenanceApplication
{
   public static class Program
    {

       public static string ConnectionStringWithFilePath =@"Provider=Microsoft Office 12.0 Access Database Engine OLE DB Provider;" + @"Data source="+ ConfigurationManager.AppSettings["FilePath"]+";Jet OLEDB:Database Password=Sanmar!123;";
       public static string DefaultRowCount = ConfigurationManager.AppSettings["DefaultRowsCount"];
       public static string DefaultRowCountMessage = string.Format("Default it showing last {0} added records.If you want to show all the records clic" +
    "k \"Show all\" button ", Program.DefaultRowCount);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static DataTable  GetDataTableDataFromDb(string query)
        {
            string connectionString = ConnectionStringWithFilePath;

            string queryString = query;
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand();
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = queryString;

                    connection.Open();

                    var dataReader = command.ExecuteReader(); 
                    var dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    return dataTable;
                  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to data source");
            }
            return null;
        }

        public static DataTable GetEquipmenttype()
        {
            var query = "SELECT * from EquipmentMaster ";
            return Program.GetDataTableDataFromDb(query);
        }
        public static  DateTime GetDateWithoutMilliseconds(DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
        }

       //Equipment List Load
        public static void CallEquipmentGridRefresh(ref DataGridView dataGridView,int isDefaultRowCount)
        {
            var queryForMaintenance = string.Empty;
            if(isDefaultRowCount==0)
            {
                 queryForMaintenance = string.Format("SELECT Top {0} *  from EquipmentMaster ORDER BY [EquipmentID] DESC", DefaultRowCount);
            }
            else
            {
                queryForMaintenance = "SELECT  *  from EquipmentMaster ORDER BY [EquipmentID] DESC";
            }

            
            var dataTableForMaintenance = Program.GetDataTableDataFromDb(queryForMaintenance);
            dataGridView.DataSource = dataTableForMaintenance;
        }
       
       
       //Maintenance List Load
        public static void CallMaintenanceGridRefresh(ref DataGridView dataGridView,string type ,int isDefaultRowCount)
        {
            var queryForEquipment = string.Empty;
            if(isDefaultRowCount==0)
            {
                 queryForEquipment = string.Format("SELECT  TOP {0} * from Maintenance Where [PlantType]=@plantType ORDER BY [MaintenanceID] DESC", DefaultRowCount);
            }
            else
            {
                queryForEquipment = "SELECT  * from Maintenance Where [PlantType]=@plantType ORDER BY [MaintenanceID] DESC";
            }
            
            string connectionString = Program.ConnectionStringWithFilePath;

            string queryString = queryForEquipment;
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand();
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = queryString;

                    command.Parameters.AddWithValue("@plantType", type);
                    //command.Parameters.AddWithValue("@plantType", "ISBL");
                    connection.Open();
                    var reader = command.ExecuteReader();
                    var dataTable = new DataTable();
                    dataTable.Load(reader);

                    dataGridView.DataSource = dataTable;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to data source");
            }
        }

        public static DataTable MaintenanceSearch(string equipmentTag, string equipmentName, string attendedBy,double dateFrom,double dateTo,string type)
        {

            OleDbCommand command = new OleDbCommand();
            string queryString = string.Empty;
            if (equipmentName != "" && equipmentTag != "")
            {
                equipmentName = "%" + equipmentName + "%";
                equipmentTag = "%" + equipmentTag + "%";
                queryString = string.Format("Select * from Maintenance  where  LCASE([EquipmentName]) LIKE  @equipmentName and LCASE([EquipmentTag]) LIKE  @equipmentTag and PlantType=@plantType ORDER BY [MaintenanceID] DESC ");
                command.Parameters.AddWithValue("@equipmentName", equipmentName);
                command.Parameters.AddWithValue("@equipmentTag", equipmentTag);
            }
            else if (equipmentTag != "")
            {
                equipmentTag = "%" + equipmentTag + "%";
                queryString = string.Format("Select * from Maintenance  where LCASE([EquipmentTag]) LIKE  @equipmentTag and PlantType=@plantType ORDER BY [MaintenanceID] DESC ");
                command.Parameters.AddWithValue("@equipmentTag", equipmentTag);


            }
            else if (equipmentName != "")
            {

                equipmentName = "%" + equipmentName + "%";
                queryString = string.Format("Select * from Maintenance  where  LCASE([EquipmentName]) LIKE  @equipmentName and PlantType=@plantType ORDER BY [MaintenanceID] DESC ");
                command.Parameters.AddWithValue("@equipmentName", equipmentName);
            }
            else if (attendedBy != "")
            {

                attendedBy = "%" + attendedBy + "%";
                queryString = string.Format("Select * from Maintenance  where  LCASE([AttendedBy]) LIKE  @attendedBy   and PlantType=@plantType ORDER BY [MaintenanceID] DESC");
                command.Parameters.AddWithValue("@attendedBy", attendedBy);
            }


            else
            {

                queryString = string.Format("Select * from Maintenance Where  [AttendedDate] BETWEEN @dateFrom AND @dateTo AND [PlantType]=@plantType ORDER BY [MaintenanceID] DESC ");
                command.Parameters.AddWithValue("@dateFrom", dateFrom);
                command.Parameters.AddWithValue("@dateTo", dateTo);
            }




            // var attendedDate = string.Format("{0:dd-MMM-yyyy}",dateTimePicker1.Value);

            string connectionString = Program.ConnectionStringWithFilePath;
            //LCASE([EquipmentTag]) LIKE  @equipmentTag or LCASE([EquipmentName]) LIKE  @equipmentName or LCASE([AttendedBy]) LIKE  @attendedBy or [AttendedDate] = @attendedDate 

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {

                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = queryString;
                    command.Parameters.AddWithValue("@plantType",type);                   
                    connection.Open();
                    var dataReader = command.ExecuteReader();
                    var dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to data source");
            }
            return null;
        }

       public static void ExportToCsv(string path,DataGridView dataGridView)
        {
            //var path = @"C:\Users\DELL5520\Desktop\mycsvfile.csv";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("EquipmentID,Equipment Tag,Equipment Name,Attended By,Attended Date,Action,Material,Plant Type");


            using (var writer = new StreamWriter(path))
            {
                for (int i = 0; i < dataGridView.RowCount; i++)
                {
                    DataGridViewRow GridRow = dataGridView.Rows[i];
                    //   var contents = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", GridRow.Cells[0].Value.ToString()+",", GridRow.Cells[1], GridRow.Cells[2],GridRow.Cells[3],GridRow.Cells[4],GridRow.Cells[5],GridRow.Cells[6],GridRow.Cells[7],GridRow.Cells[7]);
                    //sb.AppendLine(contents);
                    sb.AppendLine("");
                    if (GridRow.Cells[0].Value != null)
                    {
                        sb.Append(RemoveComma(GridRow.Cells[0].Value.ToString()) + ",");
                    }
                    if (GridRow.Cells[1].Value != null)
                    {
                        sb.Append(RemoveComma(GridRow.Cells[1].Value.ToString()) + ",");
                    }
                    if (GridRow.Cells[2].Value != null)
                    {
                        sb.Append(RemoveComma(GridRow.Cells[2].Value.ToString()) + ",");
                    }
                    if (GridRow.Cells[3].Value != null)
                    {
                        sb.Append(RemoveComma(GridRow.Cells[3].Value.ToString()) + ",");
                    }
                    if (GridRow.Cells[4].Value != null)
                    {
                        sb.Append(RemoveComma(GridRow.Cells[4].Value.ToString()) + ",");
                    }
                    if (GridRow.Cells[5].Value != null)
                    {
                        sb.Append(RemoveComma(GridRow.Cells[5].Value.ToString()) + ",");
                    }
                    if (GridRow.Cells[6].Value != null)
                    {
                        sb.Append(RemoveComma(GridRow.Cells[6].Value.ToString() )+ ",");
                    }
                    if (GridRow.Cells[7].Value != null)
                    {
                        sb.Append(RemoveComma(GridRow.Cells[7].Value.ToString()));
                    }


                }
                writer.Write(sb.ToString());
            }
        }

       private static string RemoveComma(string text)
       {
           return text.Replace(",", " ");
       }


       public static List<string> quotes = new List<string>() { "It’s hard to beat a person who never gives up.'– Babe Ruth24",
"If passion drives you, let reason hold the reins.'– Benjamin Franklin",
"I’m a greater believer in luck, and I find the harder I work the more I have of it.'– Thomas Jefferson",
"I have learned that real angels don’t have gossamer white robes and Cherubic skin, they have calloused hands and smell of the days’ sweat.'– Richard Evans",
"Greatness is sifted through the grind, therefore don’t despise the hard work now for surely it will be worth it in the end.'– Sanjo Jendayi",
"A dream doesn’t become reality through magic; it takes sweat, determination and hard work.'– Colin Powell28",
"My grandfather once told me that there were two kinds of people: those who do the work and those who take the credit. He told me to try to be in the first group; there was much less competition.'– Indira Gandhi",
"Happiness is the real sense of fulfillment that comes from hard work.'– Joseph Barbara",
"Hard work keeps the wrinkles out of the mind and spirit.'– Helena Rubinstein",
"No great achiever – even those who made it seem easy – ever succeeded without hard work.'– Jonathan Sacks",
"Hard work helps. It has never killed anyone.'– Unknown",
"The highest reward for man’s toil is not what he gets for it, but what he becomes by it.'– John Ruskin",
"You learn the value of hard work by working hard.'– Unknown",
"Nobody’s a natural. You work hard to get good and then work to get better. It’s hard to stay on top.'– Paul Coffey",
"Don’t wish it were easier. Wish you were better.'– Jim Rohn",
"All growth depends upon activity. There is no development physically or intellectually without effort, and effort means work.'– Calvin Coolidge",
"Talent means nothing, while experience, acquired in humility and with hard work, means everything.'– Patrick Suskind",
"The difference between ordinary and extraordinary is that little extra.'– Jimmy Johnson",
"If I told you I’ve worked hard to get where I’m at, I’d be lying, because I have no idea where I am right now.'– Jarod Kintz",
"Too many irons, not enough fire.'– S. Kelley Harrell",
"Patience can be bitter but her fruit is always sweet.'– Habeeb Akande",
"If you don’t burn out at the end of each day, you’re a bum.'– George Lois",
"If you try and lose then it isn’t your fault. But if you don’t try and we lose, then it’s all your fault.'– Orson Scott Card",
"Embrace the pain to inherit the gain.'– Habeeb Akande",
"Talent is what God gives us, Skill is what we give back to Him.'– Eliel Pierre",
"The only thing that overcomes hard luck is hard work.'– Harry Golden",
"I hope the millions of people I’ve touched have the optimism and desire to share their goals and hard work and persevere with a positive attitude.'– Michael Jordan",
"Goodness and hard work are rewarded with respect.'– Luther Campbell",
"Everything yields to diligence.'– Thomas Jefferson",
"When I meet succesful people I ask about 100 questions to find out who they attribute their success to. It is usually the same: persistence, hard work and hiring good people.'– Kiana Tom",
"Let me tell you the secret that has led me to my goals: my strength lies solely in my tenacity.'– Louis Pasteur",
"I don’t have a blue-collar job. It’s more of a green collar, because of all the yellow sweat stains mixing in.'– Jarod Kintz",
"No one ever drowned in sweat.'– USMC Officer",
"There are no shortcuts to any place worth going.'– Beverly Sills",
"If your dream is a big dream, and if you want your life to work on the high level that you say you do, there’s no way around doing the work it takes to get you there.'– Joyce Chapman",
"I do not care about happiness simply because I believe that joy is something worth fighting for.'– Criss Jami",
"Hard work without talent is a shame, but talent without hard work is a tragedy.'– Robert Hall",
"A clay pot sitting in the sun will always be a clay pot. It has to go through the white heat of the furnace to become porcelain.'– Mildred Struven",
"You were hired because you met expectations, you will be promoted if you can exceed them.'– Saji Ijiyemi",
"Inspiration is the windfall from hard work and focus. Muses are too unreliable to keep on the payroll.'– Helen Hanson",
"There are no shortcuts to any place worth going.'– Beverly Sills",
"Do the things you like to be happier, stronger & more successful. Only so is hard work replaced by dedication.'– Rossana Condoleo",
"Only cooked time tastes well.'– Dr. Zeeshan Ahmed",
"Nothing in life comes easy.'– Unknown",
"Change is hard work.'– Billy Crystal",
"Once you have commitment, you need the discipline and hard work to get you there.'– Haile Gebrselassie",
"If you work hard and study hard. And you fuck up. That’s okay. If you fuck up and you fuck up, then you’re a fuckup.'– Justin Halpern",
"Sometimes it takes a lowly, title-less man to humble the world. Kings, rulers, CEOs, judges, doctors, pastors, they are already expected to be greater and wiser.'– Criss Jami",
"Work hard and be patient. The rest will follow.'– Unknown",
"Start by doing what’s necessary, then what’s possible; and suddenly you are doing the impossible.'– Saint Francis",
"What I do for my work is exactly what I would do if nobody paid me.'Gretchen Rubin",
"Whatever the mind of man can conceive and believe, it can achieve. –Napoleon Hill",
"Strive not to be a success, but rather to be of value. –Albert Einstein",
"Two roads diverged in a wood, and I—I took the one less traveled by, And that has made all the difference.  –Robert Frost",
"I attribute my success to this: I never gave or took any excuse. –Florence Nightingale",
"You miss 100% of the shots you don’t take. –Wayne Gretzky",
"I’ve missed more than 9000 shots in my career. I’ve lost almost 300 games. 26 times I’ve been trusted to take the game winning shot and missed. I’ve failed over and over and over again in my life. And that is why I succeed. –Michael Jordan",
"The most difficult thing is the decision to act, the rest is merely tenacity. –Amelia Earhart",
"Every strike brings me closer to the next home run. –Babe Ruth",
"Definiteness of purpose is the starting point of all achievement. –W. Clement Stone",
"Life isn’t about getting and having, it’s about giving and being. –Kevin Kruse",
"Life is what happens to you while you’re busy making other plans. –John Lennon",
" We become what we think about. –Earl Nightingale",
"Twenty years from now you will be more disappointed by the things that you didn’t do than by the ones you did do, so throw off the bowlines, sail away from safe harbor, catch the trade winds in your sails.  Explore, Dream, Discover. –Mark Twain",
"Life is 10% what happens to me and 90% of how I react to it. –Charles Swindoll",
"The most common way people give up their power is by thinking they don’t have any. –Alice Walker",
"The mind is everything. What you think you become.  –Buddha",
"The best time to plant a tree was 20 years ago. The second best time is now. –Chinese Proverb",
"An unexamined life is not worth living. –Socrates",
"Eighty percent of success is showing up. –Woody Allen",
"Your time is limited, so don’t waste it living someone else’s life. –Steve Jobs",
"Winning isn’t everything, but wanting to win is. –Vince Lombardi",
"I am not a product of my circumstances. I am a product of my decisions. –Stephen Covey",
"Every child is an artist.  The problem is how to remain an artist once he grows up. –Pablo Picasso",
"You can never cross the ocean until you have the courage to lose sight of the shore. –Christopher Columbus",
"I’ve learned that people will forget what you said, people will forget what you did, but people will never forget how you made them feel. –Maya Angelou",
"Either you run the day, or the day runs you. –Jim Rohn",
"Whether you think you can or you think you can’t, you’re right. –Henry Ford",
" The two most important days in your life are the day you are born and the day you find out why. –Mark Twain",
"Whatever you can do, or dream you can, begin it.  Boldness has genius, power and magic in it. –Johann Wolfgang von Goethe",
"The best revenge is massive success. –Frank Sinatra",
"People often say that motivation doesn’t last. Well, neither does bathing.  That’s why we recommend it daily. –Zig Ziglar",
"Life shrinks or expands in proportion to one’s courage. –Anais Nin",
"Wisdom is the reward you get for a lifetime of listening when you'd have preferred to talk.' --Doug Larson",
 };
       public static List<string> quoteYearList = new List<string>() {"'It’s hard to beat a person who never gives up.'– Babe Ruth24",
"'If passion drives you, let reason hold the reins.'– Benjamin Franklin",
"'I’m a greater believer in luck, and I find the harder I work the more I have of it.'– Thomas Jefferson",
"'I have learned that real angels don’t have gossamer white robes and Cherubic skin, they have calloused hands and smell of the days’ sweat.'– Richard Evans",
"'Greatness is sifted through the grind, therefore don’t despise the hard work now for surely it will be worth it in the end.'– Sanjo Jendayi",
"'A dream doesn’t become reality through magic; it takes sweat, determination and hard work.'– Colin Powell28",
"'My grandfather once told me that there were two kinds of people: those who do the work and those who take the credit. He told me to try to be in the first group; there was much less competition.'– Indira Gandhi",
"'Happiness is the real sense of fulfillment that comes from hard work.'– Joseph Barbara",
"'Hard work keeps the wrinkles out of the mind and spirit.'– Helena Rubinstein",
"'No great achiever – even those who made it seem easy – ever succeeded without hard work.'– Jonathan Sacks",
"'Hard work helps. It has never killed anyone.'– Unknown",
"'The highest reward for man’s toil is not what he gets for it, but what he becomes by it.'– John Ruskin",
"'You learn the value of hard work by working hard.'– Unknown",
"'Nobody’s a natural. You work hard to get good and then work to get better. It’s hard to stay on top.'– Paul Coffey",
"'Don’t wish it were easier. Wish you were better.'– Jim Rohn",
"'All growth depends upon activity. There is no development physically or intellectually without effort, and effort means work.'– Calvin Coolidge",
"'Talent means nothing, while experience, acquired in humility and with hard work, means everything.'– Patrick Suskind",
"'The difference between ordinary and extraordinary is that little extra.'– Jimmy Johnson",
"'If I told you I’ve worked hard to get where I’m at, I’d be lying, because I have no idea where I am right now.'– Jarod Kintz",
"'Too many irons, not enough fire.'– S. Kelley Harrell",
"'Patience can be bitter but her fruit is always sweet.'– Habeeb Akande",
"'If you don’t burn out at the end of each day, you’re a bum.'– George Lois",
"'If you try and lose then it isn’t your fault. But if you don’t try and we lose, then it’s all your fault.'– Orson Scott Card",
"'Embrace the pain to inherit the gain.'– Habeeb Akande",
"'Talent is what God gives us, Skill is what we give back to Him.'– Eliel Pierre",
"'The only thing that overcomes hard luck is hard work.'– Harry Golden",
"'I hope the millions of people I’ve touched have the optimism and desire to share their goals and hard work and persevere with a positive attitude.'– Michael Jordan",
"'Goodness and hard work are rewarded with respect.'– Luther Campbell",
"'Everything yields to diligence.'– Thomas Jefferson",
"'When I meet succesful people I ask about 100 questions to find out who they attribute their success to. It is usually the same: persistence, hard work and hiring good people.'– Kiana Tom",
"'Let me tell you the secret that has led me to my goals: my strength lies solely in my tenacity.'– Louis Pasteur",
"'I don’t have a blue-collar job. It’s more of a green collar, because of all the yellow sweat stains mixing in.'– Jarod Kintz",
"'No one ever drowned in sweat.'– USMC Officer",
"'There are no shortcuts to any place worth going.'– Beverly Sills",
"'If your dream is a big dream, and if you want your life to work on the high level that you say you do, there’s no way around doing the work it takes to get you there.'– Joyce Chapman",
"'I do not care about happiness simply because I believe that joy is something worth fighting for.'– Criss Jami",
"'Hard work without talent is a shame, but talent without hard work is a tragedy.'– Robert Hall",
"'A clay pot sitting in the sun will always be a clay pot. It has to go through the white heat of the furnace to become porcelain.'– Mildred Struven",
"'You were hired because you met expectations, you will be promoted if you can exceed them.'– Saji Ijiyemi",
"'Inspiration is the windfall from hard work and focus. Muses are too unreliable to keep on the payroll.'– Helen Hanson",
"'There are no shortcuts to any place worth going.'– Beverly Sills",
"'Do the things you like to be happier, stronger & more successful. Only so is hard work replaced by dedication.'– Rossana Condoleo",
"'Only cooked time tastes well.'– Dr. Zeeshan Ahmed",
"'Nothing in life comes easy.'– Unknown",
"'Change is hard work.'– Billy Crystal",
"'Once you have commitment, you need the discipline and hard work to get you there.'– Haile Gebrselassie",
"'If you work hard and study hard. And you fuck up. That’s okay. If you fuck up and you fuck up, then you’re a fuckup.'– Justin Halpern",
"'Sometimes it takes a lowly, title-less man to humble the world. Kings, rulers, CEOs, judges, doctors, pastors, they are already expected to be greater and wiser.'– Criss Jami",
"'Work hard and be patient. The rest will follow.'– Unknown",
"'Start by doing what’s necessary, then what’s possible; and suddenly you are doing the impossible.'– Saint Francis",
"'What I do for my work is exactly what I would do if nobody paid me.'Gretchen Rubin",
"Whatever the mind of man can conceive and believe, it can achieve. –Napoleon Hill",
"Strive not to be a success, but rather to be of value. –Albert Einstein",
"Two roads diverged in a wood, and I—I took the one less traveled by, And that has made all the difference.  –Robert Frost",
"I attribute my success to this: I never gave or took any excuse. –Florence Nightingale",
"You miss 100% of the shots you don’t take. –Wayne Gretzky",
"I’ve missed more than 9000 shots in my career. I’ve lost almost 300 games. 26 times I’ve been trusted to take the game winning shot and missed. I’ve failed over and over and over again in my life. And that is why I succeed. –Michael Jordan",
"The most difficult thing is the decision to act, the rest is merely tenacity. –Amelia Earhart",
"Every strike brings me closer to the next home run. –Babe Ruth",
"Definiteness of purpose is the starting point of all achievement. –W. Clement Stone",
"Life isn’t about getting and having, it’s about giving and being. –Kevin Kruse",
"Life is what happens to you while you’re busy making other plans. –John Lennon",
" We become what we think about. –Earl Nightingale",
"Twenty years from now you will be more disappointed by the things that you didn’t do than by the ones you did do, so throw off the bowlines, sail away from safe harbor, catch the trade winds in your sails.  Explore, Dream, Discover. –Mark Twain",
"Life is 10% what happens to me and 90% of how I react to it. –Charles Swindoll",
"The most common way people give up their power is by thinking they don’t have any. –Alice Walker",
"The mind is everything. What you think you become.  –Buddha",
"The best time to plant a tree was 20 years ago. The second best time is now. –Chinese Proverb",
"An unexamined life is not worth living. –Socrates",
"Eighty percent of success is showing up. –Woody Allen",
"Your time is limited, so don’t waste it living someone else’s life. –Steve Jobs",
"Winning isn’t everything, but wanting to win is. –Vince Lombardi",
"I am not a product of my circumstances. I am a product of my decisions. –Stephen Covey",
"Every child is an artist.  The problem is how to remain an artist once he grows up. –Pablo Picasso",
"You can never cross the ocean until you have the courage to lose sight of the shore. –Christopher Columbus",
"I’ve learned that people will forget what you said, people will forget what you did, but people will never forget how you made them feel. –Maya Angelou",
"Either you run the day, or the day runs you. –Jim Rohn",
"Whether you think you can or you think you can’t, you’re right. –Henry Ford",
" The two most important days in your life are the day you are born and the day you find out why. –Mark Twain",
"Whatever you can do, or dream you can, begin it.  Boldness has genius, power and magic in it. –Johann Wolfgang von Goethe",
"The best revenge is massive success. –Frank Sinatra",
"People often say that motivation doesn’t last. Well, neither does bathing.  That’s why we recommend it daily. –Zig Ziglar",
"Life shrinks or expands in proportion to one’s courage. –Anais Nin",
"Wisdom is the reward you get for a lifetime of listening when you'd have preferred to talk.' --Doug Larson",
"One of the most sincere forms of respect is actually listening to what another has to say.' --Bryant H. McGill",
"If you make listening and observation your occupation, you will gain much more than you can by talk.' --Robert Baden-Powell",
"Listening is a magnetic and strange thing, a creative force. The friends who listen to us are the ones we move toward. When we are listened to, it creates us, makes us unfold and expand.' --Karl A. Menniger",
"Most of the successful people I've known are the ones who do more listening than talking.' --Bernard Baruch",
" 'Listening is being able to be changed by the other person.' --Alan Alda",
"Everything in writing begins with language. Language begins with listening.' --Jeanette Winterson",
"There is as much wisdom in listening as there is in speaking--and that goes for all relationships, not just romantic ones.' --Daniel Dae Kim",
"The most important thing in communication is hearing what isn't said' --Peter Drucker",
"When people talk, listen completely. Most people never listen.' --Ernest Hemingway",
"Most people do not listen with the intent to understand; they listen with the intent to reply.' --Stephen R. Covey",
"Friends are those rare people who ask how we are, and then wait to hear the answer.' --Ed Cunningham",
"The art of conversation lies in listening.' --Malcom Forbes",
" 'You cannot truly listen to anyone and do anything else at the same time.' -M. Scott Peck",
"We have two ears and one tongue so that we would listen more and talk less.' --Diogenes",
"Stories are a communal currency of humanity.' --Tahir Shah, in Arabian Nights",
"Great stories happen to those who can tell them. ' --Ira Glass",
" 'The engineers of the future will be poets. ' --Terence McKenna",
" 'The human species thinks in metaphors and learns through stories.' --Mary Catherine Bateson",
"Sometimes reality is too complex. Stories give it form.' --Jean Luc Godard",
"Story is a yearning meeting an obstacle. ' --Robert Olen Butler",
"If you're going to have a story, have a big story, or none at all. ' --Joseph Campbell",
"Storytelling reveals meaning without committing the error of defining it.' --Hannah Arendt",
"The stories we tell literally make the world. If you want to change the world, you need to change your story. This truth applies both to individuals and institutions.' --Michael Margolis",
" 'Those who tell the stories rule the world.' --Hopi American Indian Proverb",
" 'There is no greater agony than bearing an untold story inside you.' --Maya Angelou",
"There's always room for a story that can transport people to another place.' --J.K. Rowling",
"Enlightenment is the key to everything, and it is the key to intimacy, because it is the goal of true authenticity.' --Marianne Willliamson",
"We need to find the courage to say no to the things and people that are not serving us if we want to rediscover ourselves and live our lives with authenticity.' --Barbara de Angelis",
"I know of nothing more valuable, when it comes to the all-important virtue of authenticity, than simply being who you are.' --Charles R. Swindoll",
"The keys to brand success are self-definition, transparency, authenticity and accountability.' --Simon Mainwaring",
"Yes, in all my research, the greatest leaders looked inward and were able to tell a good story with authenticity and passion.' --Deepak Chopra",
"Hard times arouse an instinctive desire for authenticity.'--Coco Chanel",
"Always be a first-rate version of yourself and not a second-rate version of someone else.' --Judy Garland",
"Be yourself--not your idea of what you think somebody else's idea of yourself should be.' --Henry David Thoreau",
"Shine with all you have. When someone tries to blow you out, just take their oxygen and burn brighter.' --Katelyn S. Irons",
" 'Live authentically. Why would you continue to compromise something that's beautiful to create something that is fake?' --Steve Maraboli",
" 'Authentic brands don't emerge from marketing cubicles or advertising agencies. They emanate from everything the company does. . .' --Howard Schultz",
" 'Authenticity requires a certain measure of vulnerability, transparency, and integrity' --Janet Louise Stepenson",
" 'We have to dare to be ourselves, however frightening or strange that self may prove to be.' --May Sarton",
" 'If you trade your authenticity for safety, you may experience the following: anxiety, depression, eating disorders, addiction, rage, blame, resentment, and inexplicable grief.' -Brene Brown",
" 'I can be a better me than anyone can.' -Diana Ross",
"There is no persuasiveness more effectual than the transparency of a single heart, of a sincere life.' --Joseph Barber Lightfoot",
"Transparency, honesty, kindness, good stewardship, even humor, work in businesses at all times.' --John Gerzema",
"A lack of transparency results in distrust and a deep sense of insecurity.'",
"I love when things are transparent, free and clear of all inhibition and judgment.' --Pharrell Williams",
" 'Eyes so transparent that through them the soul is seen.' --Theophile Gautier",
" 'Transparency is not the same as looking straight through a building: it's not just a physical idea, it's also an intellectual one.' --Helmut Jahn",
" 'I wish that every human life might be pure transparent freedom.' --Simone de Beauvoir",
" 'Truth never damages a cause that is just.' --Mahatma Gandhi",
" 'He had shown her all the workings of his soul, mistaking this for love.' --E.M. Forster",
" 'Our whole philosophy is one of transparency.' --Valerie Jarrett",
" 'A basic tenet of healthy democracy is open dialogue and transparency'--Peter Fenn",
" 'I just think we need more accountability and more transparency.' --John Thune",
" 'Honesty is the first chapter in the book of wisdom.' --Thomas Jefferson",
"Teamwork",
" 'Individual commitment to a group effort--that is what makes a team work, a company work, a society work, a civilization work.' --Vince Lombardi",
" 'Talent wins games, but teamwork and intelligence wins championships.' --Michael Jordan",
" 'Teamwork is the ability to work together toward a common vision. The ability to direct individual accomplishments toward organizational objectives. It is the fuel that allows common people to attain uncommon results.' --Andrew Carnegie",
" 'Alone we can do so little, together we can do so much.' --Helen Keller",
" 'Remember teamwork begins by building trust. And the only way to do that is to overcome our need for invulnerability.' --Patrick Lencioni",
" 'I invite everyone to choose forgiveness rather than division, teamwork over personal ambition.' --Jean-Francois Cope",
" 'None of us is as smart as all of us.' --Ken Blanchard",
" 'Coming together is a beginning. Keeping together is progress. Working together is success.' --Henry Ford",
" 'If everyone is moving forward together, then success takes care of itself.' --Henry Ford",
" 'The strength of the team is each individual member. The strength of each member is the team.' --Phil Jackson",
" 'Collaboration allows teachers to capture each other's fund of collective intelligence.' --Mike Schmoker",
" 'It takes two flints to make a fire.' --Louisa May Alcott",
" 'Unity is strength. . . when there is teamwork and collaboration, wonderful things can be achieved.' --Mattie Stepanek",
" 'To me, teamwork is the beauty of our sport, where you have five acting as one. You become selfless.' --Mike Krzyzewski",
" 'The best teamwork comes from men who are working independently toward one goal in unison.' --James Cash Penney",
"Responsiveness",
" 'I think that if you keep your eyes and your ears open and you are receptive to learning, there are skills you can get from any job at all.'--Cat Deeley",
" 'Seeking means: to have a goal; but finding means: to be free, to be receptive, to have no goal.'--Herman Hesse",
" 'It is impossible to withhold education from the receptive mind, as it is impossible to force it upon the unreasoning.' -Agnes Repplier",
" 'One of the things I've learned is to be receptive of feedback.' -Ben Silbermann",
" 'The best way to persuade people is with your ears--by listening to them. ' -Dean Rusk",
" 'Confidence, like art, never comes from having all the answers; it comes from being open to all the questions.' -Earl Gray Stevens",
" 'Life is 10% what happens to me and 90% how I react.' -John Maxwell",
" 'The pessimist complains about the wind; the optimist expects it to change; the realist adjusts the sails.' -William Arthur Ward",
" 'Relax & clear your mind if someone is speaking, so that you're receptive to what they're saying.'--Roger Ailes",
" 'The most difficult thing is the decision to act, the rest is merely tenacity.'-Emelia Earhart",
" 'Either you run the day, or they day runs you'-Jim Rohn",
"Adaptability",
" 'Tactics, fitness, stroke ability, adaptability, experience, and sportsmanship are all necessary for winning.' -Fred Perry",
" 'It is not the strongest or the most intelligent who will survive but those who can best manage change.' -Charles Darwin",
" 'Adaptability is about the powerful difference between adapting to cope and adapting to win.' -Max McKeown",
" 'The art of life is a constant readjustment to our surroundings.' -Kakuzo Okakaura",
" 'Adaptability is not imitation. It means power of resistance and assimilation.'",
"Mahatma Gandhi",
" 'You can't build an adaptable organization without adaptable people- and individuals change only when they have to, or when they want to.' -Gary Hamel",
" 'People will try to tell you that all the great opportunities have been snapped up. In reality, the world changes every second, blowing new opportunities in all directions, including yours.' -Ken Hakuta",
" 'Learn to adjust yourself to the conditions you have to endure, but make a point of trying to alter or correct conditions so that they are most favorable to you.' -William Frederick Book",
" 'All fixed set patterns are incapable of adaptability or pliability. The truth is outside of all fixed patterns.' ~ Bruce Lee",
" 'A wise man adapts himself to circumstances, as water shapes itself to the vessel that contains it.' -Chinese Proverb",
" 'Fall seven times and stand up eight.' -Japanese Proverb",
" 'When I let go of what I am, I become what I might be.'-Lao Tzu",
" 'You can't fall if you don't climb. But there's no joy in living your whole life on the ground.'- Unknown",
"Passion",
" 'Every great dream begins with a dreamer. Always remember, you have within you the strength, the patience, and the passion to reach for the stars to change the world.' -Harriet Tubman",
" 'There is no passion to be found playing small--in settling for a life that is less than the one you are capable of living.' -Nelson Mandela",
" 'Develop a passion for learning. If you do, you will never cease to grow.' -Anthony J. D'Angelo",
" 'Passion is energy. Feel the power that comes from focusing on what excites you.' -Oprah Winfrey",
" 'If passion drives you, let reason hold the reins.' -Benjamin Franklin",
" 'We must act out passion before we can feel it.' -Jean-Paul Sartre",
" 'It is obvious that we can no more explain a passion to a person who has never experienced it than we can explain light to the blind.'--T. S. Eliot",
" 'Nothing is as important as passion. No matter what you want to do with your life, be passionate.' -Jon Bon Jovi",
" 'You can't fake passion.' -Barbara Corcoran",
" 'You have to be burning with 'an idea, or a problem, or a wrong that you want to right.' If you're not passionate enough from the start, you'll never stick it out.' Steve Jobs",
" 'Yes, in all my research, the greatest leaders looked inward and were able to tell a good story with authenticity and passion.' -Deepak Chopra",
" 'If you feel like there's something out there that you're supposed to be doing, if you have a passion for it, then stop wishing and just do it.' -Wanda Skyes",
" 'If you don't love what you do, you won't do it with much conviction or passion.' -Mia Hamm",
" 'There is no end. There is no beginning. There is only the passion of life. There is no end. There is no beginning. There is only the passion of life.' -Federico Fellini",
" 'Any guy that's not working with the same amount of intensity and passion that I do, I don't want to know.' -Zakk Wylde",
" It is the soul's duty to be loyal to its own desires. It must abandon itself to its master passion. -Rebecca West",
"Surprise and Delight",
" 'The husband who decides to surprise his wife is often very much surprised himself.'--Voltaire",
" 'Never tell people how to do things. Tell them what to do and they will surprise you with their ingenuity.' -George S. Patton",
" 'A story to me means a plot where there is some surprise. Because that is how life is--full of surprises.' -Isaac Bashevis Singer",
" 'Truth is so rare that it is delightful to tell it.' -Emily Dickinson",
" 'It doesn't take much to surprise others, but to surprise oneself- now that is a great feat.' -Kristen Hartley",
" 'Surprise yourself every day with your own courage.' -Denholm Elliott",
" 'To the issues of friendship, love, business and war, 'surprise' is the optimistic solution.' -Amit Kalantri",
" 'Never tell people how to do things. Tell them what to do and they will surprise you with their ingenuity.' -George S. Patton",
" 'A story to me means a plot where there is some surprise. Because that is how life is--full of surprises.' -Isaac Bashevis Singer",
" 'People tend to play in their comfort zone, so the best things are achieved in a state of surprise, actually.' -Brian Eno",
" 'Simplicity is the ultimate sophistication.' -Leonardo da Vinci",
" 'There is no greatness where there is no simplicity, goodness and truth.' -Leo Tolstoy",
" 'Manifest plainness, embrace simplicity, reduce selfishness, have few desires.' -Lao Tzu",
" 'Simplicity and repose are the qualities that measure the true value of any work of art.' -Frank Lloyd Wright",
" 'Simplicity is the most difficult thing to secure in this world; it is the last limit of experience and the last effort of genius.' -George Sand",
" 'There is a certain majesty in simplicity which is far above all the quaintness of wit.' -Alexander Pope",
" 'Simplicity is not the goal. It is the by-product of a good idea and modest expectations.' -Paul Rand",
" 'Simplicity is prerequisite for reliability.' -Edsger Dijkstra",
" 'Simplicity, clarity, singleness: These are the attributes that give our lives power and vividness and joy as they are also the marks of great art. They seem to be the purpose of God for his whole creation.' -Richard Holloway",
" 'If you haven't done much giving in your life-try it and see how you feel afterwards.' Michelle Moore, Selling Simplified",
" 'If you can't explain it to a six year old, you don't understand it yourself.' -Albert Einstein",
" 'Life is really simple, but we insist on making it complicated.'- Aristotle",
" Simplicity is about subtracting the obvious and adding the meaningful.' John Maeda,",
" 'Free yourself from the complexities of your life! A life of simplicity and happiness awaits you.'--Steve Maraboli",
" 'Nature is pleased with simplicity. And nature is no dummy' -Isaac Newton",
" 'If you will stay close to nature, to its simplicity, to the small things hardly noticeable, those things can unexpectedly become great and immeasurable.' Rainer Maria Rilke",
" 'Reflect upon your present blessings--of which every man has many--not on your past misfortunes, of which all men have some.' -Charles Dickens,",
" 'The truest indication of gratitude is to return what you are grateful for.' -Richard Paul Evans",
" 'I work very hard, and I play very hard. I'm grateful for life. And I live it--I believe life loves the liver of it. I live it.' -Maya Angelou",
" 'When you are grateful--when you can see what you have--you unlock blessings to flow in your life.'--Suze Orman",
" 'We must find time to stop and thank the people who make a difference in our lives.' -John F. Kennedy",
" 'True forgiveness is when you can say, 'Thank you for that experience.' -Oprah Winfrey",
" 'You can complain because roses have thorns, or you can be grateful that thorn bushes have roses.' -Tom Wilson",
" 'Do not spoil what you have by desiring what you have not; remember that what you now have was once among the things you only hoped for.' -Epicurus",
" 'Cultivate the habit of being grateful for every good thing that comes to you, and to give thanks continuously. And because all things have contributed to your advancement, you should include all things in your gratitude.' -Ralph Waldo Emerson",
" 'Let gratitude be the pillow upon which you kneel to say your nightly prayer. And let faith be the bridge you build to overcome evil and welcome good.'",
"Maya Angelou, Celebrations: Rituals of Peace and Prayer",
" 'We can only be said to be alive in those moments when our hearts are conscious of our treasures.' -Thornton Wilder",
" 'As we express our gratitude, we must never forget that the highest appreciation is not to utter words, but to live by them.' -John F. Kennedy",
" 'Gratitude is not only the greatest of virtues, but the parent of all others.' -Cicero",
" 'Those who have the ability to be grateful are the ones who have the ability to achieve greatness.' -Steve Maraboli",
" 'This is my simple religion. There is no need for temples; no need for complicated philosophy. Our own brain, our own heart is our temple; the philosophy is kindness.' -Dalai Lama",
" 'A warm smile is the universal language of kindness.' -William Arthur Ward",
" 'Truth is a deep kindness that teaches us to be content in our everyday life and share with the people the same happiness.' -Khalil Gibran",
" 'There is overwhelming evidence that the higher the level of self-esteem, the more likely one will be to treat others with respect, kindness, and generosity.' -Nathaniel Branden",
" 'Wherever there is a human being, there is an opportunity for a kindness.' -Lucius Annaeus Seneca",
" 'Treat everyone with respect and kindness. Period. No exceptions.' -Kiana Tom",
" 'Be kind, for everyone you meet is fighting a harder battle.'--Plato",
" 'Kindness is a language which the deaf can hear and the blind can see.' -Mark Twain",
" 'The smallest act of kindness is worth more than the greatest intention.' -Kahlil Gibran, The Essential Kahlil Gibran",
" 'I would rather make mistakes in kindness and compassion than work miracles in unkindness and hardness.' -Mother Teresa",
" 'Never lose a chance of saying a kind word. ' -William Makepeace Thackeray, Vanity Fair",
" 'Our kindness may be the most persuasive argument for that which we believe.' -Gordon B. Hinckley",
" 'It's a little embarrassing that after 45 years of research & study, the best advice I can give people is to be a little kinder to each other.' -Aldous Huxley",
" 'It seems to me that no matter what religion you subscribe to, acts of kindness are the stepping-stones to making the world a better place--because we become better people in it.' -Jodi Picoult",
" 'Never look down on anybody unless you're helping them up.' Jesse Jackson",
" 'Humility is not thinking less of yourself, it's thinking of yourself less.' -C. S. Lewis",
" 'Pride makes us artificial and humility makes us real.' -Thomas Merton",
" 'Thank you' is the best prayer that anyone could say. I say that one a lot. Thank you expresses extreme gratitude, humility, understanding.' -Alice Walker",
" 'The greatest friend of truth is Time, her greatest enemy is Prejudice, and her constant companion is Humility.' -Charles Caleb Colton",
" 'The proud man can learn humility, but he will be proud of it.' -Mignon McLaughlin",
" 'Real genius is nothing else but the supernatural virtue of humility in the domain of thought.' -Simone Weil",
" 'Humility is really important because it keeps you fresh and new.' -Steven Tyler",
" 'Humility, that low, sweet root, from which all heavenly virtues shoot.' -Thomas Moore",
" 'Humility is throwing oneself away in complete concentration on something or someone else.' Madeleine L'Engle",
" 'Pride must die in you, or nothing of heaven can live in you.' Andrew Murray, Humility",
" 'Humility is nothing but truth, and pride is nothing but lying.' St. Vincent de Paul",
" 'One cannot be humble and aware of oneself at the same time.' Madeleine L'Engle, A Circle of Quiet",
" 'Selflessness is humility. Humility and freedom go hand in hand. Only a humble person can be free.' -Jeff Wilson",
" 'Have more humility. Remember you don't know the limits of your own abilities. Successful or not, if you keep pushing beyond yourself, you will enrich your own life--and maybe even please a few strangers.' A.L. Kennedy",
" 'No one has ever become poor by giving.' -Anne Frank",
" 'A kind gesture can reach a wound that only compassion can heal.' -Steve Maraboli",
" 'Give, but give until it hurts.' -Mother Teresa",
" 'As we work to create light for others, we naturally light our own way.' -Mary Anne Radmacher",
" Until we can receive with an open heart, we're never really giving with an open heart. When we attach judgment to receiving help, we knowingly or unknowingly attach judgment to giving help.' -Bren Brown",
" 'Even the smallest act of caring for another person is like a drop of water -it will make ripples throughout the entire pond...' -Jessy and Bryan Matteo",
" 'Don't wait for other people to be loving, giving, compassionate, grateful, forgiving, generous, or friendly... lead the way!' -Steve Maraboli",
" 'What we spend, we lose. What we keep will be left for others. What we give away will be ours forever.' -David McGee",
" 'There is a very real relationship, both quantitatively and qualitatively, between what you contribute and what you get out of this world.' -Oscar Hammerstein II",
" 'When in doubt as to what you should do, err on the side of giving.'-Tony Cleaver",
" 'I wish I were like Facebook; being able to 'like' and 'share' everything I get.' -Ashok Kallarakkal",
" 'Just like downing a powerful caffeine drink, 'reaching out to others' pays that big 'life energizer dividend!' -Wes Adamson",
" 'Every sunrise is an invitation for us to arise and brighten someone's day.' -Richelle E. Goodrich, Smile Anyway",
" 'Nothing in the world can take the place of persistence. Talent will not; nothing is more common than unsuccessful men with talent. Genius will not; unrewarded genius is almost a proverb. Education will not; the world is full of educated derelicts. Persistence and determination alone are omnipotent.' -Calvin Coolidge",
" 'Energy and persistence conquer all things.' -Benjamin Franklin",
" 'As long as we are persistence in our pursuit of our deepest destiny, we will continue to grow. We cannot choose the day or time when we will fully bloom. It happens in its own time.' -Denis Waitley",
" 'Success is almost totally dependent upon drive and persistence. The extra energy required to make another effort or try another approach is the secret of winning.' -Denis Waitley",
" 'Never let your persistence and passion turn into stubbornness and ignorance.' -Anthony J. D'Angelo",
" 'The difference between people who believe they have books inside of them and those who actually write books is sheer cussed persistence--the ability to make yourself work at your craft, every day--the belief, even in the face of obstacles, that you've got something worth saying.' -Jennifer Weiner",
" 'When I meet successful people I ask 100 questions as to what they attribute their success to. It is usually the same: persistence, hard work and hiring good people.' -Kiana Tom",
" 'Success is stumbling from failure to failure with no loss of enthusiasm.' -Winston Churchill",
" 'The best way out is always through.' -Robert Frost",
" 'If you have an important point to make, don't try to be subtle or clever. Use a pile driver. Hit the point once. Then come back and hit it again. Then hit it a third time--a tremendous whack.' -Winston Churchill",
" 'Knowing trees, I understand the meaning of patience. Knowing grass, I can appreciate persistence.' -Hal Borland",
" 'Character consists of what you do on the third and fourth tries.' -James A. Michener",
" 'Keep a little fire burning; however small, however hidden.' 'Keep a little fire burning; however small, however hidden.' -Cormac McCarthy, The Road",
" 'No matter how much falls on us, we keep plowing ahead. That's the only way to keep the roads clear.' -Greg Kincaid",
" 'If you wish to be out front, then act as if you were behind.' -Lao Tzu",
" 'The slogans 'hang on' and 'press on' have solved and will continue to solve the problems of humanity.' -Ogwo David Emenike",
" 'It is during our darkest moments that we must focus to see the light.' -Aristotle Onassis",
" 'You never have to change anything you got up in the middle of the night to write.' -Saul Bellow",
" 'Do you want to know who you are? Don't ask. Act! Action will delineate and define you.' Thomas Jefferson",
" 'The best way to predict the future is to invent it.' -Alan Kay",
" 'If you have built castles in the air, your work need not be lost; that is where they should be. Now put the foundations under them.' -Henry David Thoreau",
" 'I know for sure that what we dwell on is who we become.' -Oprah Winfrey",
" 'Logic will get you from A to B. Imagination will take you everywhere.' -Albert Einstein",
" 'If not us, who? If not now, when?' -Hillel the Elder",
" 'Winners lose much more often than losers. So if you keep losing but you're still trying, keep it up! You're right on track.'--Matthew Keith Groves",
" 'Success is not the key to happiness. Happiness is the key to success. If you love what you are doing, you'll be a success.'--Albert Schweitzer",
" 'When the need to succeed is as bad as the need to breathe, then you'll be successful.' -Eric Thomas",
" 'If you want to test your memory, try to recall what you were worrying about one year ago today.' -E.Joseph Coffman",
" 'What you do speaks so loudly that I cannot hear what you say.' -Ralph Waldo Emerson",
"Life",
" 'In three words I can sum up everything I've learned about life: it goes on.' -Robert Frost",
" 'Life is really simple, but we insist on making it complicated.' -Confucius",
" 'Go confidently in the direction of your dreams. Live the life you have imagined.'--Henry David Thoreau",
" 'Only a life lived for others is a life worthwhile.' -Albert Einstein",
" 'Change is the law of life. And those who look only to the past or present are certain to miss the future.' -John F. Kennedy",
" 'Life is a succession of lessons which must be lived to be understood.' -Helen Keller",
" 'The price of anything is the amount of life you exchange for it.' -Henry David Thoreau",
" 'Life is far too important a thing ever to talk seriously about.' -Oscar Wilde",
" 'Don't let life discourage you; everyone who got where he is had to begin where he was.' -Richard L. Evans",
" 'The only disability in life is a bad attitude.' -Scott Hamilton",
" 'Every creature is better alive than dead, men and moose and pine trees, and he who understands it aright will rather preserve its life than destroy it.' -Henry David Thoreau",
" 'Don't let life discourage you; everyone who got where he is had to begin where he was.' -Richard L. Evans",
" 'Not life, but good life, is to be chiefly valued.' -Socrates",
" 'If you live long enough, you'll make mistakes. But if you learn from them, you'll be a better person. It's how you handle adversity, not how it affects you. The main thing is never quit, never quit, never quit.' -William J. Clinton",
" 'Life in abundance comes only through great love.' -Elbert Hubbard",
" 'Love is not maximum emotion. Love is maximum commitment.' -Sinclair B. Ferguson",
" 'Although love is communicated in a number of ways,our words often reflect the condition of our heart.' -Jennifer Dion",
" 'Keep love in your heart. A life without it is like a sunless garden when the flowers are dead.' -Oscar Wilde",
" 'Sometimes the heart sees what is invisible to the eye.' -H. Jackson Brown, Jr.",
" 'I have decided to stick with love. Hate is too great a burden to bear.' -Martin Luther King, Jr.",
" 'Where there is love there is life.' -Mahatma Gandhi",
" 'Let us always meet each other with smile, for the smile is the beginning of love.'",
"-Mother Teresa",
" 'A loving heart is the beginning of all knowledge.' -Thomas Carlyle",
" 'Love begins at home, and it is not how much we do... but how much love we put in that action.' -Mother Teresa",
" 'A flower cannot blossom without sunshine, and man cannot live without love.' -Max Muller",
" 'We love life, not because we are used to living but because we are used to loving.' -Friedrich Nietzsche",
" 'Love yourself first and everything else falls into line. You really have to love yourself to get anything done in this world.' -Lucille Ball",
" 'Love is a better teacher than duty.'-Albert Einstein",
" 'The best proof of love is trust.' -Joyce Brothers",
" 'Love is life. And if you miss love, you miss life.' -Leo Buscaglia",
" 'Faith makes all things possible... love makes all things easy.' -Dwight L. Moody",
" 'Change is hardest at the beginning, messiest in the middle and best at the end.' -Robin S. Sharma,",
" 'Be the change that you wish to see in the world.' -Mahatma Gandhi",
" 'Things change. And friends leave. Life doesn't stop for anybody.' -Stephen Chbosky",
" 'Never doubt that a small group of thoughtful, committed, citizens can change the world. Indeed, it is the only thing that ever has.' -Margaret Mead",
" 'Everyone thinks of changing the world, but no one thinks of changing himself.' -Leo Tolstoy",
" 'Education is the most powerful weapon which you can use to change the world.' -Nelson Mandela",
" 'Change will not come if we wait for some other person, or if we wait for some other time. We are the ones we've been waiting for. We are the change that we seek.' -Barack Obama",
" 'They always say time changes things, but you actually have to change them yourself.' -Andy Warhol",
" 'We are taught you must blame your father, your sisters, your brothers, the school, the teachers--but never blame yourself. It's never your fault. But it's always your fault, because if you wanted to change you're the one who has got to change.' -Katharine Hepburn",
" 'Change the way you look at things and the things you look at change.' -Wayne W. Dyer",
" 'No matter who you are, no matter what you did, no matter where you've come from, you can always change, become a better version of yourself.' -Madonna",
" 'Those who cannot change their minds cannot change anything.' -George Bernard Shaw",
" 'I alone cannot change the world, but I can cast a stone across the waters to create many ripples.' -Mother Theresa",
" 'Change is the end result of all true learning.' -Leo Buscaglia",
" 'To improve is to change; to be perfect is to change often.' -Winston Churchill",
" 'Life belongs to the living, and he who lives must be prepared for changes' -Johann Wolfgang von Goethe",
" 'If you want to change the world, pick up your pen and write.' -Martin Luther",
" 'A tiny change today brings a dramatically different tomorrow.' -Richard Bach",
" 'Change your thoughts and you change your world.' -Norman Vincent Peale",
" 'Forgiveness does not change the past, but it does enlarge the future' -Paul Boese",
" 'Some people believe holding on and hanging in there are signs of great strength. However, there are times when it takes much more strength to know when to let go and then do it.' -Ann Landers",
" 'The beautiful journey of today can only begin when we learn to let go of yesterday.' -Steve Maraboli",
" 'Thank God I found the GOOD in goodbye' -Beyonc Knowles",
" 'The weak can never forgive. Forgiveness is the attribute of the strong.'--Mahatma Gandhi",
" 'Always forgive your enemies--nothing annoys them so much.' -Oscar Wilde",
" 'Forgiveness is the fragrance that the violet sheds on the heel that has crushed it.' -Mark Twain",
" 'When you forgive, you in no way change the past--but you sure do change the future.' -Bernard Meltzer",
" 'People can be more forgiving than you can imagine. But you have to forgive yourself. Let go of what's bitter and move on.' -Bill Cosby",
" 'To err is human; to forgive, divine.' -Alexander Pope",
" 'There is no love without forgiveness, and there is no forgiveness without love.'",
"-Bryant H. McGill",
" 'Forgiveness is a funny thing. It warms the heart and cools the sting.' -William Arthur Ward",
" 'It takes one person to forgive, it takes two people to be reunited.' -Lewis B. Smedes",
" 'Forgiveness is a gift you give yourself.' -Suzanne Somers",
" 'Without forgiveness, there's no future.' -Desmond Tutu",
" 'Sooner or later we've all got to let go of our past.' -Dan Brown",
" 'Yesterday is not ours to recover, but tomorrow is ours to win or lose.' -Lyndon B. Johnson",
" 'The great courageous act that we must all do, is to have the courage to step out of our history and past so that we can live our dreams.' -Oprah Winfrey",
" 'I've learned through the years that it's not where you live, it's the people who surround you that make you feel at home.' -J.B. McGee",
" 'The love of family and the admiration of friends is much more important than wealth and privilege.' -Charles Kuralt",
" 'Family is not an important thing. It's everything.' -Michael J. Fox",
" 'Family is the most important thing in the world.' -Princess Diana",
" 'A happy family is but an earlier heaven.' -George Bernard Shaw",
" 'A man should never neglect his family for business.' -Walt Disney",
" 'To us, family means putting your arms around each other and being there.' -Barbara Bush",
" 'Without a family, man, alone in the world, trembles with the cold.' -Andre Maurois",
" 'Cherish your human connections--your relationships with friends and family.' -Barbara Bush",
" 'In every conceivable manner, the family is link to our past, bridge to our future.'",
"-Alex Haley",
" 'The family is one of nature's masterpieces.' -George Santayana",
" 'The family is the first essential cell of human society.' -Pope John XXIII",
" 'Family means no one gets left behind or forgotten.' -David Ogden Stiers",
" 'A man travels the world over in search of what he needs, and returns home to find it.' -George Moore",
" 'I sustain myself with the love of family.' -Maya Angelou",
" 'Home is where you are loved the most and act the worst.' -Marjorie Pay Hinckley",
" 'You are born into your family and your family is born into you. No returns. No exchanges.' -Elizabeth Berg",
" 'Home is people. Not a place. If you go back there after the people are gone, then all you can see is what is not there any more.' -Robin Hobb",
"Strength & Courage",
" 'That which does not kill us makes us stronger.' -Friedrich Nietzsche",
" 'We are only as strong as we are united, as weak as we are divided.' -J.K. Rowling",
" 'Courage is the most important of all the virtues because without courage, you can't practice any other virtue consistently.' -Maya Angelou",
" 'A brave man acknowledges the strength of others.' -Veronica Roth",
" 'Strength does not come from physical capacity. It comes from an indomitable will.' -Mahatma Gandhi",
" 'It is easier to build strong children than to repair broken men.' -Frederick Douglass",
" 'With the new day comes new strength and new thoughts.' -Eleanor Roosevelt",
" 'The world breaks every one and afterward many are strong at the broken places.' -Ernest Hemingway",
" 'Be strong. Live honorably and with dignity. When you don't think you can, hold on.' -James Frey",
" 'You feel your strength in the experience of pain.' -Jim Morrison",
" 'My attitude is that if you push me towards something that you think is a weakness, then I will turn that perceived weakness into a strength.' -Michael Jordan",
" 'Its not always necessary to be strong, but to feel strong.' -Jon Krakauer",
" 'We acquire the strength we have overcome.' -Ralph Waldo Emerson",
" 'Mastering others is strength. Mastering oneself makes you fearless.' -Lao Tzu",
" 'We gain strength, and courage, and confidence by each experience in which we really stop to look fear in the face... we must do that which we think we cannot.' -Eleanor Roosevelt",
" 'Character cannot be developed in ease and quiet. Only through experience of trial and suffering can the soul be strengthened, ambition inspired, and success achieved.' -Helen Keller",
" 'Shallow men believe in luck. Strong men believe in cause and effect.' -Ralph Waldo Emerson",
" 'Leadership is service, not position.' -Tim Fargo",
" 'A leader is a dealer in hope.' -Napoleon Bonaparte",
" 'A leader is one who knows the way, goes the way, and shows the way.' -John Maxwell",
" 'No man will make a great leader who wants to do it all himself, or to get all the credit for doing it.' -Andrew Carnegie",
" 'As we look ahead into the next century, leaders will be those who empower others.' -Bill Gates",
" 'A genuine leader is not a searcher for consensus but a molder of consensus.'",
"-Martin Luther King, Jr.",
" 'Effective leadership is not about making speeches or being liked; leadership is defined by results not attributes.' -Peter Drucker",
" 'Innovation distinguishes between a leader and a follower.' -Steve Jobs",
" 'Be the chief but never the lord.' -Lao Tzu",
" 'The speed of the team is the speed of the boss. '-Barbara Corcoran",
" 'Leaders think and talk about the solutions. Followers think and talk about the problems.' -Brian Tracy",
" 'Leadership and learning are indispensable to each other.' --John F. Kennedy",
" 'To handle yourself, use your head; to handle others, use your heart.' -Eleanor Roosevelt",
" 'Management is nothing more than motivating other people.' -Lee Iacocca",
" 'Management is efficiency in climbing the ladder of success; leadership determines whether the ladder is leaning against the right wall.' -Stephen Covey",
" 'The price of greatness is responsibility.' -Winston Churchill",
" 'The key to successful leadership today is influence, not authority.' -Kenneth Blanchard",
" 'Create with the heart; build with the mind.' -Criss Jami",
" 'No legacy is so rich as honesty.' -William Shakespeare",
" 'Carve your name on hearts, not tombstones. A legacy is etched into the minds of others and the stories they share about you.' -Shannon L. Alder",
" 'If you would not be forgotten as soon as you are dead, either write something worth reading or do something worth writing.' -Benjamin Franklin",
" 'The things you do for yourself are gone when you are gone, but the things you do for others remain as your legacy.' -Kalu Ndukwe Kalu",
" 'The only thing you take with you when you're gone is what you leave behind.' ~John Allston",
" 'I agree with you entirely in condemning the mania of giving names to objects of any kind after persons still living. Death alone can seal the title of any man to this honor, by putting it out of his power to forfeit it.' --Thomas Jefferson",
" 'The greatest use of life is to spend it for something that will outlast it.' --William James",
" 'You can't leave a footprint that lasts if you're always walking on tiptoe.' --Marion Blakely",
};
       public static string GetDailyQuotesOfTheDay()
       {
           //int month = DateTime.Now.Month;
           //int monthSequence = month % 2;
           int dayOfYear = DateTime.Now.DayOfYear;
           //if(monthSequence==1)
           //{
           //    return quotes[dayOfYear];
           //}
           //else
           //{
           //    return quotes1[dayOfYear];
           //}

           return quoteYearList[dayOfYear];


           

       }

    }
}
