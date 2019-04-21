//Title          :   User control Cabinet
//Author         :   .....
//URL/Mail       :   onsaurav@yahoo.com/onsaurav@gmail.com/onsaurav@hotmail.com
//Description    :   User control Cabinet
//Created        :   Saurav Biswas / Jun-10-2011
//Modified       : 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.IO;

namespace Cabinet
{
    /// <summary>
    /// Interaction logic for Cabinet.xaml
    /// </summary>
    public partial class MyCabinet : UserControl
    {
        #region Member
        private int SlNo;
        private string strName;
        private bool ImgChange;
        private string ImagePath;
        private string ImageName;
        private bool boolSelected;
        private Image _DisplayImage = new Image();
        private List<Drawer> MyListValue = new List<Drawer>();
        private Point _previousLocation;
        private Transform _previousTransform;
        #endregion
        #region Property
        public int Sl
        {
            get { return SlNo; }
            set { SlNo = value; }
        }
        public string MyName
        {
            get { return strName; }
            set { strName = value; }
        }
        public List<Drawer> MyDrawer
        {
            get { return MyListValue; }
            set { MyListValue = value; }
        }
        public bool Selected
        {
            get { return boolSelected; }
            set { boolSelected = value; }
        }
        public bool Img_Change
        {
            get { return ImgChange; }
            set { ImgChange = value; }
        }
        public string Image_Path
        {
            get { return ImagePath; }
            set { ImagePath = value; }
        }
        public string Image_Name
        {
            get { return ImageName; }
            set { ImageName = value; }
        }
        public Image DisplayImage
        {
            get { return _DisplayImage; }
            set { _DisplayImage = value; }
        }        
        #endregion
        #region Events
        public event CabinetDragHandler Drag;
        public event CabinetResizeHandler Resize;
        #endregion
        #region Event Handlers
        public delegate void CabinetDragHandler(object sender, CabinetDragEventArgs e);
        public delegate void CabinetResizeHandler(object sender, CabinetResizeEventArgs e);
        #endregion
        #region Method
        public MyCabinet()
        {
            InitializeComponent();
            lblName.Content = MyName;
            LoadDrawer();
        }
        protected void OnDrag(CabinetDragEventArgs e)
        {
            CabinetDragHandler handler = Drag;
            if (handler != null) handler(this, e);
        }
        protected void OnResize(CabinetResizeEventArgs e)
        {
            CabinetResizeHandler handler = Resize;
            if (handler != null) handler(this, e);
        }
        private void CloseMe(object sender, RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }
        private void AddContent(object sender, RoutedEventArgs e)
        {
            try
            {
                lstDr.Items.Clear();
                lblName.Visibility = System.Windows.Visibility.Hidden;
                DrAdd.Visibility = System.Windows.Visibility.Visible;
                cboDr.Visibility = System.Windows.Visibility.Hidden;
                txtDrS.Visibility = System.Windows.Visibility.Hidden;
                txtDr.Visibility = System.Windows.Visibility.Visible;
                AddDr.Visibility = System.Windows.Visibility.Visible;
                lstDr.Visibility = System.Windows.Visibility.Visible;
                LoadDrawer();
            }
            catch { }
        }
        public void ShowName()
        {
             lblName.Content = MyName;
        }
        private void ShowContent(object sender, RoutedEventArgs e)
        {
            try
            {
                lstDr.Items.Clear();
                lblName.Visibility = System.Windows.Visibility.Hidden;
                DrAdd.Visibility = System.Windows.Visibility.Visible;
                cboDr.Visibility = System.Windows.Visibility.Visible;
                txtDrS.Visibility = System.Windows.Visibility.Visible;
                txtDr.Visibility = System.Windows.Visibility.Hidden;
                AddDr.Visibility = System.Windows.Visibility.Hidden;
                lstDr.Visibility = System.Windows.Visibility.Visible;
                //LoadDrawer();
            }
            catch { }
        }
        private void HideContent(object sender, RoutedEventArgs e)
        {
            DrAdd.Visibility = System.Windows.Visibility.Hidden;            
        }
        private void ShowName(object sender, RoutedEventArgs e)
        {
            lblName.Content = MyName;
            lblName.Visibility = System.Windows.Visibility.Visible;
        }
        private void HideName(object sender, RoutedEventArgs e)
        {
            lblName.Visibility = System.Windows.Visibility.Hidden;            
        }        
        private void AddImage(object sender, RoutedEventArgs e)
        {
            Stream checkStream = null;
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Multiselect = false;
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Image Files (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog.Filter = "All Image Files | *.*";
            if ((bool)openFileDialog.ShowDialog())
            {
                try
                {
                    if ((checkStream = openFileDialog.OpenFile()) != null)
                    {
                        var background = new ImageBrush();
                        background.ImageSource = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
                        this.Background = background;

                        Image_Path = openFileDialog.FileName;
                        Image_Name = openFileDialog.SafeFileName;
                        Img_Change = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Problem occured, try again later");
            }

        }
        #endregion
        #region Overrides
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.CaptureMouse();
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this.ReleaseMouseCapture();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.CaptureMouse();
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Window wnd = Window.GetWindow(this);
            Point currentLocation = e.MouseDevice.GetPosition(wnd);

            var move = new TranslateTransform(
                    currentLocation.X - _previousLocation.X, currentLocation.Y - _previousLocation.Y);

            double width = double.IsNaN(this.Width) ? this.ActualWidth : this.Width;
            double height = double.IsNaN(this.Height) ? this.ActualHeight : this.Height;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.Cursor == Cursors.Hand)
                {
                    var group = new TransformGroup();
                    if (_previousTransform != null)
                    {
                        group.Children.Add(_previousTransform);
                    }
                    group.Children.Add(move);

                    this.RenderTransform = group;

                    // Invoke drag event
                    OnDrag(new CabinetDragEventArgs(currentLocation));
                }
                else if (this.Cursor == Cursors.SizeWE)
                {
                    if (width + currentLocation.X - _previousLocation.X > this.MinWidth)
                    {
                        this.Width = width + currentLocation.X - _previousLocation.X;
                    }

                    // Invoke resize event
                    OnResize(new CabinetResizeEventArgs(new Size(this.Width, this.Height)));
                }
                else if (this.Cursor == Cursors.SizeNS)
                {
                    if (height + currentLocation.Y - _previousLocation.Y > this.MinHeight)
                    {
                        this.Height = height + currentLocation.Y - _previousLocation.Y;
                    }

                    // Invoke resize event
                    OnResize(new CabinetResizeEventArgs(new Size(this.Width, this.Height)));
                }
            }
            else
            {
                const int dragHandleWidth = 3;

                var bottomHandle = new Rect(0, height - dragHandleWidth, width, dragHandleWidth);
                var rightHandle = new Rect(width - dragHandleWidth, 0, dragHandleWidth, height);

                Point relativeLocation = wnd.TranslatePoint(currentLocation, this);

                if (rightHandle.Contains(relativeLocation))
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else if (bottomHandle.Contains(relativeLocation))
                {
                    this.Cursor = Cursors.SizeNS;
                }
                else
                {
                    this.Cursor = Cursors.Hand;
                }
            }

            _previousLocation = currentLocation;
            _previousTransform = this.RenderTransform;

            base.OnMouseMove(e);
        }

