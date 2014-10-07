using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using System.Net.Http;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace JuicyReddit
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private Reddit reddit = new Reddit();
        private List<Page.Topic> pTopics;
        private string url = "http://reddit.com/.json";
        private string[] filters = new string [] { "hot", "new", "rising", "controversial", "top" };
        private string selectedFilter;
        private string searchBoxSubreddit;
        private string currentSubreddit = "r/frontpage";
        private string[] defaultSubreddits;
        private List<Comments.CommentsObject> comments;
        private int topicSelectedIndex = 0;
        private int dropdownCount;
        private bool wasRefreshed = false;
        private bool isSubredditRefresh = false;
        private bool searchButtonClicked = false;
        private bool wasShowCommentsClicked = false;
        private bool wasNextButtonPressed = false;
        private bool isDropdownRefresh = false;
        private int refreshCount;

        //Initializes the page
        public MainPage()
        {
            this.InitializeComponent();

            AddElementsToDropdown();
            FilterDropdown.SelectedIndex = 0;
            selectedFilter = filters[0].ToString();


            ShowComments.Visibility = Visibility.Collapsed;
            SearchBox.Visibility = Visibility.Collapsed;
            searchTextBlock.Visibility = Visibility.Collapsed;
            searchButton.Visibility = Visibility.Collapsed;
            CommentsListView1.Visibility = Visibility.Collapsed;
            OriginalTopicBorder.Visibility = Visibility.Collapsed;
            SeperatorOne.Visibility = Visibility.Collapsed;
            SeperatorTwo.Visibility = Visibility.Collapsed;
            CurrentSubredditTextBlock.Visibility = Visibility.Collapsed;
            FilterDropdown.Visibility = Visibility.Collapsed;
            WebView1.Visibility = Visibility.Collapsed;
        }

        //When the application is started it performs this
        override
        protected async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MessageDialog md = new MessageDialog("Network connection not detected");
                await md.ShowAsync();
                Application.Current.Exit();
            }
            SetCorrectUrl();
            defaultSubreddits = reddit.GetDefaultSubreddits();
            //pTopics = await reddit.GetTopicList(url);
            pTopics = new List<Page.Topic>();
            //subreddits = await reddit.GetSubredditList(url, defaultSubreddits);
            PopulateTopicListView();
            PopulateDefaultSubreddits();
        } 

        //Populates the left hand view of all the topics
        private void PopulateTopicListView()
        {
            //topicsListView.ItemsSource = pTopics;


            for (int i = 0; i < pTopics.Count; i++)
            {
                //This bit of code was to handle images but couldn't get it working
                if (!String.IsNullOrEmpty(pTopics[i].thumbnail) && pTopics[i].thumbnail.Contains("http"))
                {
                   
                    topicsListView.Items.Add(pTopics[i].title + "\n" + pTopics[i].author + " " + pTopics[i].timeposted + " hours ago" + "\n" + pTopics[i].points + " points\t"
                        + pTopics[i].commentCount + " comments\n" + "[" + pTopics[i].subreddit + "]");
                }
                else
                {
                    topicsListView.Items.Add(pTopics[i].title + "\n" + pTopics[i].author + " " + pTopics[i].timeposted + " hours ago" + "\n" + pTopics[i].points + " points\t"
                        + pTopics[i].commentCount + " comments\n" + "[" + pTopics[i].subreddit + "]");
                }

            }

            //topicsListView.ItemsSource = commentsList;

            SeperatorOne.Visibility = Visibility.Visible;
            CurrentSubredditTextBlock.Visibility = Visibility.Visible;
            FilterDropdown.Visibility = Visibility.Visible;
        }

        //Gets the next 25 topics
        private void LoadMoreTopics()
        {
            RemoveItemsFromTopicListView();


        }

        //Gets image thumbnail
        private async Task<Image> GetImage(string url)
        {
            var httpClient = new HttpClient();
            var contentBytes = await httpClient.GetByteArrayAsync(new System.Uri(url));
            var ims = new InMemoryRandomAccessStream();
            var dataWriter = new DataWriter(ims);
            dataWriter.WriteBytes(contentBytes);
            await dataWriter.StoreAsync();
            ims.Seek(0);

            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(ims);

            Image myImage = new Image();
            myImage.Source = bitmap;
            return myImage;
        }

        //Removes items from topic list view
        private void RemoveItemsFromTopicListView()
        {
            topicsListView.ItemsSource = null;
            topicsListView.Items.Clear();
        }

        //Clears the comments listview
        private void RemoveItemsFromCommentListView()
        {
            CommentsListView1.ItemsSource = null;
            //CommentsListView1.Items.Clear();

        }

        //Populates the default subreddit listview
        private void PopulateDefaultSubreddits()
        {
            
            string[] subredditList = reddit.GetDefaultSubreddits();

            for (int i = 0; i < defaultSubreddits.Length; i++)
            {
                //string subredditUrl = "http://www.reddit.com/"+ subredditList[i] + "/about.json";

                //if (subredditUrl.Equals("http://www.reddit.com/r/front/about.json") || subredditUrl.Equals("http://www.reddit.com/r/all/about.json"))
                 //   subredditUrl = "http://reddit.com/.json";
                //Subreddit.Data subreddit = await reddit.GetSubreddit(subredditUrl, i);

               // if(String.IsNullOrEmpty(subreddit.description))
                    SubredditList.Items.Add(defaultSubreddits[i] + "\n");
                //else
                //    SubredditList.Items.Add(defaultSubreddits[i] + "\n" + subreddit.description);
            }

            SeperatorTwo.Visibility = Visibility.Visible;

            SearchBox.Visibility = Visibility.Visible;
            searchTextBlock.Visibility = Visibility.Visible;
            searchButton.Visibility = Visibility.Visible;
        }


        //Populates comments listview
        private void PopulateComments(List<Comments.CommentsObject> comments, double commentCount)
        {
            List<string> commentsList = new List<string>();

            foreach (var child in comments[1].data.children)
            {
                commentsList.Add(child.data.body);
            }

            CommentsListView1.ItemsSource = commentsList;
        }


        //Gets the original text post by a user
        private void SetOriginalPost(int selectedIndex)
        {
            originalPostTextBox.Text = pTopics[selectedIndex].text;
       
        }

        //Handles pressing on a topic
        private async void topicsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveItemsFromCommentListView();

            SetCorrectUrl();
            Page.Topic topic;



            if ((wasRefreshed && refreshCount != 1) || (isDropdownRefresh && dropdownCount != 1))
            {
                topic = pTopics[topicSelectedIndex];
                ShowComments.Visibility = Visibility.Collapsed;
                WebView1.Visibility = Visibility.Collapsed;
                OriginalTopicBorder.Visibility = Visibility.Collapsed;
            }
            else
                topic = pTopics[topicsListView.SelectedIndex];

            if(!String.IsNullOrEmpty(topic.linkExternal) &&  !topic.linkExternal.Contains("wwww.reddit.com") && isSubredditRefresh == false)
            {
                CommentsListView1.Visibility = Visibility.Collapsed;
                ShowComments.Visibility = Visibility.Visible;
                WebView1.Visibility = Visibility.Visible;
                WebView1.Source = new System.Uri(topic.linkExternal);
                

                OriginalTopicBorder.Visibility = Visibility.Collapsed;
            }

            if (!String.IsNullOrWhiteSpace(topic.text) && searchButtonClicked == true)
            {
                SetOriginalPost(topicsListView.SelectedIndex);
                originalPostTextBox.Text = topic.text;

                ShowComments.Visibility = Visibility.Collapsed;
                OriginalTopicBorder.Visibility = Visibility.Visible;
                WebView1.Visibility = Visibility.Collapsed;
            }


            if (searchButtonClicked)
            {
                WebView1.Visibility = Visibility.Collapsed;
                OriginalTopicBorder.Visibility = Visibility.Collapsed;
            }

            isDropdownRefresh = false;
            isSubredditRefresh = false;
            wasRefreshed = false;
            searchButtonClicked = false;

            comments = await reddit.GetComments(topic.subreddit, topic.commentId);
            PopulateComments(comments, topic.commentCount);
        }

        //Changes subreddit
        private void SubredditList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isSubredditRefresh = true;
            string selectedSubreddit = defaultSubreddits[SubredditList.SelectedIndex];
            currentSubreddit = selectedSubreddit;
            url = "http://reddit.com/"+selectedSubreddit+"/"+selectedFilter+".json";

            WebView1.Visibility = Visibility.Collapsed;
            OriginalTopicBorder.Visibility = Visibility.Collapsed;

            RefreshTopics(selectedSubreddit, selectedFilter);
        }

        //Refreshes the topic lists
        private async void RefreshTopics(string selectedSubreddit, string selectedFilter)
        {
            wasRefreshed = true;
            refreshCount++;
            SetCorrectUrl();

            try
            {
                pTopics = await reddit.GetTopicList(url);

            }
            catch (Exception e)
            {
                MessageDialog md = new MessageDialog(e.Message);
            }

            RemoveItemsFromTopicListView();
            CurrentSubredditTextBlock.Text = selectedSubreddit;
            PopulateTopicListView();
        }

        private void originalPostTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        //Adds elements into the dropdown filter list
        private void AddElementsToDropdown()
        {
            for (int i = 0; i < filters.Length; i++)
            {
                FilterDropdown.Items.Add(filters[i]);
            }
        }

        //Do check for front page etc
        private void SetCorrectUrl()
        {
            if (url.Equals("http://reddit.com/.json") && searchButtonClicked == false)
            {
                if(selectedFilter.Equals("hot"))
                    url.Equals("http://reddit.com/.json");
                else
                    url = "http://reddit.com/" + selectedFilter + ".json";
            }
            else if(searchButtonClicked == true)
                url = "http://reddit.com/" + searchBoxSubreddit + "/" + selectedFilter + ".json";
            else
                url = "http://reddit.com/" + currentSubreddit + "/" + selectedFilter + ".json";

            if (url.Contains("frontpage") || url.Contains("front"))
                url = "http://reddit.com/" + selectedFilter + ".json";

            if (wasNextButtonPressed)
                url = "http://www.reddit.com/" + currentSubreddit + "/?count=25&after=t3_1uvo1l";

            searchButtonClicked = false;
            wasNextButtonPressed = false;
        }

        private void FilterDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isDropdownRefresh = true;
            dropdownCount++;
            selectedFilter = filters[FilterDropdown.SelectedIndex];
            SetCorrectUrl();
            RefreshTopics(currentSubreddit, selectedFilter);
        }

        private async void searchButton_Click(object sender, RoutedEventArgs e)
        {
            searchButtonClicked = true;
            if (String.IsNullOrEmpty(SearchBox.Text))
            {
                MessageDialog md = new MessageDialog("Must enter a subreddit to search");
                await md.ShowAsync();
            }
            else 
            {
                searchBoxSubreddit = "r/" + SearchBox.Text;
                RefreshTopics(searchBoxSubreddit, selectedFilter);
                CurrentSubredditTextBlock.Text = searchBoxSubreddit;

            }

        }

        //Shows comments behind a webpage
        private void ShowComments_Click(object sender, RoutedEventArgs e)
        {
            if (wasShowCommentsClicked == false)
            {
                CommentsListView1.Visibility = Visibility.Visible;
                WebView1.Visibility = Visibility.Collapsed;
                wasShowCommentsClicked = true;
            }
            else
            {
                CommentsListView1.Visibility = Visibility.Collapsed;
                WebView1.Visibility = Visibility.Visible;
                wasShowCommentsClicked = false;
            }

        }

        //Load more topics
        private void LoadMoreTopicsButton_Click(object sender, RoutedEventArgs e)
        {
            wasNextButtonPressed = true;
            RefreshTopics(currentSubreddit, selectedFilter);
        }
    }
}
