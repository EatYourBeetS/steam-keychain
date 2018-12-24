using EYB.MVVM;
using SteamKeychain.Models;

namespace SteamKeychain.ViewModels
{
    public class SteamKeyViewModel : ObservableObject<SteamKey>
    {
        public bool Hidden { get { return source.hidden; } set { SetProperty(ref source.hidden, value); } }
        public string Title { get { return source.title; } set { SetProperty(ref source.title, value); } }
        public string Content { get { return source.content; } set { SetProperty(ref source.content, value); } }
        public string StoreUrl { get { return source.storeUrl; } set { SetProperty(ref source.storeUrl, value); } }
        public string BasePrice { get { return source.basePrice.ToString(); }
            set
            {
                if (float.TryParse(value, out float price))
                {
                    SetProperty(ref source.basePrice, price);
                }
            }
        }

        public SteamKeyViewModel() : base(new SteamKey())
        {

        }

        public SteamKeyViewModel(SteamKey key) : base(key)
        {

        }
    }
}