        public override string ToString()
        {
            string details = string.Empty;

            double width = this.Width == double.NaN ? this.ActualWidth : this.Width;
            double height = this.Height == double.NaN ? this.ActualHeight : this.Height;

            if (this.Cursor == Cursors.Hand)
            {
                Window wnd = Window.GetWindow(this);
                Point location = this.TranslatePoint(new Point(0, 0), wnd);

                details = " : X = " + location.X + ", Y = " + location.Y;
            }
            else if (this.Cursor == Cursors.SizeWE)
            {
                details = " : Width2 = " + width;
            }
            else if (this.Cursor == Cursors.SizeNS)
            {
                details = " : Height2 = " + height;
            }

            return this.Name + details;
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void AddDr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool AddState = true;
                if (txtDr.Text.Trim() != "")
                {
                    Drawer oDrawer = new Drawer();
                    for (int i = 0; i < MyListValue.Count; i++)
                    {
                        if (MyListValue[i]._Name.ToString().Trim().ToLower() == txtDr.Text.Trim().ToLower())
                        {
                            AddState = false;
                        }
                    }
                    if (AddState == true)
                    {
                        oDrawer._Name = txtDr.Text.Trim();
                        oDrawer._List = new List<string>();
                        MyListValue.Add(oDrawer);
                        txtDr.Text = "";
                    }
                    LoadDrawer();
                }
            }
            catch { }
        }

