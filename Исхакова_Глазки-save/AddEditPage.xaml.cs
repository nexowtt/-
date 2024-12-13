﻿using Microsoft.Win32;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Agent _currentAgent = new Agent();

        public AddEditPage(Agent SelectedAgent)
        {

            InitializeComponent();
            if (SelectedAgent != null)
            {
                _currentAgent = SelectedAgent;
            }
            DataContext = _currentAgent;
            /*
            switch (_currentAgent.AgentTypeID)
            {
                case 1:
                    if (ComboType.Items.Count > 0)
                    {
                        ComboType.SelectedItem = ComboType.Items[0]; //Или значение напрямую, если это не строка
                    }
                    break;
                default:
                    break;
            }*/
            // Загружаем типы агентов напрямую в ComboBox
            ComboType.ItemsSource = Entities.GetContext().AgentType.ToList();
            ComboType.DisplayMemberPath = "Title"; // Отображаемые названия
            ComboType.SelectedValuePath = "ID";   // Идентификатор для привязки
            ComboType.SelectedValue = _currentAgent.AgentTypeID; // Устанавливаем начальное значение




        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentAgent.Title))
                errors.AppendLine("Укажите наименование агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.Address))
                errors.AppendLine("Укажите адрес агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.DirectorName))
                errors.AppendLine("Укажите ФИО директора");

            if (ComboType == null)
                errors.AppendLine("Укажите типа агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента");

            if (_currentAgent.Priority <= 0)
                errors.AppendLine("дукажите положительный приоритет агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.INN))
                errors.AppendLine("Укажите ИННН агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = _currentAgent.Phone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "");
                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11) || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("укажите правильно телефон агента");
            }

            if (string.IsNullOrWhiteSpace(_currentAgent.Email))
                errors.AppendLine("Укажите почту агента");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (_currentAgent.ID == 0)
                Entities.GetContext().Agent.Add(_currentAgent);
            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void ChangePicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                _currentAgent.Logo = myOpenFileDialog.FileName;
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var currentAgent = (sender as Button).DataContext as Agent;
            var currentSaleAgent = Entities.GetContext().ProductSale.ToList();
            currentSaleAgent = currentSaleAgent.Where(p => p.AgentID == currentAgent.ID).ToList();
            if (currentSaleAgent.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, так как существует записи на эту услугу");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Entities.GetContext().Agent.Remove(currentAgent);
                        Entities.GetContext().SaveChanges();
                        Manager.MainFrame.GoBack();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void LogoImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            LogoImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            LogoImage.UpdateLayout();
        }

    }

}
