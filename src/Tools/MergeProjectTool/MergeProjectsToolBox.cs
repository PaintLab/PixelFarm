//MIT, 2017-present, WinterDev

using System;
using System.Windows.Forms;



namespace BuildMergeProject
{
    public partial class MergeProjectsToolBox : UserControl
    {
        SolutionMx _slnMx;
        SolutionListViewController _slnListViewController;
        string _slnFilename;
        public MergeProjectsToolBox()
        {
            InitializeComponent();
        }
        public void LoadSolution(string slnFilename)
        {
            _slnFilename = slnFilename;
            ////read sln file 
            _slnMx = new SolutionMx();
            _slnMx.ReadSolution(slnFilename);

            cmbNetStd.Items.AddRange(new object[]
            {
                "classic",
                //"netstandard1.3",
                //"netstandard1.6",
                "netstandard2.0",
                //"netstandard2.1",
                "xamarin_ios",
                "xamarin_droid",
                "xamarin_ios_and_droid"
            });
            cmbNetStd.SelectedIndex = 0;

            _slnListViewController = new SolutionListViewController();
            _slnListViewController.SetSolutionListView(this.listView1);
            _slnListViewController.SetMergePlanListView(this.listView2);
            _slnListViewController.SetProjectReferenceListView(this.lstAsmReferenceList);
            _slnListViewController.LoadSolutionMx(_slnMx);
        }

        //private void cmdBuildSelectedMergePro_Click(object sender, EventArgs e)
        //{
        //    _slnListViewController.ProjectKind = null;//'classic' .net framework
        //    _slnListViewController.BuildMergeProjectFromSelectedItem();
        //}

        //private void cmdBuildMergeNetStd_Click(object sender, EventArgs e)
        //{
        //    _slnListViewController.ProjectKind = (string)cmbNetStd.SelectedItem;
        //    _slnListViewController.BuildMergeProjectFromSelectedItem();
        //}

        private void cmdBuildMerge_Click(object sender, EventArgs e)
        {
            _slnListViewController.ProjectKind = (string)cmbNetStd.SelectedItem;
            _slnListViewController.BuildMergeProjectFromSelectedItem();
        }
    }
}