        public void LoadDrawer()
        {
            try
            {
                cboDr.Items.Clear(); lstDr.Items.Clear();
                for (int i = 0; i < MyListValue.Count; i++)
                {
                    cboDr.Items.Add(MyListValue[i]._Name);
                    lstDr.Items.Add(MyListValue[i]._Name);
                }
            }
            catch { }
        }

        private void lstDr_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           // ShowContent(sender, e);
        }

        private void cboDr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                lstDr.Items.Clear();
                if (cboDr.SelectedItem.ToString().Trim().ToLower() != "")
                {
                    for (int i = 0; i < MyListValue.Count; i++)
                    {
                        if (MyListValue[i]._Name.ToString().Trim().ToLower() == cboDr.SelectedItem.ToString().Trim().ToLower())
                        {
                            for (int j = 0; j < MyListValue[i]._List.Count; j++)
                            {
                                if (MyListValue[i]._List[j].ToString().Trim() != "") lstDr.Items.Add(MyListValue[i]._List[j]);
                            }
                        }
                    }
                }
            }
            catch {}
        }

        private void txtDrS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    bool st = true;
                    if (cboDr.SelectedItem.ToString().Trim().ToLower() != "" && txtDrS.Text.Trim().ToLower() != "")
                    {
                        for (int i = 0; i < MyListValue.Count; i++)
                        {
                            if (MyListValue[i]._Name.ToString().Trim().ToLower() == cboDr.SelectedItem.ToString().Trim().ToLower())
                            {
                                for (int j = 0; j < MyListValue[i]._List.Count; j++)
                                {
                                    if (MyListValue[i]._List[j].Trim().ToLower() == txtDrS.Text.Trim().ToLower()) st = false;
                                }
                                if (st == true) MyListValue[i]._List.Add(txtDrS.Text.Trim());
                                txtDrS.Text = "";
                            }
                        }
                    }


                    lstDr.Items.Clear();
                    if (cboDr.SelectedItem.ToString().Trim().ToLower() != "")
                    {
                        for (int i = 0; i < MyListValue.Count; i++)
                        {
                            if (MyListValue[i]._Name.ToString().Trim().ToLower() == cboDr.SelectedItem.ToString().Trim().ToLower())
                            {
                                for (int j = 0; j < MyListValue[i]._List.Count; j++)
                                {
                                    lstDr.Items.Add(MyListValue[i]._List[j]);
                                }
                            }
                        }
                    }
                }
                catch { }
            }            
        }

    }
    #region Event Arguments

    /// <summary>
    /// Event arguments to be passed into the drag event.
    /// </summary>
    public class CabinetDragEventArgs : EventArgs
    {
        private Point _location;

        public CabinetDragEventArgs(Point location)
        {
            _location = location;
        }

        public int X
        {
            get
            {
                return (int)_location.X;
            }
        }

        public int Y
        {
            get
            {
                return (int)_location.Y;
            }
        }
    }

    /// <summary>
    /// Event arguments to be passed into the resize event.
    /// </summary>
    public class CabinetResizeEventArgs : EventArgs
    {
        private Size _size;

        public CabinetResizeEventArgs(Size size)
        {
            _size = size;
        }

        public int Width
        {
            get
            {
                return (int)_size.Width;
            }
        }

        public int Height
        {
            get
            {
                return (int)_size.Height;
            }
        }
    }

    #endregion
}
