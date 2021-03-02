using Configurate.TemplateObjects;
using System.Windows.Controls;
using Configurate.Managers;
using System.Windows;

namespace Configurate.Tools
{
    class PostButton
    {
        // VARIABLES
        public readonly Button Button;
        private readonly PostTO myPost;

        // EXECUTION FUNCTIONS
        public PostButton(PostTO post)
        {
            myPost = post;
            Button = UIManager.CreateSharedPostButton(post.Owner, post.Description, new RoutedEventHandler(OnClick));
        }

        // METHODS
        private void OnClick(object sender, RoutedEventArgs eventArgs)
        {

        }
    }
}
