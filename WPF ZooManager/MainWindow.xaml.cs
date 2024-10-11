using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace WPF_ZooManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //create a sql connection object 
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();

            //setting database conection
            string connectionString = ConfigurationManager.ConnectionStrings["WPF_ZooManager.Properties.Settings.ZoosDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            ShowZoos();
            ShowAllAnimals();
        }

        private void ShowZoos()
        {
            //Note that is Always important to have try,catch
            //when you dealing with the data connection
            
            try
            {
                // So what it's going to do is this SQL adapter will
                // run this query into that connection.
                //The SqlDataAdapter can be imagend like Interfce to make
                //Tables useble by C#-Objects
                string query = "select * from Zoo";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                //So what this data table allows us is to store data
                //from tables within a object
                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();

                    //now we can run the SqlDataAdapter
                    sqlDataAdapter.Fill(zooTable);

                    //Which information of the Table in the DataTable
                    //should be shown in our listBox?
                    listZoos.DisplayMemberPath = "Location";

                    //Which value should be delivered when an item from
                    //a list box is selected?
                    listZoos.SelectedValuePath = "Id";

                    //The Reference to the data the listBox should populate.
                    listZoos.ItemsSource = zooTable.DefaultView;
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void ShowAssociatedAnimals()
        {
            try
            {
                string query = "select * from Animal a inner join ZooAnimal za " +
                    "on a.Id = za.AnimalId where za.ZooId = @ZooId";

                //Now in order to use @ZooId as a value we should
                //use SqlCommand object

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);

                    DataTable animalTable = new DataTable();

                    sqlDataAdapter.Fill(animalTable);

                    listAssociatedAnimals.DisplayMemberPath = "Name";

                    listAssociatedAnimals.SelectedValuePath = "Id";

                    listAssociatedAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ShowAllAnimals()
        {

            try
            {

                string query = "select * from Animal";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable animalTable = new DataTable();

                    //now we can run the SqlDataAdapter
                    sqlDataAdapter.Fill(animalTable);

                    //Which information of the Table in the DataTable
                    //should be shown in our listBox?
                    ListAllAnimals.DisplayMemberPath = "Name";

                    //Which value should be delivered when an item from
                    //a list box is selected?
                    ListAllAnimals.SelectedValuePath = "Id";

                    //The Reference to the data the listBox should populate.
                    ListAllAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
        }

        private void AllAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAllAnimals();
        }

        private void Click_DeleteZoo(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Zoo where id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
            } catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());  
            } 
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }

            
            
            
        }
    }
}
