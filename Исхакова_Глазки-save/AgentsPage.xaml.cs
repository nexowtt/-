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

namespace Исхакова_Глазки_save
{
    /// <summary>
    /// Логика взаимодействия для AgentsPage.xaml
    /// </summary>
    public partial class AgentsPage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        List<Agent> CurrentPageList = new List<Agent>();
        List<Agent> TableList;
        public AgentsPage()
        {

            InitializeComponent();
            var currentAgents = Entities.GetContext().Agent.ToList();
            AgentListView.ItemsSource = currentAgents;
            ComboSort.SelectedIndex = 0;
            ComboType.SelectedIndex = 0;

            UpdateAgents();
        }

        private void UpdateAgents()
        {
            var currentAgents = Entities.GetContext().Agent.ToList();

            if (ComboType.SelectedIndex == 1)
                currentAgents = currentAgents.Where(p => p.AgentTypeID==1).ToList();
            if (ComboType.SelectedIndex == 2)
                currentAgents = currentAgents.Where(p => p.AgentTypeID==2).ToList();
            if (ComboType.SelectedIndex == 3)
                currentAgents = currentAgents.Where(p => p.AgentTypeID==3).ToList();
            if (ComboType.SelectedIndex == 4)
                currentAgents = currentAgents.Where(p => p.AgentTypeID==4).ToList();
            if (ComboType.SelectedIndex == 5)
                currentAgents = currentAgents.Where(p => p.AgentTypeID==5).ToList();
            if (ComboType.SelectedIndex == 6)
                currentAgents = currentAgents.Where(p => p.AgentTypeID==6).ToList();

            currentAgents = currentAgents.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())
              || p.Email.ToLower().Contains(TBoxSearch.Text.ToLower())
              || p.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Contains(TBoxSearch.Text.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""))).ToList();


            if (ComboSort.SelectedIndex == 1)
             currentAgents = currentAgents.OrderBy(p => p.Title).ToList();
            if (ComboSort.SelectedIndex == 2)
                currentAgents = currentAgents.OrderByDescending(p => p.Title).ToList();
            if (ComboSort.SelectedIndex == 3)
                currentAgents = currentAgents.OrderBy(p => p.Title).ToList();
            if (ComboSort.SelectedIndex == 4)
                AgentListView.ItemsSource = currentAgents.OrderByDescending(p => p.Title).ToList();
            if (ComboSort.SelectedIndex == 5)
                currentAgents = currentAgents.OrderBy(p => p.Priority).ToList();
            if (ComboSort.SelectedIndex == 6)
                currentAgents = currentAgents.OrderByDescending(p => p.Priority).ToList();

            AgentListView.ItemsSource = currentAgents.ToList();
            TableList=currentAgents;
            ChangePage(0, 0);
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            if (CountRecords % 10 > 0)
                CountPage = CountRecords / 10 + 1;
            else
                CountPage = CountRecords / 10;
            Boolean IfUpdate = true;
            int min;
            if (selectedPage.HasValue)
            {
                if (selectedPage >=0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                        CurrentPageList.Add(TableList[i]);
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                                CurrentPageList.Add(TableList[i]);
                        }
                        else
                            IfUpdate = false;
                        break;
                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                                CurrentPageList.Add(TableList[i]);
                        }
                        else
                            IfUpdate = false;
                        break;
                }
            }
            if (IfUpdate)
            {
                PageListBox.Items.Clear();
                for (int i = 1; i <=CountPage; i++)
                    PageListBox.Items.Add(i);
                PageListBox.SelectedIndex = CurrentPage;
                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = " из " + CountRecords.ToString();
                AgentListView.ItemsSource = CurrentPageList;
                AgentListView.Items.Refresh();
            }
        }


        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
            UpdateAgents();

        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                AgentListView.ItemsSource = Entities.GetContext().Agent.ToList();
                UpdateAgents();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Agent));
            UpdateAgents();

        }
    }
}
