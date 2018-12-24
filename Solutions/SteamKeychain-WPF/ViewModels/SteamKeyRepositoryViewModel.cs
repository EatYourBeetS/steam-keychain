using EYB.FileManager;
using EYB.MVVM;
using SteamKeychain.Data;
using SteamKeychain.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SteamKeychain.ViewModels
{
    public class SteamKeyRepositoryViewModel : ObservableObject
    {
        private static IsThereAnyDealAPI webApi = new IsThereAnyDealAPI();

        private SteamKeyRepository _repository = null;
        private ObservableCollection<SteamKeyViewModel> _models = new ObservableCollection<SteamKeyViewModel>();
        private int _selectedIndex = -1;

        public ObservableCollection<SteamKeyViewModel> Models { get { return _models; } set { SetProperty(ref _models, value); } }
        public int SelectedIndex { get { return _selectedIndex; } set { if (SetProperty(ref _selectedIndex, value)) { RaisePropertyChanged(nameof(SelectedModel)); } } }
        public SteamKeyViewModel SelectedModel { get { return (_selectedIndex >= 0) ? _models[_selectedIndex] : null; } }

        public Command Search { get; }
        public Command Create { get; }
        public Command Delete { get; }

        public SteamKeyRepositoryViewModel(JsonFile file)
        {
            Search = new Command(_Search);
            Create = new Command(_Create);
            Delete = new Command(_Delete);

            _repository = new SteamKeyRepository(file);

            foreach (var key in _repository.Items.Where(x => x.hidden == false).OrderBy(x => x.title))
            {
                var model = new SteamKeyViewModel(key);
                model.PropertyChanged += Model_PropertyChanged;

                _models.Add(model);
            }
        }

        public string GeneratePlainText(bool includeHidden)
        {
            var builder = new StringBuilder();
            foreach (var item in _repository.Items.Where(x => includeHidden || x.hidden == false).OrderByDescending(x => x.basePrice))
            {
                if (item.basePrice < 0)
                {
                    builder.AppendFormat("- [  ???? ]");
                }
                if (item.basePrice >= 10)
                {
                    builder.AppendFormat("- [{0:0.00}€] ", item.basePrice);
                }
                else
                {
                    builder.AppendFormat("- [  {0:0.00}€] ", item.basePrice);
                }

                if (item.storeUrl.StartsWith("http"))
                {
                    builder.AppendFormat("[{0}]({1})", item.title, item.storeUrl);
                }
                else
                {
                    builder.Append(item.title);
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        private void _Search()
        {
            var res = webApi.SearchGame(SelectedModel.Title);
            if (res.data != null)
            {
                var prices = webApi.SearchPrice(res.data);
                var game = prices.data?.Values.FirstOrDefault();
                if (game != null)
                {
                    var l = game.list.ElementAtOrDefault(0);
                    if (l != null)
                    {
                        SelectedModel.StoreUrl = l.url;
                        SelectedModel.BasePrice = l.price_old.ToString();
                    }
                    else
                    {
                        SelectedModel.StoreUrl = "Not Found";
                        SelectedModel.BasePrice = "-1";
                    }
                }
            }
        }

        private void _Create()
        {
            var model = new SteamKeyViewModel();
            model.PropertyChanged += Model_PropertyChanged;

            _models.Add(model);
            _repository.Add(model);

            SelectedIndex = Models.IndexOf(model);
        }

        private void _Delete()
        {
            var index = _selectedIndex;
            if (index >= 0)
            {
                var model = Models[index];
                model.PropertyChanged -= Model_PropertyChanged;

                _models.RemoveAt(index);
                _repository.Delete(model);

                if (_models.Count > 0)
                {
                    SelectedIndex = Math.Max(index - 1, 0);
                }
            }
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _repository.Update(sender as SteamKeyViewModel);
        }
    }
}