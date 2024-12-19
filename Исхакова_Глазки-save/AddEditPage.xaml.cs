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

        private ProductSale _currentProductSale = new ProductSale();

        private CollectionViewSource _productsView;
        public AddEditPage(Agent SelectedAgent)
        {

            InitializeComponent();
            if (SelectedAgent != null)
            {
                _currentAgent = SelectedAgent;
            }
            DataContext = _currentAgent;
            DataContext = _productsView;
            ComboType.ItemsSource = Entities.GetContext().AgentType.ToList();
            ComboType.DisplayMemberPath = "Title"; // Отображаемые названия
            ComboType.SelectedValuePath = "ID";   // Идентификатор для привязки
            ComboType.SelectedValue = _currentAgent.AgentTypeID; // Устанавливаем начальное значение
            realize.ItemsSource = Entities.GetContext().ProductSale.ToList();

            int selectAgentID = _currentAgent.ID;
            var filtrSale = Entities.GetContext().ProductSale.Where(sale => sale.AgentID == selectAgentID).ToList();
            realize.ItemsSource= filtrSale;
            realize.DisplayMemberPath = "Datacount";
            realize.SelectedValuePath = "AgentID";
            //int selectProductID = _currentProduct.ID;

            _productsView = new CollectionViewSource();

            var products = Entities.GetContext().Product.ToList();
            _productsView.Source = products;
            Products.ItemsSource = _productsView.View; // Привязываем View к ComboBox
            Products.DisplayMemberPath = "Title";
            Products.SelectedValuePath = "ID";



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



        private void add_Click(object sender, RoutedEventArgs e)
        {
            {
                StringBuilder errors = new StringBuilder();
                if (Products.SelectedItem == null)
                    errors.AppendLine("Укажите продукт");
                if (string.IsNullOrWhiteSpace(ProductCountTB.Text))
                    errors.AppendLine("Укажите количество продуктов");
                bool isProductCountDigits = true;
                for (int i = 0; i < ProductCountTB.Text.Length; i++)
                {
                    if (ProductCountTB.Text[i] < '0' || ProductCountTB.Text[i] > '9')
                    {
                        isProductCountDigits = false;
                    }
                }
                if (!isProductCountDigits)
                    errors.AppendLine("Укажите численное положительное продуктов");
                if (ProductCountTB.Text =="0")
                {
                    errors.AppendLine("Укажите количество продаж");
                }
                if (string.IsNullOrWhiteSpace(saleData.Text))
                    errors.AppendLine("Укажите дату продажи");
                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return;
                }
                _currentProductSale.AgentID = _currentAgent.ID;
                _currentProductSale.ProductID = Products.SelectedIndex + 1;
                _currentProductSale.ProductCount = Convert.ToInt32(ProductCountTB.Text);
                _currentProductSale.SaleDate = Convert.ToDateTime(saleData.Text);
                if (_currentProductSale.ID == 0)
                    Entities.GetContext().ProductSale.Add(_currentProductSale);
                try
                {
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("информация сохранена");
                    Manager.MainFrame.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void delete_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    if (realize.SelectedItem != null) // Проверка на наличие выбранного элемента
                    {
                        ProductSale selectedHistory = (ProductSale)realize.SelectedItem; // Получаем выбранный объект
                        Entities.GetContext().ProductSale.Remove(selectedHistory);
                        Entities.GetContext().SaveChanges();
                        MessageBox.Show("Информация удалена!");
                        Manager.MainFrame.GoBack();
                    }
                    else
                    {
                        MessageBox.Show("Пожалуйста, выберите запись для удаления.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void searchprod_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchprod.Text.ToLower();
            _productsView.View.Filter = o =>
            {
                Product p = o as Product;
                return p != null && p.Title.ToLower().Contains(searchText);
            };
        }
    }
}
