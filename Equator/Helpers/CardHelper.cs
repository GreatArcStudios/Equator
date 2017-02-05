using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Equator.Helpers
{
    public class CardHelper
    {
        private Uri _musicThumbUri;

        public void UpdateCardText(Label textLabel, String songTitle)
        {
            textLabel.Content = songTitle;
           
        }

        public void SetMusicThumbUri(Uri currentUri)
        {
            _musicThumbUri = currentUri;
        }

        public Uri GetMusicThumbUri()
        {
            return _musicThumbUri;
        }
    }
}
