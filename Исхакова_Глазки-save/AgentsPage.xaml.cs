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
        public AgentsPage()
        {
            InitializeComponent();
            var currentAgents = Entities.GetContext().Agent.ToList();
            AgentListView.ItemsSource = currentAgents;
            ComboSort.SelectedIndex = 0;
            ComboType.SelectedIndex = 0;

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
    }
}
